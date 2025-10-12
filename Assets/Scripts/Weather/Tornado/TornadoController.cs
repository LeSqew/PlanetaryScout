using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Центральный контроллер, управляющий полным жизненным циклом торнадо:
/// проверка вероятности, предупреждение, спавн, активная фаза и исчезновение.
/// </summary>
public class TornadoController : MonoBehaviour
{
    public event Action<float> OnTornadoWarningStarted; 
    public event Action<Vector3, Transform> OnTornadoSpawned; 
    public event Action OnTornadoFaded;
    
    [Header("Model Dependencies")]
    public TornadoConfig config;
    public Transform playerTransform; 

    [Header("Controller State")]
    [SerializeField] private TornadoState currentState = TornadoState.Idle;
    private Coroutine tornadoSequenceCoroutine;
    
    /// <summary>
    /// Инициализирует контроллер и запускает основной цикл проверки появления торнадо.
    /// </summary>
    void Start()
    {
        Debug.Log(">>> TornadoController: Метод Start() вызван."); 
        if (config == null || playerTransform == null)
        {
            Debug.LogError("Controller не настроен: назначьте Config и Player Transform!");
            enabled = false;
            return;
        }
        StartTornadoLoop();
    }
    
    /// <summary>
    /// Останавливает любую текущую последовательность и запускает корутину TornadoCheckLoop.
    /// </summary>
    private void StartTornadoLoop()
    {
        if (tornadoSequenceCoroutine != null) StopCoroutine(tornadoSequenceCoroutine);
        tornadoSequenceCoroutine = StartCoroutine(TornadoCheckLoop());
        currentState = TornadoState.Idle;
    }

    /// <summary>
    /// Основной непрерывный цикл, который ждет интервал проверки и вызывает CheckForTornadoSpawn.
    /// </summary>
    private IEnumerator TornadoCheckLoop()
    {
        while (currentState == TornadoState.Idle)
        {
            yield return new WaitForSeconds(config.checkIntervalSeconds); 
            CheckForTornadoSpawn();
        }
    }
    
    /// <summary>
    /// Использует вероятность из конфига для случайного определения, должно ли появиться торнадо.
    /// При успехе запускает TornadoSequence.
    /// </summary>
    private void CheckForTornadoSpawn()
    {
        float chance = config.constantTornadoChance;
        float roll = UnityEngine.Random.Range(0f, 100f);
        
        if (roll <= chance)
        {
            Debug.Log($"[Controller] Проверка успешна ({chance:F1}%). Состояние: Warning.");
            currentState = TornadoState.Warning;
            StartCoroutine(TornadoSequence());
        }
    }
    
    /// <summary>
    /// Управляет последовательными фазами жизни торнадо (Warning -> Active -> Fading).
    /// </summary>
    private IEnumerator TornadoSequence()
    {
        // 1. WARNING
        OnTornadoWarningStarted?.Invoke(config.warningDurationSeconds);
        yield return new WaitForSeconds(config.warningDurationSeconds);
        
        // 2. ACTIVE
        currentState = TornadoState.Active;
        Vector3 spawnPosition = FindSpawnPosition();
        
        OnTornadoSpawned?.Invoke(spawnPosition, playerTransform); 
        
        float activeDuration = UnityEngine.Random.Range(config.minActiveDurationSeconds, config.maxActiveDurationSeconds);
        yield return new WaitForSeconds(activeDuration);
        
        // 3. FADING и IDLE
        currentState = TornadoState.Fading;
        OnTornadoFaded?.Invoke();
        
        yield return new WaitForSeconds(5f); 
        
        currentState = TornadoState.Idle;
        StartTornadoLoop(); 
    }

    /// <summary>
    /// Рассчитывает случайную позицию для спавна торнадо вокруг игрока в заданном радиусе.
    /// </summary>
    private Vector3 FindSpawnPosition()
    {
        Vector3 playerPos = playerTransform.position;
        float distance = UnityEngine.Random.Range(config.minSpawnRadius, config.maxSpawnRadius);
        float angle = UnityEngine.Random.Range(0f, 360f);
        
        Vector3 spawnOffset = new Vector3(
            Mathf.Cos(angle * Mathf.Deg2Rad) * distance,
            0,
            Mathf.Sin(angle * Mathf.Deg2Rad) * distance
        );
        return playerPos + spawnOffset;
    }
}
