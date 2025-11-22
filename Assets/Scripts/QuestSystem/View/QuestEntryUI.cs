using UnityEngine;
using TMPro;

public class QuestEntryUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private TextMeshProUGUI rewardText;

    public void Initialize(QuestTemplate template, int current, int required)
    {
        titleText.text = template.displayName;
        progressText.text = $"{current}/{required}";
        rewardText.text = $"${template.rewardMoney} + {template.rewardResearchPoints} RP";
    }

    public void UpdateProgress(int current, int required)
    {
        progressText.text = $"{current}/{required}";
    }
}