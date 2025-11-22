using UnityEngine;
using System.Collections.Generic;

public class QuestJournalUI : MonoBehaviour
{
    [SerializeField] private QuestEntryUI entryPrefab;
    [SerializeField] private Transform container;

    private List<QuestEntryUI> entries = new();

    public void Refresh(List<ActiveQuest> quests)
    {
        Clear();
        foreach (var q in quests)
        {
            var entry = Instantiate(entryPrefab, container);
            entry.Initialize(q.template, q.currentProgress, q.requiredCount);
            entries.Add(entry);
        }
    }

    public void Clear()
    {
        foreach (var e in entries) Destroy(e.gameObject);
        entries.Clear();
    }
}