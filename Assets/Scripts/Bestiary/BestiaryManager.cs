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
    [SerializeField] private TextMeshProUGUI rarDisplay;

    public static bool IsBestiaryOpen { get; private set; }
    private InputActionMap _playerMap;
    private InputActionMap _uiMap;
    private InputAction _openBestiaryAction;
    private bool _isOpened = false;

    void Awake()
    {
        if (inputActions != null)
        {
            _uiMap = inputActions.FindActionMap("UI", true);
            _playerMap = inputActions.FindActionMap("Player", true);
            _openBestiaryAction = _uiMap.FindAction("OpenBestiary", true);
        }
        else
        {
            Debug.LogError("InputActionAsset не назначен в BestiaryManager!");
        }
    }

    void OnEnable()
    {
        _openBestiaryAction.Enable();
        _openBestiaryAction.performed += OnToggleBestiary;
    }

    void OnDisable()
    {
        _openBestiaryAction.performed -= OnToggleBestiary;
    }

    void Start()
    {
        bestiaryPanel.SetActive(false);
        PopulateList();
    }

    private void OnToggleBestiary(InputAction.CallbackContext context)
    {
        // Не открываем, если игрок мертв или уже стоит пауза
        if (MissionReportUI.IsDeathScreenActive) return; 

        IsBestiaryOpen = !IsBestiaryOpen;
        bestiaryPanel.SetActive(IsBestiaryOpen);

        if (IsBestiaryOpen)
        {
            PopulateList();
            _playerMap.Disable(); // Полностью выключаем игрока (движение/камера)
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            _playerMap.Enable(); // Включаем обратно
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
        rarDisplay.text = entry.rarity;
    }
}