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
            .GroupBy(t => new { t.goalCategory, t.biome, t.minRarity, t.maxRarity })
            .Select(g => g.First())
            .OrderBy(_ => Random.value)
            .Take(3);

        foreach (var t in candidates)
        {
            int availableCount = ObjectRegistry.Instance.GetRemainingCount(t.goalCategory);
        
            // 🔥 Вычисляем количество целей на основе доступных объектов
            int actualCount = CalculateQuestCount(t, availableCount);
        
            // Пропускаем квест, если нет объектов
            if (actualCount <= 0) continue;

            model.ActiveQuests.Add(new ActiveQuest {
                template = t,
                requiredCount = actualCount
            });
        }

        journalUI.Refresh(model.ActiveQuests);
    }
    
    private int CalculateQuestCount(QuestTemplate template, int availableCount)
    {
        if (availableCount <= 0) return 0;

        // Для редких объектов — меньше целей
        int adjustedMax = template.maxTargetCount;
        if (template.minRarity > 2) // редкость 3-4
        {
            adjustedMax = Mathf.Min(adjustedMax, 2);
        }

        int min = Mathf.Max(1, template.minTargetCount);
        int max = Mathf.Min(availableCount, adjustedMax);

        if (availableCount < min) return Mathf.Min(1, availableCount);
        if (min > max) min = max;

        return Random.Range(min, max + 1);
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