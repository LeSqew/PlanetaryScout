using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class BestiaryManager : MonoBehaviour
{
    [Header("Input Settings")]
    [SerializeField] private InputActionAsset inputActions; // Перетащите сюда ваш .inputactions файл

    [Header("Data")]
    [SerializeField] private List<BestiaryEntry> allEntries;

    [Header("UI References")]
    [SerializeField] private GameObject bestiaryPanel;
    [SerializeField] private Transform listContainer;
    [SerializeField] private BestiaryButton buttonPrefab;
    [SerializeField] private TextMeshProUGUI nameDisplay;
    [SerializeField] private TextMeshProUGUI descDisplay;

    private InputActionMap _playerMap;
    private InputAction _openBestiaryAction;
    private bool _isOpened = false;

    void Awake()
    {
        if (inputActions != null)
        {
            // Находим карту Player в ассете
            _playerMap = inputActions.FindActionMap("Player", true);
            // Находим конкретный экшен по имени
            _openBestiaryAction = _playerMap.FindAction("OpenBestiary", true);
        }
        else
        {
            Debug.LogError("InputActionAsset не назначен в BestiaryManager!");
        }
    }

    void OnEnable()
    {
        if (_playerMap != null)
        {
            _playerMap.Enable(); // Включаем всю карту
            _openBestiaryAction.performed += OnToggleBestiary; // Подписываемся на событие
        }
    }

    void OnDisable()
    {
        if (_playerMap != null)
        {
            _openBestiaryAction.performed -= OnToggleBestiary;
            _playerMap.Disable();
        }
    }

    void Start()
    {
        bestiaryPanel.SetActive(false);
        PopulateList();
    }

    private void OnToggleBestiary(InputAction.CallbackContext context)
    {
        // Используем вашу логику проверки экрана смерти
        if (MissionReportUI.IsDeathScreenActive) return; 

        _isOpened = !_isOpened;
        bestiaryPanel.SetActive(_isOpened);

        if (_isOpened)
        {
            PopulateList(); // Обновляем список при открытии
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void PopulateList()
    {
        Debug.Log("Попытка заполнить список");
        if (allEntries == null || allEntries.Count == 0) return;

        // Очищаем старые кнопки
        foreach (Transform child in listContainer) Destroy(child.gameObject);

        // Создаем новые кнопки на основе ассетов
        foreach (var entry in allEntries)
        {
            if (entry == null) continue;
            Debug.Log($"Создание кнопки для: {entry.objectName}");
            var btn = Instantiate(buttonPrefab, listContainer);
            btn.Setup(entry.objectName, () => ShowDetails(entry));
        }
    }

    public void ShowDetails(BestiaryEntry entry)
    {
        nameDisplay.text = entry.objectName;
        descDisplay.text = entry.description;
    }
}