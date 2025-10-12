using UnityEngine;

public class TornadoEffect : MonoBehaviour
{
    [Header("Настройте коллайдеры на префабе")]
    public string outerZoneTag = "Tornado_Outer"; // Тег для внешней зоны (Light Damage)
    public string coreZoneTag = "Tornado_Core";   // Тег для ядра (Destruction)
    
    private TornadoConfig config;

    public void Initialize(TornadoConfig conf)
    {
        config = conf;
    }

    void OnTriggerStay(Collider other)
    {
        if (config == null) return;
        
        // Предполагаем, что у вас есть интерфейсы или компоненты для урона
        
        // 1. Внешняя Зона: Легкий урон и отбрасывание
        if (other.CompareTag(outerZoneTag))
        {
            // other.GetComponent<IPlayer>()?.ApplyDamage(config.lightDamagePerSecond * Time.deltaTime);
            
            Rigidbody rb = other.attachedRigidbody;
            if (rb != null)
            {
                // Применяем слабую силу
                Vector3 force = (other.transform.position - transform.position).normalized * config.destructionForce * 0.1f;
                rb.AddForce(force, ForceMode.Force);
            }
        }
        
        // 2. Ядро: Критический урон и разрушение
        if (other.CompareTag(coreZoneTag))
        {
            // other.GetComponent<IPlayer>()?.ApplyDamage(config.heavyDamagePerSecond * Time.deltaTime);

            DestructibleObject destructible = other.GetComponent<DestructibleObject>();
            if (destructible != null)
            {
                // Применяем сильную разрушительную силу
                destructible.TakeDamage(config.destructionForce * Time.deltaTime);
                
                Rigidbody rb = other.attachedRigidbody;
                if (rb != null)
                {
                    // Притягиваем/отбрасываем объект
                    Vector3 pullForce = (transform.position - other.transform.position).normalized * config.destructionForce;
                    rb.AddForce(pullForce, ForceMode.Acceleration);
                }
            }
        }
    }
}
