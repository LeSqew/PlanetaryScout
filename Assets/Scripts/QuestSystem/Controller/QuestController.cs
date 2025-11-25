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
            .GroupBy(t => t.goalCategory) // ← группировка только по категории
            .Select(g => g.First())
            .OrderBy(_ => Random.value)
            .Take(3);

        foreach (var t in candidates)
        {
            var objects = ObjectRegistry.Instance.GetObjects(t.goalCategory);
            if (objects.Count == 0) continue;

            // 🔥 Определяем редкость на основе доступных объектов
            var rarities = objects.Where(o => o != null).Select(o => o.rarity).ToList();
            int minRarity = rarities.Min();
            int maxRarity = rarities.Max();

            // 🔥 Определяем количество целей (например, 30-70% от общего числа)
            int totalCount = objects.Count;
            int minCount = Mathf.Max(1, Mathf.CeilToInt(totalCount * 0.3f));
            int maxCount = Mathf.Min(totalCount, Mathf.FloorToInt(totalCount * 0.7f));
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
        if (quest.status == QuestStatus.Completed)
        {
            model.ActiveQuests.Remove(quest);
            journalUI.Refresh(model.ActiveQuests);
            CheckIfAllQuestsCompleted();
        }
    }
    void OnDataCollected(ScanResult result)
    {
        Debug.Log($"📥 Событие получено: {result.category}");
        if (model.ProcessScanResult(result))
        {
            journalUI.Refresh(model.ActiveQuests); // или обновить частично
            CheckIfAllQuestsCompleted();
        }
    }
    
    private void OnObjectDestroyed(DataCategory category)
    {
        // 🔥 БОЛЬШЕ НЕ НУЖНО: remainingCount приходит извне
        // Вместо этого — получаем актуальное количество из реестра
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