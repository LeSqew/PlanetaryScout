using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class QuestController : MonoBehaviour
{
    [Header("Данные")]
    public QuestTemplateRegistry templateRegistry;

    [Header("Ссылки")]
    public QuestJournalUI journalUI;

    private QuestModel model;
    public bool AreAllQuestsCompletedOrFailed => model.AreAllQuestsCompletedOrFailed();
    public static event Action OnAllQuestsCompleted;
    public static QuestController Instance { get; private set; }

    void Awake()
    {
        Instance = this;
        model = new QuestModel();
    }

    void OnEnable()
    {
        DataCollectionEvents.OnDataCollected += OnDataCollected;
        DataCollectionEvents.OnScannableObjectDestroyed += OnObjectDestroyed;
        model.OnQuestCompleted += OnQuestCompleted;
    }

    void OnDisable()
    {
        DataCollectionEvents.OnDataCollected -= OnDataCollected;
        DataCollectionEvents.OnScannableObjectDestroyed -= OnObjectDestroyed;
    }

    public void GenerateBaseQuests(Biome biome, WeatherCondition weather)
    {
        model.Clear();

        var candidates = templateRegistry.allTemplates
            .Where(t => t.faction == Faction.None)
            .Where(t => t.biome == biome)
            .Where(t => !t.requiresWeather || t.weather == weather)
            .GroupBy(t => t.goalCategory) // ← группировка только по категории
            .Select(g => g.First())
            .OrderBy(_ => Random.value)
            .Take(3);

        foreach (var t in candidates)
        {
            var objects = ObjectRegistry.Instance.GetObjects(t.goalCategory);
            if (objects.Count == 0) continue;

            var rarities = objects.Where(o => o != null).Select(o => o.rarity).ToList();
            int minRarity = rarities.Min();
            int maxRarity = rarities.Max();
            
            int totalCount = objects.Count;
            int minCount = Mathf.Max(1, Mathf.CeilToInt(totalCount * 0.1f));
            int maxCount = Mathf.Min(totalCount, Mathf.FloorToInt(totalCount * 0.3f));
            int requiredCount = Random.Range(minCount, maxCount + 1);

            model.ActiveQuests.Add(new ActiveQuest
            {
                template = t,
                requiredCount = requiredCount,
                minRarity = minRarity,
                maxRarity = maxRarity
            });
        }

        journalUI.Refresh(model.ActiveQuests);
    }

    void OnQuestCompleted(ActiveQuest quest)
    {
        if (quest.status == QuestStatus.Completed || quest.status == QuestStatus.Failed)
        {
            model.CompletedQuests.Add(quest);
            model.ActiveQuests.Remove(quest);
            journalUI.Refresh(model.ActiveQuests);
            CheckIfAllQuestsCompleted();
        }
    }
    void OnDataCollected(ScanResult result)
    {
        if (model.ProcessScanResult(result))
        {
            journalUI.Refresh(model.ActiveQuests); // или обновить частично
            CheckIfAllQuestsCompleted();
        }
    }
    
    private void OnObjectDestroyed(DataCategory category)
    {
        int actualRemaining = ObjectRegistry.Instance.GetRemainingCount(category);

        var affectedQuests = model.ActiveQuests
            .Where(q => q.status == QuestStatus.Active && q.template.goalCategory == category)
            .ToList();
    
        foreach (var quest in affectedQuests)
        {
            int stillNeeded = quest.requiredCount - quest.currentProgress;
            if (actualRemaining < stillNeeded)
            {
                quest.status = QuestStatus.Failed;
                OnQuestCompleted(quest);
            }
        }
        CheckIfAllQuestsCompleted();
    }
    
    private void CheckIfAllQuestsCompleted()
    {
        if (AreAllQuestsCompletedOrFailed)
        {
            OnAllQuestsCompleted?.Invoke();
        }
    }

    public MissionReport GenerateMissionReport()
    {
        var allQuests = model.GetAllQuests();
        return new MissionReport(allQuests);
    }
}


public class MissionReport
{
    public int TotalQuests { get; }
    public int CompletedQuests { get; }
    public int FailedQuests { get; }
    public List<ActiveQuest> Quests { get; }

    public MissionReport(List<ActiveQuest> quests)
    {
        Quests = quests;
        TotalQuests = quests.Count;
        CompletedQuests = quests.Count(q => q.status == QuestStatus.Completed);
        FailedQuests = quests.Count(q => q.status == QuestStatus.Failed);
    }
}