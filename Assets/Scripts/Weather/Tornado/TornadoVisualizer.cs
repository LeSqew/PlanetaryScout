using UnityEngine;

public class TornadoVisualizer : MonoBehaviour
{
    [Header("View Dependencies")]
    public TornadoController controller;
    public GameObject tornadoPrefab;
    
    private GameObject activeTornadoInstance;

    void OnEnable()
    {
        if (controller == null) return;
        // Подписка на события Контроллера
        controller.OnTornadoWarningStarted += ShowWarningUI;
        controller.OnTornadoSpawned += SpawnTornado;
        controller.OnTornadoFaded += DestroyTornado;
    }

    void OnDisable()
    {
        if (controller == null) return;
        controller.OnTornadoWarningStarted -= ShowWarningUI;
        controller.OnTornadoSpawned -= SpawnTornado;
        controller.OnTornadoFaded -= DestroyTornado;
    }

    private void ShowWarningUI(float duration)
    {
        // Активация UI предупреждения (например, красная рамка + таймер)
        Debug.Log($"[VIEW] Показать UI предупреждение на {duration} секунд.");
    }

    private void SpawnTornado(Vector3 position, Transform target)
    {
        if (tornadoPrefab == null) return;
        
        activeTornadoInstance = Instantiate(tornadoPrefab, position, Quaternion.identity);
        
        // Инициализация логики движения и эффектов на префабе
        TornadoMovement movement = activeTornadoInstance.GetComponent<TornadoMovement>();
        TornadoEffect effect = activeTornadoInstance.GetComponent<TornadoEffect>();

        if (movement != null) movement.Initialize(target, controller.config);
        if (effect != null) effect.Initialize(controller.config);
        
        Debug.Log("[VIEW] 3D-объект торнадо создан и активирован.");
    }

    private void DestroyTornado()
    {
        // Запуск анимации исчезновения и уничтожение
        if (activeTornadoInstance != null)
        {
            // Здесь может быть вызов метода FadeOut() на префабе
            Destroy(activeTornadoInstance, 5f); 
            activeTornadoInstance = null;
        }
    }
}
