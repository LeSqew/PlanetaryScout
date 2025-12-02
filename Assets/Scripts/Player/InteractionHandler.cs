using Player.InventorySystem;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionHandler : MonoBehaviour
{
    [Header("Ссылки на Системы")]
    [SerializeField] private InventoryController inventoryController;

    [SerializeField] private InputActionAsset inputActions;

    [Header("Настройки Raycast")]
    public float interactDistance = 3f;
    // public LayerMask interactableLayer;

    [SerializeField] private InteractionHintUI interactionHint;
    
    private GameObject gameInstance;
    private ToolData currentTool;
    private InputAction _interactAction;

    void Awake()
    {
        // Получаем InputAction из ссылки
        if (inputActions != null)
        {
            _interactAction = inputActions.FindActionMap("Player", true).FindAction("Interact", true);
            _interactAction.performed += OnInteractPerformed;
        }
    }

    void OnEnable()
    {
        _interactAction?.Enable();
    }

    void OnDisable()
    {
        _interactAction?.Disable();
    }

    void OnDestroy()
    {
        if (_interactAction != null)
        {
            _interactAction.performed -= OnInteractPerformed;
        }
    }

    void Update()
    {
        if (MinigameManager.IsInMinigame)
        {
            interactionHint.Hide();
            return;
        }

        HandleHover();
    }
    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        HandleInteraction();
    }

    // 🔍 Новое: обработка наведения
    private void HandleHover()
    {
        if (TryGetTarget(out ScannableObject targetObject))
        {
            currentTool = inventoryController.GetCurrentTool();
            if (currentTool != null && inventoryController.CanInteractWith(targetObject.category))
            {
                string actionName = GetActionName(currentTool.toolName);
                interactionHint.Show($"Исследовать [{actionName}]", targetObject.transform.position);
                return;
            }
        }

        // Если нет цели или несовместимо — скрываем
        interactionHint.Hide();
    }

    private void HandleInteraction()
    {
        if (!TryGetTarget(out ScannableObject targetObject)) return;

        currentTool = inventoryController.GetCurrentTool();
        if (currentTool == null)
        {
            Debug.Log("Слот пуст.");
            return;
        }

        // Проверка совместимости
        if (!inventoryController.CanInteractWith(targetObject.category))
        {
            Debug.Log($"Инструмент '{currentTool.toolName}' не совместим.");
            return;
        }

        // Проверка префаба
        if (currentTool.minigamePrefab == null)
        {
            Debug.LogError($"Инструмент '{currentTool.toolName}' не имеет префаба мини-игры.");
            return;
        }

        // Уничтожаем предыдущую миниигру (страховка)
        if (gameInstance != null) Destroy(gameInstance);

        // Создаём новую
        gameInstance = Instantiate(currentTool.minigamePrefab);
        var controller = gameInstance.GetComponent<IMinigameController>();

        if (controller == null)
        {
            Debug.LogError($"Префаб '{currentTool.minigamePrefab.name}' не содержит IMinigameController.");
            Destroy(gameInstance);
            return;
        }

        if (controller.RequiresInputBlocking)
        {
            MinigameManager.Instance.EnterMinigame();
        }

        controller.StartAnalysis(targetObject, (success, target) =>
        {
            HandleMinigameResult(success, target, controller, controller.RequiresInputBlocking);
        });
    }
    
    string GetActionName(string toolName)
    {
        return "ЛКМ";
    }

    private void HandleMinigameResult(bool success, ScannableObject target, IMinigameController controller, bool wasInputBlocked)
    {
        Debug.Log($"📥 HandleMinigameResult: success={success}, target={target?.category}");
        
        if (wasInputBlocked)
        {
            MinigameManager.Instance.ExitMinigame();
        }

        if (success)
        {
            target.OnScanCompleted();
            ObjectRegistry.Instance?.UnregisterObject(target);
        
            MinigameReportUI.Instance?.ShowSuccessReport(target);
        }
        else
        {
            string failureReason = "Сканирование не удалось";
            MinigameReportUI.Instance?.ShowFailureReport(target, failureReason);
            if (currentTool?.destroyObjectOnFailure == true)
            {
                
                ObjectRegistry.Instance?.UnregisterObject(target);
                DataCollectionEvents.RaiseObjectDestroyed(target.category);
                Destroy(target.gameObject);
            }
            
        }

        controller.Cleanup();
        currentTool = null;
    }

    private bool TryGetTarget(out ScannableObject obj)
    {
        // Получаем актуальный LayerMask из реестра
        LayerMask scanLayerMask = ObjectRegistry.Instance?.GetScannableLayerMask() ?? 0;

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, scanLayerMask))
        {
            obj = hit.collider.GetComponent<ScannableObject>();
            return obj != null && obj.CanBeInteractedWith;
        }

        obj = null;
        return false;
    }
}