using UnityEngine;
using System.Linq; // Для работы с compatibleTypes

public class InteractionHandler : MonoBehaviour
{
    [Header("Ссылки на Системы")]
    [SerializeField] private InventoryController inventoryController;

    [Header("Настройки Raycast")]
    public float interactDistance = 3f;
    public LayerMask interactableLayer;

    // TODO: Привяжите этот метод к вашему Input Action (например, "UseTool")
    void Update() // Используем Update только для простоты демонстрации Raycast
    {
        if (MinigameManager.IsInMinigame) return;

        if (Input.GetMouseButtonDown(0))
        {
            HandleInteraction();
        }
    }

    private void HandleInteraction()
    {
        if (!TryGetTarget(out ScannableObject targetObject)) return;

        var currentTool = inventoryController.GetCurrentTool();
        if (currentTool == null) { Debug.Log("Слот пуст."); return; }

        // 1. Проверка совместимости (Используем ваш InventoryController)
        if (!inventoryController.CanInteractWith(targetObject.category))
        {
            Debug.Log($"Инструмент '{currentTool.toolName}' не совместим.");
            return;
        }

        // 2. Проверка наличия префаба
        if (currentTool.minigamePrefab == null)
        {
            Debug.LogError($"Инструмент '{currentTool.toolName}' не имеет назначенного префаба мини-игры.");
            return;
        }

        // 3. Создание префаба и получение контроллера
        GameObject gameInstance = Instantiate(currentTool.minigamePrefab);
        IMinigameController controller = gameInstance.GetComponent<IMinigameController>();

        if (controller == null)
        {
            Debug.LogError($"Префаб '{currentTool.minigamePrefab.name}' не содержит IMinigameController.");
            Destroy(gameInstance);
            return;
        }
        MinigameManager.Instance.EnterMinigame();
        // 4. Запуск Мини-игры и передача Callback
        controller.StartAnalysis(targetObject, (success, completedTarget) =>
        {
            HandleMinigameResult(success, completedTarget, controller);
        });
    }

    // Этот метод вызывается контроллером мини-игры по завершении
    private void HandleMinigameResult(bool success, ScannableObject completedTarget, IMinigameController controller)
    {
        MinigameManager.Instance.ExitMinigame(); // Выход из глобального состояния

        if (success)
        {
            // Система квестов ловит это событие
            completedTarget.OnScanCompleted();
            // 💡 ВАЖНО: ScannableObject.OnScanCompleted() уже вызывает Destroy(gameObject)
        }
        else
        {
            Debug.Log("Мини-игра провалена.");
        }

        // 5. Очистка (уничтожение префаба мини-игры)
        controller.Cleanup();
    }

    // Вспомогательный метод (Рейкаст)
    private bool TryGetTarget(out ScannableObject obj)
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactableLayer))
        {
            obj = hit.collider.GetComponent<ScannableObject>();
            return obj != null;
        }

        obj = null;
        return false;
    }
}