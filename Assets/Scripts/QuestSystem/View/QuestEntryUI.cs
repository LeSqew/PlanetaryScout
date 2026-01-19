// QuestEntryUI.cs
using UnityEngine;
using TMPro;

public class QuestEntryUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private TextMeshProUGUI rarityText;

    public void Initialize(ActiveQuest quest)
    {
        titleText.text = quest.template.displayName;
    
        if (quest.status == QuestStatus.Failed)
        {
            progressText.text = "ЗАДАНИЕ НЕВОЗМОЖНО";
            progressText.color = Color.red;
            rarityText.color = Color.gray;
        }
        else
        {
            progressText.text = $"Собрано: {quest.currentProgress}/{quest.requiredCount}";
            progressText.color = Color.white;
            rarityText.color = Color.white;
        }

        string rarity;
        if (quest.minRarity == quest.maxRarity)
        {
            rarity = quest.minRarity.ToString();
        }
        else
        {
            rarity = $"{quest.minRarity}-{quest.maxRarity}";
        }
        rarityText.text = $"Редкость: {rarity}";

        if (quest.status == QuestStatus.Failed)
        {
            rarityText.color = Color.gray;
        }
    }

    public void UpdateProgress(int current, int required)
    {
        progressText.text = $"Собрано: {current}/{required}";
    }
}