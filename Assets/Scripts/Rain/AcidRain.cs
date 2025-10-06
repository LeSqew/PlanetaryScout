using UnityEngine;

public class AcidRain : MonoBehaviour
{
    [SerializeField] private float damagePerSecond = 10f;
    [SerializeField] private float damageInterval = 0.1f;
    [SerializeField] private float height = 100f;

    private float timer;
    private HealthController playerHealth;
    private bool playerInRain = false;
    private BoxCollider rainCollider;

    void Start()
    {
        // possible 
        //rainCollider = GetComponent<BoxCollider>();
        if (rainCollider == null)
        {
            rainCollider = gameObject.AddComponent<BoxCollider>();
        }

        rainCollider.isTrigger = true;
        UpdateColliderSize();
    }

    void Update()
    {
        // could be refactored
        timer += Time.deltaTime;

        if (timer >= damageInterval && playerInRain && playerHealth != null)
        {
            int damage = Mathf.RoundToInt(damagePerSecond * damageInterval);
            playerHealth.ApplyDamage(damage);
            timer = 0f;
        }
    }

    private void UpdateColliderSize()
    {
        // can be done with collider or rigidbody. Check if possible.
        Renderer planeRenderer = GetComponent<Renderer>();
        if (planeRenderer != null && rainCollider != null)
        {
            Bounds bounds = planeRenderer.bounds;
            rainCollider.size = new Vector3(
                bounds.size.x / transform.lossyScale.x,
                height,
                bounds.size.z / transform.lossyScale.z
            );
            rainCollider.center = new Vector3(0, -5f, 0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //check again
        // Проверяем, что вошел объект с HealthController (пустой объект Player)
        HealthController health = other.GetComponent<HealthController>();
        if (health != null)
        {
            playerHealth = health;
            playerInRain = true;
            return;
        }

        // Если вошла капсула, ищем HealthController у ее родителя
        if (other.GetComponent<CapsuleCollider>() != null)
        {
            health = other.GetComponentInParent<HealthController>();
            if (health != null)
            {
                playerHealth = health;
                playerInRain = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Проверяем, что вышел объект с HealthController (пустой объект Player)
        HealthController health = other.GetComponent<HealthController>();
        if (health != null && health == playerHealth)
        {
            playerInRain = false;
            return;
        }

        // Если вышла капсула, проверяем ее родителя
        if (other.GetComponent<CapsuleCollider>() != null)
        {
            health = other.GetComponentInParent<HealthController>();
            if (health != null && health == playerHealth)
            {
                playerInRain = false;
            }
        }
    }
}