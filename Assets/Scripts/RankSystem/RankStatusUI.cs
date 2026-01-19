using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RankStatusUI : MonoBehaviour
{
    [Header("Текстовые поля")]
    [SerializeField] private TextMeshProUGUI rankTitleText;    // Название ранга
    [SerializeField] private TextMeshProUGUI xpValueText;      // Число XP (например, 150 / 500)
    [SerializeField] private TextMeshProUGUI statusText;       // "Квалификация подтверждена"
    
    [Header("Визуализация")]
    [SerializeField] private Image xpProgressBar;              // Полоска прогресса (Fill Amount)
    [SerializeField] private Color perfectDoneColor = Color.green;
    [SerializeField] private Color perfectMissingColor = Color.red;

    [Header("Настройки RankSettings")]
    [SerializeField] private RankSettings rankSettings;

    void Start()
    {
        UpdateDisplay();
    }

    // Вызываем обновление в Start или через события, чтобы не грузить Update
    public void UpdateDisplay()
    {
        if (RankManager.Instance == null) return;

        int currentRank = RankManager.Instance.CurrentRank;
        int currentXP = RankManager.Instance.CurrentXP;
        bool isPerfectDone = RankManager.Instance.PerfectMissionDoneForRank;

        // 1. Название текущего ранга
        if (currentRank < rankSettings.levels.Count)
        {
            rankTitleText.text = $"РАНГ: {rankSettings.levels[currentRank].rankName}";
        }

        // 2. Логика XP и прогресс-бара
        if (currentRank + 1 < rankSettings.levels.Count)
        {
            int requiredXP = rankSettings.levels[currentRank + 1].requiredXP;
            xpValueText.text = $"{currentXP} / {requiredXP} XP";
            
            if (xpProgressBar != null)
            {
                xpProgressBar.fillAmount = (float)currentXP / requiredXP;
            }
        }
        else
        {
            xpValueText.text = "МАКСИМАЛЬНЫЙ РАНГ";
            if (xpProgressBar != null) xpProgressBar.fillAmount = 1f;
        }

        // 3. Статус "Идеальной высадки" (Квалификация)
        if (isPerfectDone)
        {
            statusText.text = "КВАЛИФИКАЦИЯ: ПОДТВЕРЖДЕНА";
            statusText.color = perfectDoneColor;
        }
        else
        {
            statusText.text = "КВАЛИФИКАЦИЯ: ТРЕБУЕТСЯ ИДЕАЛЬНАЯ ВЫСАДКА";
            statusText.color = perfectMissingColor;
        }
    }
}