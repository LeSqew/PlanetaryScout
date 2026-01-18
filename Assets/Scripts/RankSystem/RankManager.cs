using UnityEngine;

public class RankManager : MonoBehaviour
{
    public static RankManager Instance { get; private set; }

    [SerializeField] private RankSettings settings;

    public int CurrentXP { get; private set; }
    public int CurrentRank { get; private set; }
    public bool PerfectMissionDoneForRank { get; private set; }

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
    }

    public void ProcessMissionResults(MissionReport report, bool hadMinigameErrors, bool playerDied)
    {
        int totalEarnedXP = 0;

        foreach (var quest in report.Quests)
        {
            // Ранг-очки на основе твоей схемы (R1=50, R2=100...)
            int baseValue = quest.maxRarity * 50;

            if (quest.status == QuestStatus.Completed)
                totalEarnedXP += baseValue;
            else if (quest.status == QuestStatus.Failed)
                totalEarnedXP -= (baseValue / 2); // Штраф за провал
        }

        // Идеальная высадка: Все квесты выполнены + нет ошибок + игрок жив
        bool isPerfect = report.CompletedQuests == report.TotalQuests && !hadMinigameErrors && !playerDied;

        if (isPerfect)
        {
            totalEarnedXP = Mathf.RoundToInt(totalEarnedXP * 1.5f); // Бонус 50%
            PerfectMissionDoneForRank = true;
        }

        CurrentXP = Mathf.Max(0, CurrentXP + totalEarnedXP);
        CheckRankUp();
    }

    private void CheckRankUp()
    {
        if (CurrentRank + 1 >= settings.levels.Count) return;

        RankLevel nextLevel = settings.levels[CurrentRank + 1];

        if (CurrentXP >= nextLevel.requiredXP && PerfectMissionDoneForRank)
        {
            CurrentRank++;
            PerfectMissionDoneForRank = false; // Сброс условия для следующего ранга
            Debug.Log($"<color=green>РАНГ ПОВЫШЕН ДО: {CurrentRank}</color>");
        }
    }
}