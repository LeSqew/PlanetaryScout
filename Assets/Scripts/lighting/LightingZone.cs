using Player.Health;
using UnityEngine;

public class LightningZone : MonoBehaviour
{
    [Header("Zone Settings")]
    public bool isActive = true;

    [Header("References")]
    public LightningZoneManager manager; // ссылка на менеджер
    public Transform player;

    [Header("Debug")]
    public bool drawGizmos = true;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (player == null) player = manager.player;

        InvokeRepeating(
            nameof(TryStrikeLightning),
            manager.checkInterval,
            manager.checkInterval
        );
    }

    void TryStrikeLightning()
    {
        if (!isActive)
            return;

        if (Random.value > manager.lightningChance)
            return;

        Vector3 strikePoint = GetRandomPointInZone();
        StrikeLightning(strikePoint);
    }

    Vector3 GetRandomPointInZone()
    {
        Vector2 randomCircle = Random.insideUnitCircle * manager.zoneRadius;

        Vector3 rayStart = new Vector3(
            transform.position.x + randomCircle.x,
            transform.position.y + 50f,
            transform.position.z + randomCircle.y
        );

        if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, 100f))
            return hit.point;

        return transform.position;
    }

    void StrikeLightning(Vector3 strikePoint)
    {
        SpawnLightningFX(strikePoint);
        PlaySound(strikePoint);
        DealDamage(strikePoint);
    }

    void SpawnLightningFX(Vector3 strikePoint)
    {
        if (manager.lightningPrefab == null) return;

        Vector3 spawnPos = strikePoint + Vector3.up * manager.lightningHeightOffset;

        GameObject lightning = Instantiate(
            manager.lightningPrefab,
            spawnPos,
            Quaternion.identity
        );

        Destroy(lightning, manager.lightningLifeTime);
    }

    void PlaySound(Vector3 strikePoint)
    {
        if (manager.lightningSound == null) return;

        audioSource.transform.position = strikePoint;
        audioSource.PlayOneShot(manager.lightningSound, manager.soundVolume);
    }

    void DealDamage(Vector3 strikePoint)
    {
        float sqrDistance =
            (player.position - strikePoint).sqrMagnitude;

        if (sqrDistance <= manager.damageRadius * manager.damageRadius)
        {
            manager.healthController.takeDamage.Invoke(manager.damageAmount);
            Debug.Log("Игрок поражён молнией!");
        }
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, manager.zoneRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, manager.damageRadius);
    }
}