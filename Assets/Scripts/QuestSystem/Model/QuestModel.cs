using System;
using System.Collections.Generic;
using System.Linq;

public class QuestModel
{
    public List<ActiveQuest> ActiveQuests { get; private set; } = new();

    public event Action<ActiveQuest> OnQuestProgressed;
    public event Action<ActiveQuest> OnQuestCompleted;

    public bool ProcessScanResult(ScanResult result)
    {
        bool changed = false;
        var questsCopy = new List<ActiveQuest>(ActiveQuests);
        foreach (var quest in questsCopy)
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
    
    public bool AreAllQuestsCompletedOrFailed()
    {
        return ActiveQuests.Count == 0 || 
               ActiveQuests.All(q => 
                   q.status == QuestStatus.Completed || 
                   q.status == QuestStatus.Failed);
    }

    public void Clear() => ActiveQuests.Clear();
}