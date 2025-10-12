using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Управляет применением физических сил, притяжением и уроном к объектам,
/// основываясь на том, в какой зоне (Ядро или Внешняя) они находятся.
/// </summary>
public class TornadoEffect : MonoBehaviour
{
    private TornadoConfig config;
    
    private List<DestructibleObject> coreDestructibleObjects = new List<DestructibleObject>();
    private List<Rigidbody> outerAffectedRigidbodies = new List<Rigidbody>();

    /// <summary>
    /// Инициализирует скрипт глобальной конфигурацией.
    /// </summary>
    /// <param name="conf">ScriptableObject с конфигурацией TornadoConfig.</param>
    public void Initialize(TornadoConfig conf)
    {
        config = conf;
    }
    
    /// <summary>
    /// Обрабатывает вход объекта в коллайдер зоны. Добавляет объект
    /// в соответствующий список (Ядро для разрушения, Внешняя для притяжения).
    /// </summary>
    /// <param name="other">Коллайдер вошедшего объекта.</param>
    /// <param name="zoneType">Тип зоны (Ядро или Внешняя), в которую вошел объект.</param>
    public void ProcessEnter(Collider other, TornadoZone.ZoneType zoneType)
    {
        if (other.gameObject.GetComponentInParent<TornadoEffect>() != null) return;
        
        if (zoneType == TornadoZone.ZoneType.Core)
        {
            DestructibleObject destructible = other.GetComponent<DestructibleObject>();
            if (destructible != null && !coreDestructibleObjects.Contains(destructible))
            {
                coreDestructibleObjects.Add(destructible);
                Debug.Log($"[TornadoEffect] Цель {other.gameObject.name} вошла в ЯДРО. (Урон будет в FixedUpdate)");
            }
        }
        else if (zoneType == TornadoZone.ZoneType.Outer)
        {
            Rigidbody rb = other.attachedRigidbody;
            if (rb != null && !outerAffectedRigidbodies.Contains(rb))
            {
                outerAffectedRigidbodies.Add(rb);
                Debug.Log($"[TornadoEffect] Объект {other.gameObject.name} вошел во ВНЕШНЮЮ ЗОНУ.");
            }
        }
    }

    /// <summary>
    /// Обрабатывает выход объекта из коллайдера зоны. Удаляет объект
    /// из соответствующего списка.
    /// </summary>
    /// <param name="other">Коллайдер вышедшего объекта.</param>
    /// <param name="zoneType">Тип зоны (Ядро или Внешняя), из которой вышел объект.</param>
    public void ProcessExit(Collider other, TornadoZone.ZoneType zoneType)
    {
        if (zoneType == TornadoZone.ZoneType.Core)
        {
            DestructibleObject destructible = other.GetComponent<DestructibleObject>();
            if (destructible != null)
            {
                coreDestructibleObjects.Remove(destructible);
                Debug.Log($"[TornadoEffect] Цель {other.gameObject.name} покинула ЯДРО.");
            }
        }
        else if (zoneType == TornadoZone.ZoneType.Outer)
        {
            Rigidbody rb = other.attachedRigidbody;
            if (rb != null)
            {
                outerAffectedRigidbodies.Remove(rb);
                Debug.Log($"[TornadoEffect] Объект {other.gameObject.name} покинул ВНЕШНЮЮ ЗОНУ.");
            }
        }
    }

    /// <summary>
    /// Выполняется в физическом цикле (FixedUpdate). Применяет непрерывные силы и урон.
    /// </summary>
    void FixedUpdate()
    {
        if (config == null) return;
        
        float fixedDeltaTime = Time.fixedDeltaTime; 
        ApplyOuterZoneEffect(fixedDeltaTime);
        ApplyCoreZoneEffect(fixedDeltaTime);
    }

    /// <summary>
    /// Применяет более слабую силу притяжения и вихревой эффект к объектам во внешней зоне.
    /// Использует 'playerPullForce' для баланса игрока/легких объектов.
    /// </summary>
    private void ApplyOuterZoneEffect(float fixedDeltaTime)
    {
        float pullForceMagnitude = config.playerPullForce; 
        
        for (int i = outerAffectedRigidbodies.Count - 1; i >= 0; i--)
        {
            Rigidbody rb = outerAffectedRigidbodies[i];

            if (rb == null) 
            {
                outerAffectedRigidbodies.RemoveAt(i);
                continue;
            }
            
            Vector3 pullDirection = (transform.position - rb.transform.position).normalized; 
            
            Vector3 pullForce = pullDirection * pullForceMagnitude;
            
            Vector3 whirlForce = Vector3.Cross(pullDirection, Vector3.up).normalized * (pullForceMagnitude * 0.5f);

            rb.AddForce(pullForce + whirlForce, ForceMode.Force);
        }
    }

    /// <summary>
    /// Применяет сильный урон и сильную силу притяжения к разрушаемым объектам в ядре.
    /// </summary>
    private void ApplyCoreZoneEffect(float fixedDeltaTime)
    {
        float damageAmount = config.heavyDamagePerSecond * fixedDeltaTime;
        float pullForceMagnitude = config.destructionForce; 
        
        for (int i = coreDestructibleObjects.Count - 1; i >= 0; i--)
        {
            DestructibleObject destructible = coreDestructibleObjects[i];
            
            if (destructible == null)
            {
                coreDestructibleObjects.RemoveAt(i);
                continue;
            }

            destructible.TakeDamage(damageAmount);

            Rigidbody rb = destructible.gameObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 pullForce = (transform.position - destructible.transform.position).normalized * pullForceMagnitude;
                rb.AddForce(pullForce, ForceMode.Acceleration);
            }
            
        }
    }
}