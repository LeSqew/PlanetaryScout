using UnityEngine;

public class AcidRainSpawner : MonoBehaviour
{
    [Header("Rain Settings")]
    [SerializeField] private GameObject acidRainPrefab;

    [Header("Path Points")]
    [SerializeField] private Transform startPoint; // Перетащите объект в инспектор
    [SerializeField] private Transform endPoint;   // Перетащите объект в инспектор
    [SerializeField] private float spawnInterval = 10f;

    private void Start()
    {
        if (startPoint == null || endPoint == null)
        {
            Debug.LogError("AcidRainSpawner: Укажите startPoint и endPoint в инспекторе!");
            enabled = false;
            return;
        }

        InvokeRepeating(nameof(SpawnRain), 0f, spawnInterval);
    }

    private void SpawnRain()
    {
        GameObject rainObj = Instantiate(acidRainPrefab, startPoint.position, Quaternion.identity);
        AcidRainManager manager = rainObj.GetComponent<AcidRainManager>();
        if (manager != null)
        {
            // Передаем точки через Transform
            manager.SetPath(startPoint.position, endPoint.position);
        }
    }

    // Визуализация линии в редакторе (для удобства)
    private void OnDrawGizmosSelected()
    {
        if (startPoint != null && endPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(startPoint.position, endPoint.position);
            Gizmos.DrawSphere(startPoint.position, 0.5f);
            Gizmos.DrawSphere(endPoint.position, 0.5f);
        }
    }
}