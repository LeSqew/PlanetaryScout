using System;

[Serializable]
public class ActiveQuest
{
    public QuestTemplate template;
    public int requiredCount;
    public int minRarity; // ← теперь хранится здесь
    public int maxRarity; // ← 
    public int currentProgress = 0;
    public QuestStatus status = QuestStatus.Active;
    public bool isCompleted => status == QuestStatus.Completed;

    public bool TryProgress(ScanResult result)
    {
        if (status != QuestStatus.Active || !result.success) return false;
        if (result.category != template.goalCategory) return false;
        if (result.rarity < minRarity || result.rarity > maxRarity) return false;

        currentProgress++;
        if (currentProgress >= requiredCount)
            status = QuestStatus.Completed;

        return true;
    }
}