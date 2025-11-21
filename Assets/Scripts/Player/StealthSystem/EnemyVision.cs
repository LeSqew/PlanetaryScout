using UnityEngine;

/// <summary>
/// Простейшая система видимости игрока врагом.
/// Враг "видит" игрока, если тот находится в пределах радиуса видимости.
/// Радиус уменьшается вдвое, если игрок присел.
/// </summary>
public class EnemyVision : MonoBehaviour
{
    [Header("Vision Settings")]
    [Tooltip("Максимальное расстояние, на котором враг может видеть стоящего игрока.")]
    [SerializeField] private float viewDistance = 10f;

    private Transform player;
    private PlayerStealthController playerStealth;

    private void Start()
    {
        // Ищем игрока по тегу (у игрока должен быть tag = Player)
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null)
        {
            Debug.LogError("❌ Player не найден! Убедись, что объект игрока имеет тег 'Player'.");
            enabled = false;
            return;
        }

        player = playerObj.transform;
        playerStealth = playerObj.GetComponent<PlayerStealthController>();
    }

    private void Update()
    {
        if (player == null || playerStealth == null)
            return;

        float distance = Vector3.Distance(transform.position, player.position);
        float currentViewRange = playerStealth.isCrouching ? viewDistance / 2f : viewDistance;

        if (distance <= currentViewRange)
        {
            Debug.Log($"👀 {name} видит игрока (расстояние {distance:F1})");
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Визуализируем радиус видимости в редакторе
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewDistance);
    }
}