using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class MinigameManager : MonoBehaviour
{
    public static MinigameManager Instance { get; private set; }

    [Header("UI")]
    public QuestJournalUI questJournal;

    [Header("Input")]
    public InputActionAsset inputActions;
    private InputActionMap _playerMap;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        _playerMap = inputActions.FindActionMap("Player", true);
    }

    /// <summary>
    /// Вызывается в начале любой мини-игры
    /// </summary>
    public void EnterMinigame()
    {
        // Скрыть журнал заданий
        questJournal.gameObject.SetActive(false);;

        // Отключить управление игрока
        _playerMap.Disable();

        // Освободить курсор
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /// <summary>
    /// Вызывается при завершении любой мини-игры
    /// </summary>
    public void ExitMinigame()
    {
        // Показать журнал заданий
        questJournal.gameObject.SetActive(true);

        // Включить управление
        _playerMap.Enable();

        // Захватить курсор
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}