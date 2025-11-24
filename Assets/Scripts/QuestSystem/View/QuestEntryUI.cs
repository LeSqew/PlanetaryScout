using UnityEngine;
using TMPro;

public class QuestEntryUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private TextMeshProUGUI rewardText;

    public void Initialize(ActiveQuest quest)
    {
        titleText.text = quest.template.displayName;
    
        if (quest.status == QuestStatus.Failed)
        {
            progressText.text = "ЗАДАНИЕ НЕВОЗМОЖНО";
            progressText.color = Color.red;
            rewardText.color = Color.gray;
        }
        else
        {
            progressText.text = $"Собрано: {quest.currentProgress}/{quest.requiredCount}";
            progressText.color = Color.white;
            rewardText.color = Color.white;
        }

        rewardText.text = $"${quest.template.rewardMoney} + {quest.template.rewardResearchPoints} RP";
        if (quest.status == QuestStatus.Failed)
        {
            rewardText.color = Color.gray;
        }
    }

    public void UpdateProgress(int current, int required)
    {
        progressText.text = $"Собрано: {current}/{required}";
    }
}