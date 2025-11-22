using System;
using System.Collections.Generic;

public class QuestModel
{
    public List<ActiveQuest> ActiveQuests { get; private set; } = new();

    public event Action<ActiveQuest> OnQuestProgressed;
    public event Action<ActiveQuest> OnQuestCompleted;

    public bool ProcessScanResult(ScanResult result)
    {
        bool changed = false;
        foreach (var quest in ActiveQuests)
        {
            if (quest.TryProgress(result))
            {
                changed = true;
                OnQuestProgressed?.Invoke(quest);
                if (quest.isCompleted)
                    OnQuestCompleted?.Invoke(quest);
            }
        }
        return changed;
    }

    public void Clear() => ActiveQuests.Clear();
}