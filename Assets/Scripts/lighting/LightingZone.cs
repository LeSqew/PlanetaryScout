using Player.Health;
using UnityEngine;

public class LightningZone : MonoBehaviour
{
    [Header("Zone Settings")]
    public float zoneRadius = 20f;

    [Header("Lightning Settings")]
    [Range(0f, 1f)]
    public float lightningChancePerSecond = 0.1f;
    public float damageRadius = 5f;
    public int damageAmount = 20;

    [Header("Lightning Visual")]
    public GameObject lightningPrefab;
    public float lightningLifeTime = 2f;
    public float lightningHeightOffset = 10f;

    [Header("References")]
    public Transform player;
    public HealthController healthController;

    [Header("Debug")]
    public bool drawGizmos = true;

    private void Start()
    {
        InvokeRepeating(nameof(TryStrikeLightning), 1f, 1f);
    }

    void TryStrikeLightning()
    {
        if (Random.value > lightningChancePerSecond)
            return;

        Vector3 strikePoint = GetRandomPointInZone();
        StrikeLightning(strikePoint);
    }

    Vector3 GetRandomPointInZone()
    {
        Vector2 randomCircle = Random.insideUnitCircle * zoneRadius;

        Vector3 startPoint = new Vector3(
            transform.position.x + randomCircle.x,
            transform.position.y + 50f,
            transform.position.z + randomCircle.y
        );

        // Raycast вниз, чтобы попасть в землю
        if (Physics.Raycast(startPoint, Vector3.down, out RaycastHit hit, 100f))
        {
            return hit.point;
        }

        return transform.position;
    }

    void StrikeLightning(Vector3 strikePoint)
    {
        SpawnLightningFX(strikePoint);
        DealDamage(strikePoint);
    }

    void SpawnLightningFX(Vector3 strikePoint)
    {
        if (lightningPrefab == null) return;

        Vector3 spawnPos = strikePoint + Vector3.up * lightningHeightOffset;

        GameObject lightning = Instantiate(
            lightningPrefab,
            spawnPos,
            Quaternion.identity
        );

        Destroy(lightning, lightningLifeTime);
    }

    void DealDamage(Vector3 strikePoint)
    {
        float sqrDistance =
            (player.position - strikePoint).sqrMagnitude;

        if (sqrDistance <= damageRadius * damageRadius)
        {
            healthController.takeDamage.Invoke(damageAmount);
            Debug.Log("Игрок поражён молнией!");
        }
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, zoneRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
    }
}