using UnityEngine;
using System.Linq;

public class InteractionHandler : MonoBehaviour
{
    [Header("Ссылки на Системы")]
    [SerializeField] private InventoryController inventoryController;

    [Header("Настройки Raycast")]
    public float interactDistance = 3f;
    public LayerMask interactableLayer;

    [SerializeField] private InteractionHintUI interactionHint;
    
    private GameObject gameInstance;
    private ToolData currentTool;

    void Update()
    {
        if (MinigameManager.IsInMinigame)
        {
            interactionHint.Hide();
            return;
        }

        // 🔥 Проверка наведения КАЖДЫЙ КАДР
        HandleHover();

        // Запуск взаимодействия по клику
        if (Input.GetMouseButtonDown(0))
        {
            HandleInteraction();
        }
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
            target.DisableInteraction();
        }
        else
        {
            if (currentTool?.destroyObjectOnFailure == true)
            {
                int remaining = FindObjectsOfType<ScannableObject>()
                    .Count(obj => obj.category == target.category && obj != target);
            
                DataCollectionEvents.RaiseObjectDestroyed(target.category, remaining);
                Destroy(target.gameObject);
            }
        }

        controller.Cleanup();
        currentTool = null;
    }

    private bool TryGetTarget(out ScannableObject obj)
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactableLayer))
        {
            obj = hit.collider.GetComponent<ScannableObject>();
            return obj != null && hit.collider.enabled;
        }

        obj = null;
        return false;
    }
}