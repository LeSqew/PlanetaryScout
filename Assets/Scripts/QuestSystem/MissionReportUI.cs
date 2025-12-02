// MissionReportUI.cs
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
        // Находим HealthController на сцене
        _healthController = FindObjectOfType<HealthController>();
        if (_healthController == null)
        {
            Debug.LogError("HealthController не найден на сцене!");
        }

        // Подписка на ивенты
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
        
        playerMap.Disable();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        DisplayVictory(report);
    }

    private void OnPlayerDeath()
    {
        if (IsDeathScreenActive) return;
        IsDeathScreenActive = true;
        Debug.Log("Сработал OnPlayerDeath в MissionReportUI");
        
        
        DisplayDeath();
        playerMap.Disable();
        Invoke(nameof(DisplayDeath), 0f);
    }

    private void DisplayVictory(MissionReport report)
    {
        HideAllPanels();
        victoryPanel.SetActive(true);
        Time.timeScale = 0f;

        completedText.text = $"Успешно: {report.CompletedQuests} / {report.TotalQuests}";
        failedText.text = $"Провалено: {report.FailedQuests} / {report.TotalQuests}";

        // Очистка старого списка
        foreach (Transform child in questListContainer)
        {
            Destroy(child.gameObject);
        }

        // Заполнение списка
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