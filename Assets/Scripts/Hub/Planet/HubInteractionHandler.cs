using UnityEngine;
using UnityEngine.InputSystem;

public class HubTrigger : MonoBehaviour
{
    [SerializeField] private PlanetSelector selector;
    [SerializeField] private InputActionAsset inputActions;
    
    private bool _canInteract = false;
    private InputAction _interactAction;

    void Awake()
    {
        // 1. Находим карту и кнопку
        var playerMap = inputActions.FindActionMap("Player");
        _interactAction = playerMap.FindAction("Interact");
    }

    void OnEnable()
    {
        // 2. ОБЯЗАТЕЛЬНО включаем действие, иначе оно не будет реагировать
        _interactAction?.Enable();
    }

    void OnDisable()
    {
        _interactAction?.Disable();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.transform.root.CompareTag("Player")) 
        {
            selector.ShowPanel(true);
            Debug.Log("Панель выбора планет активна");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.transform.root.CompareTag("Player"))
        {
            selector.ShowPanel(false);
            Debug.Log("Панель выбора планет скрыта");
        }
    }
}