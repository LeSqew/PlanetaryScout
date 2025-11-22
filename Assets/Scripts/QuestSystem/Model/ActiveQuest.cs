using System;

[Serializable]
public class ActiveQuest
{
    public QuestTemplate template;
    public int requiredCount;      // конечное число целей (сгенерировано при высадке)
    public int currentProgress = 0;
    public bool isCompleted = false;

    public bool TryProgress(ScanResult result)
    {
        if (isCompleted || !result.success) return false;
        if (result.category != template.goalCategory) return false;
        if (result.rarity < template.minRarity || result.rarity > template.maxRarity) return false;
        if (result.biome != template.biome) return false;
        if (template.requiresWeather && result.weather != template.weather) return false;

        currentProgress++;
        if (currentProgress >= requiredCount)
            isCompleted = true;

        return true;
    }
}