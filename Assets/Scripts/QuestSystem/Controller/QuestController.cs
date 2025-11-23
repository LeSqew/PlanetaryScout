using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class QuestController : MonoBehaviour
{
    [Header("Данные")]
    public QuestTemplateRegistry templateRegistry;

    [Header("Ссылки")]
    public QuestJournalUI journalUI;

    private QuestModel model;

    void Awake()
    {
        model = new QuestModel();
    }

    void OnEnable()
    {
        DataCollectionEvents.OnDataCollected += OnDataCollected;
        model.OnQuestCompleted += OnQuestCompleted;
    }

    void OnDisable()
    {
        DataCollectionEvents.OnDataCollected -= OnDataCollected;
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
        // 1. Удаляем из модели
        model.ActiveQuests.Remove(quest);

        // 2. Обновляем UI
        journalUI.Refresh(model.ActiveQuests);

        // 3. (Опционально) выдаём награду
        Debug.Log($"Задание завершено: {quest.template.displayName}");
        // PlayerWallet.AddMoney(quest.template.rewardMoney);
        // PlayerProgress.AddResearchPoints(quest.template.rewardResearchPoints);
    }
    void OnDataCollected(ScanResult result)
    {
        if (model.ProcessScanResult(result))
        {
            journalUI.Refresh(model.ActiveQuests); // или обновить частично
        }
    }
}