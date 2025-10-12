using UnityEngine;

/// <summary>
/// Обрабатывает визуальное представление системы торнадо.
/// Подписывается на события TornadoController для отображения предупреждений, спавна и уничтожения 3D-модели.
/// </summary>
public class TornadoVisualizer : MonoBehaviour
{
    [Header("View Dependencies")]
    public TornadoController controller;
    public GameObject tornadoPrefab;
    
    private GameObject activeTornadoInstance;

    /// <summary>
    /// Подписывается на события контроллера при включении.
    /// </summary>
    void OnEnable()
    {
        if (controller == null) return;
        controller.OnTornadoWarningStarted += ShowWarningUI;
        controller.OnTornadoSpawned += SpawnTornado;
        controller.OnTornadoFaded += DestroyTornado;
    }

    /// <summary>
    /// Отписывается от событий контроллера при выключении.
    /// </summary>
    void OnDisable()
    {
        if (controller == null) return;
        controller.OnTornadoWarningStarted -= ShowWarningUI;
        controller.OnTornadoSpawned -= SpawnTornado;
        controller.OnTornadoFaded -= DestroyTornado;
    }

    /// <summary>
    /// Обработчик события для фазы предупреждения. (Заглушка для логики UI).
    /// </summary>
    /// <param name="duration">Продолжительность предупреждения.</param>
    private void ShowWarningUI(float duration)
    {
        Debug.Log($"[VIEW] Показать UI предупреждение на {duration} секунд.");
    }

    /// <summary>
    /// Обработчик события для спавна торнадо. Создает префаб и инициализирует его скрипты.
    /// </summary>
    /// <param name="position">Мировая позиция для спавна торнадо.</param>
    /// <param name="target">Трансформ, за которым торнадо должно следовать.</param>
    private void SpawnTornado(Vector3 position, Transform target)
    {
        if (tornadoPrefab == null) return;
        
        activeTornadoInstance = Instantiate(tornadoPrefab, position, Quaternion.identity);
        
        TornadoMovement movement = activeTornadoInstance.GetComponent<TornadoMovement>();
        TornadoEffect effect = activeTornadoInstance.GetComponent<TornadoEffect>();

        if (movement != null) movement.Initialize(target, controller.config);
        if (effect != null) effect.Initialize(controller.config);
        
        Debug.Log("[VIEW] 3D-объект торнадо создан и активирован.");
    }

    /// <summary>
    /// Обработчик события для уничтожения торнадо.
    /// </summary>
    private void DestroyTornado()
    {
        if (activeTornadoInstance != null)
        {
            Destroy(activeTornadoInstance, 5f); 
            activeTornadoInstance = null;
        }
    }
}
