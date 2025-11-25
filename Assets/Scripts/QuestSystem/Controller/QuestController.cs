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

    void Awake()
    {
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

    // Вызывается при высадке на планету
    public void GenerateBaseQuests(Biome biome, WeatherCondition weather)
    {
        model.Clear();

        var candidates = templateRegistry.allTemplates
            .Where(t => t.faction == Faction.None)
            .Where(t => t.biome == biome)
            .Where(t => !t.requiresWeather || t.weather == weather)
            .GroupBy(t => new { t.goalCategory, t.biome, t.minRarity, t.maxRarity }) // ← группировка по смыслу
            .Select(g => g.First()) // ← берём только один шаблон на тип
            .OrderBy(_ => Random.value)
            .Take(3);

        foreach (var t in candidates)
        {
            int actualCount = Random.Range(t.minTargetCount, t.maxTargetCount + 1);
            model.ActiveQuests.Add(new ActiveQuest {
                template = t,
                requiredCount = actualCount
            });
        }

        journalUI.Refresh(model.ActiveQuests);
    }
    
    void OnQuestCompleted(ActiveQuest quest)
    {
        if (quest.status == QuestStatus.Completed)
        {
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
    
    private void OnObjectDestroyed(DataCategory category, int remainingCount)
    {
        var affectedQuests = model.ActiveQuests
            .Where(q => q.status == QuestStatus.Active && q.template.goalCategory == category)
            .ToList();
        
        foreach (var quest in affectedQuests)
        {
            int stillNeeded = quest.requiredCount - quest.currentProgress;
            if (remainingCount < stillNeeded)
            {
                quest.status = QuestStatus.Failed;
                journalUI.Refresh(model.ActiveQuests);
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
}