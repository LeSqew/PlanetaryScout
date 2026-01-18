using Player.Health;
using UnityEngine;

public class LightningZoneManager : MonoBehaviour
{
    [Header("Zones")]
    public LightningZone[] zones;

    [Header("Timing")]
    public float checkInterval = 1f;
    public float switchInterval = 60f;

    [Header("Zone Settings")]
    public float zoneRadius = 20f;
    public float raycastHeight = 50f;

    [Header("Lightning Settings")]
    [Range(0f, 1f)]
    public float lightningChance = 0.1f;
    public float damageRadius = 5f;
    public int damageAmount = 20;

    [Header("Lightning Visual")]
    public GameObject lightningPrefab;
    public float lightningLifeTime = 2f;
    public float lightningHeightOffset = 10f;

    [Header("Sound")]
    public AudioClip lightningSound;
    [Range(0f, 1f)]
    public float soundVolume = 1f;

    [Header("References")]
    public Transform player;
    public HealthController healthController;

    private int currentZoneIndex = -1;

    private void Start()
    {
        DisableAllZones();
        ActivateRandomZone();

        InvokeRepeating(
            nameof(SwitchZone),
            switchInterval,
            switchInterval
        );
    }

    void SwitchZone()
    {
        ActivateRandomZone();
    }

    void ActivateRandomZone()
    {
        if (zones.Length == 0)
            return;

        if (currentZoneIndex >= 0)
            zones[currentZoneIndex].isActive = false;

        int newIndex;
        do
        {
            newIndex = Random.Range(0, zones.Length);
        }
        while (newIndex == currentZoneIndex && zones.Length > 1);

        zones[newIndex].isActive = true;
        currentZoneIndex = newIndex;
    }

    void DisableAllZones()
    {
        foreach (var zone in zones)
            zone.isActive = false;
    }
}