using System;
using System.Collections;
using UnityEngine;

public class TornadoController : MonoBehaviour
{
    public event Action<float> OnTornadoWarningStarted; // float: время до спавна
    public event Action<Vector3, Transform> OnTornadoSpawned; // Vector3: позиция, Transform: цель (игрок)
    public event Action OnTornadoFaded;
    
    [Header("Model Dependencies")]
    public TornadoConfig config;
    public Transform playerTransform; // Игрок как цель спавна

    [Header("Controller State")]
    [SerializeField] private TornadoState currentState = TornadoState.Idle;
    private Coroutine tornadoSequenceCoroutine;
    
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
    
    private void StartTornadoLoop()
    {
        if (tornadoSequenceCoroutine != null) StopCoroutine(tornadoSequenceCoroutine);
        tornadoSequenceCoroutine = StartCoroutine(TornadoCheckLoop());
        currentState = TornadoState.Idle;
    }

    private IEnumerator TornadoCheckLoop()
    {
        while (currentState == TornadoState.Idle)
        {
            yield return new WaitForSeconds(config.checkIntervalSeconds); 
            CheckForTornadoSpawn();
        }
    }
    
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
        // Если проверка не удалась, цикл просто ждет следующей итерации.
    }
    
    private IEnumerator TornadoSequence()
    {
        // 1. WARNING
        OnTornadoWarningStarted?.Invoke(config.warningDurationSeconds);
        yield return new WaitForSeconds(config.warningDurationSeconds);
        
        // 2. ACTIVE
        currentState = TornadoState.Active;
        Vector3 spawnPosition = FindSpawnPosition();
        
        // Передаем View не только позицию, но и цель для движения
        OnTornadoSpawned?.Invoke(spawnPosition, playerTransform); 
        
        float activeDuration = UnityEngine.Random.Range(config.minActiveDurationSeconds, config.maxActiveDurationSeconds);
        yield return new WaitForSeconds(activeDuration);
        
        // 3. FADING и IDLE
        currentState = TornadoState.Fading;
        OnTornadoFaded?.Invoke();
        
        yield return new WaitForSeconds(5f); // Время для анимации исчезновения в View
        
        currentState = TornadoState.Idle;
        StartTornadoLoop(); // Запуск цикла ожидания снова
    }

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
