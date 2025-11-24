using UnityEngine;
using UnityEngine.InputSystem;

public class JournalToggle : MonoBehaviour
{
    public QuestJournalUI journalUI;
    public InputActionAsset inputActions;

    private InputActionMap playerMap;

    private InputAction _toggleAction;

    void Awake()
    {
        // Находим действие в существующей карте (например, "UI")
        playerMap = inputActions.FindActionMap("Player", true);
        _toggleAction = playerMap.FindAction("ToggleJournal", true);
        _toggleAction.performed += _ => journalUI.Toggle();
    }

    void OnEnable() => _toggleAction.Enable();
    void OnDisable() => _toggleAction.Disable();

    
}