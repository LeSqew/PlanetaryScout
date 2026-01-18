using Player.Health;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Utils;

public class MissionReportUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject victoryPanel;  
    [SerializeField] private GameObject deathPanel;

    [Header("Victory Panel Stats")]
    [SerializeField] private TextMeshProUGUI completedText;
    [SerializeField] private TextMeshProUGUI failedText;
    [SerializeField] private Transform questListContainer;
    [SerializeField] private QuestEntryUI questEntryPrefab;

    [Header("Action Map")]
    [SerializeField] private InputActionAsset inputActionAsset;

    public static bool IsDeathScreenActive { get; private set; }

    private InputActionMap UIActionMap;
    private InputActionMap playerMap;

    private HealthController _healthController;
    private static MissionReportUI _instance;
    public static MissionReportUI Instance => _instance;

    void Awake()
    {
        IsDeathScreenActive = false;
        _healthController = FindObjectOfType<HealthController>();
        if (_healthController == null)
        {
            Debug.LogError("HealthController �� ������ �� �����!");
        }

        QuestController.OnAllQuestsCompleted += OnAllQuestsCompleted;
        if (_healthController != null)
        {
            _healthController.death+= OnPlayerDeath;
        }
        UIActionMap = inputActionAsset.FindActionMap("UI", true);
        playerMap = inputActionAsset.FindActionMap("Player", true);

    }
    void OnDestroy()
    {
        QuestController.OnAllQuestsCompleted -= OnAllQuestsCompleted;
        if (_healthController != null)
        {
            _healthController.death -= OnPlayerDeath;
        }
    }

    private void OnAllQuestsCompleted()
    {
        var report = QuestController.Instance.GenerateMissionReport();
        
        // Добавлена проверка на null
        if (RankManager.Instance != null)
        {
            // Используй то же имя, что и в классе MissionStatus (HadMinigameErrors)
            RankManager.Instance.ProcessMissionResults(report, MissionStatus.HadMinigameErrors, false);
        }
        
        playerMap.Disable();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        DisplayVictory(report);
    }

    private void OnPlayerDeath()
    {
        if (IsDeathScreenActive) return;
        IsDeathScreenActive = true;

        var report = QuestController.Instance.GenerateMissionReport();
        
        if (RankManager.Instance != null)
        {
            // Исправлено имя на HadMinigameErrors для соответствия
            RankManager.Instance.ProcessMissionResults(report, MissionStatus.HadMinigameErrors, true);
        }

        playerMap.Disable();
        // Убрали лишний Invoke, оставили прямой вызов
        DisplayDeath(); 
    }

    private void DisplayVictory(MissionReport report)
    {
        HideAllPanels();
        victoryPanel.SetActive(true);
        Time.timeScale = 0f;

        completedText.text = $"�������: {report.CompletedQuests} / {report.TotalQuests}";
        failedText.text = $"���������: {report.FailedQuests} / {report.TotalQuests}";

        foreach (Transform child in questListContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var quest in report.Quests)
        {
            var entry = Instantiate(questEntryPrefab, questListContainer);
            entry.Initialize(quest);
        }
    }

    private void DisplayDeath()
    {
        HideAllPanels();
        deathPanel.SetActive(true);
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void HideAllPanels()
    {
        victoryPanel.SetActive(false);
        deathPanel.SetActive(false);
    }
}