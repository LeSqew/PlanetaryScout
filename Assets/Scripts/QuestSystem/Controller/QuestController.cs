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
            .Where(t => t.faction == Faction.None)           // ← только Контора
            .Where(t => t.biome == biome)
            .Where(t => !t.requiresWeather || t.weather == weather)
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

    void OnDataCollected(ScanResult result)
    {
        if (model.ProcessScanResult(result))
        {
            journalUI.Refresh(model.ActiveQuests); // или обновить частично
        }
    }
}