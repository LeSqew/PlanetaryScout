using UnityEngine;
using System.Collections.Generic;

public class TornadoEffect : MonoBehaviour
{
    private TornadoConfig config;
    
    private List<DestructibleObject> coreDestructibleObjects = new List<DestructibleObject>();
    private List<Rigidbody> outerAffectedRigidbodies = new List<Rigidbody>();

    public void Initialize(TornadoConfig conf)
    {
        config = conf;
    }
    
    // ProcessEnter и ProcessExit остаются без изменений,
    // так как они были исправлены в предыдущем шаге.

    public void ProcessEnter(Collider other, TornadoZone.ZoneType zoneType)
    {
        // Проверяем, что вошел НЕ наш собственный коллайдер (защита от "самостолкновения")
        if (other.gameObject.GetComponentInParent<TornadoEffect>() != null) return;

        // 1. Проверяем, что это разрушаемый объект (для ЯДРА)
        if (zoneType == TornadoZone.ZoneType.Core)
        {
            DestructibleObject destructible = other.GetComponent<DestructibleObject>();
            if (destructible != null && !coreDestructibleObjects.Contains(destructible))
            {
                coreDestructibleObjects.Add(destructible);
                Debug.Log($"[TornadoEffect] Цель {other.gameObject.name} вошла в ЯДРО. (Урон будет в FixedUpdate)");
            }
        }
        
        // 2. Проверяем Rigidbody для ВНЕШНЕЙ ЗОНЫ
        else if (zoneType == TornadoZone.ZoneType.Outer)
        {
            Rigidbody rb = other.attachedRigidbody;
            // Проверяем, что это не наш собственный Rigidbody (если он есть)
            if (rb != null && !outerAffectedRigidbodies.Contains(rb))
            {
                outerAffectedRigidbodies.Add(rb);
                Debug.Log($"[TornadoEffect] Объект {other.gameObject.name} вошел во ВНЕШНЮЮ ЗОНУ.");
            }
        }
    }

    public void ProcessExit(Collider other, TornadoZone.ZoneType zoneType)
    {
        // 1. Удаление из списка разрушаемых (Ядро)
        if (zoneType == TornadoZone.ZoneType.Core)
        {
            DestructibleObject destructible = other.GetComponent<DestructibleObject>();
            if (destructible != null)
            {
                coreDestructibleObjects.Remove(destructible);
                Debug.Log($"[TornadoEffect] Цель {other.gameObject.name} покинула ЯДРО.");
            }
        }

        // 2. Удаление из списка притягиваемых (Внешняя зона)
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

    void FixedUpdate()
    {
        if (config == null) return;
        
        // Используем Time.fixedDeltaTime для равномерной физики
        float fixedDeltaTime = Time.fixedDeltaTime; 

        // 1. Воздействие на объекты во ВНЕШНЕЙ ЗОНЕ
        ApplyOuterZoneEffect(fixedDeltaTime);

        // 2. Воздействие и разрушение объектов в ЯДРЕ
        ApplyCoreZoneEffect(fixedDeltaTime);
    }

    private void ApplyOuterZoneEffect(float fixedDeltaTime)
    {
        // ИСПОЛЬЗУЕМ НОВУЮ, более слабую СИЛУ для игрока/легких объектов
        float pullForceMagnitude = config.playerPullForce; // <-- ИЗМЕНЕНИЕ
        
        for (int i = outerAffectedRigidbodies.Count - 1; i >= 0; i--)
        {
            Rigidbody rb = outerAffectedRigidbodies[i];
            
            // Если объект был уничтожен из другого источника, Rigidbody может стать null
            if (rb == null) 
            {
                outerAffectedRigidbodies.RemoveAt(i);
                continue;
            }
            
            // Нанесение легкого урона игроку (раскомментировать при наличии IPlayer)
            // rb.GetComponent<IPlayer>()?.ApplyDamage(config.lightDamagePerSecond * fixedDeltaTime);

            // ИСПРАВЛЕНИЕ: Теперь сила тянет К торнадо
            Vector3 pullDirection = (transform.position - rb.transform.position).normalized; 
            
            // 1. Сила притяжения
            Vector3 pullForce = pullDirection * pullForceMagnitude;
            
            // 2. Добавление вихревого (тангенциального) компонента (кручение)
            // Добавим вихревой компонент (половина от силы притяжения)
            Vector3 whirlForce = Vector3.Cross(pullDirection, Vector3.up).normalized * (pullForceMagnitude * 0.5f);
            
            // Применяем сумму сил
            rb.AddForce(pullForce + whirlForce, ForceMode.Force);
        }
    }

    private void ApplyCoreZoneEffect(float fixedDeltaTime)
    {
        float damageAmount = config.heavyDamagePerSecond * fixedDeltaTime;
        float pullForceMagnitude = config.destructionForce; // Используем сильную силу для разрушения
        
        for (int i = coreDestructibleObjects.Count - 1; i >= 0; i--)
        {
            DestructibleObject destructible = coreDestructibleObjects[i];
            
            // Если объект был уничтожен (Die() вызван), он будет null
            if (destructible == null)
            {
                coreDestructibleObjects.RemoveAt(i);
                continue;
            }

            // Нанесение урона разрушаемому объекту
            destructible.TakeDamage(damageAmount);
            
            // Применение физической силы (притягивание)
            Rigidbody rb = destructible.gameObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Формула притяжения (направлена К центру торнадо)
                Vector3 pullForce = (transform.position - destructible.transform.position).normalized * pullForceMagnitude;
                rb.AddForce(pullForce, ForceMode.Acceleration);
            }
            
            // *Важно:* Если Die() в DestructibleObject.cs вызывает Destroy(gameObject), 
            // то на следующем шаге FixedUpdate этот объект будет null и удалится из списка.
        }
    }
}