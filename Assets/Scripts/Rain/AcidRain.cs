using Player.Health;
using UnityEngine;

public class AcidRain : MonoBehaviour
{
    [Header("Rain Settings")]
    [SerializeField] private ParticleSystem rainParticles;
    [SerializeField] private float damagePerSecond = 10f;
    [SerializeField] private float damageInterval = 0.1f;
    [SerializeField] private float height = 100f;

    private float timer;
    private HealthController playerHealth;
    private bool playerInRain = false;
    private BoxCollider rainCollider;

    void Start()
    {
        SetupRainSystem();
    }

    private void SetupRainSystem()
    {
        // Создаем коллайдер для определения попадания в дождь
        if (rainCollider == null)
        {
            rainCollider = gameObject.AddComponent<BoxCollider>();
        }
        rainCollider.isTrigger = true;
        rainCollider.enabled = true;
        
        UpdateColliderSize();

        // Активируем частицы если они есть
        if (rainParticles != null)
        {
            var main = rainParticles.main;
            main.loop = true;
            rainParticles.Play();
        }
    }

    void Update()
    {
        if (playerInRain && playerHealth != null)
        {
            timer += Time.deltaTime;

            if (timer >= damageInterval)
            {
                int damage = Mathf.RoundToInt(damagePerSecond * damageInterval);
                playerHealth.takeDamage?.Invoke(damage);
                timer = 0f;
            }
        }
    }

    private void UpdateColliderSize()
    {
        // Если у нас есть меш-рендерер (плоскость дождя)
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null && rainCollider != null)
        {
            Bounds bounds = meshRenderer.bounds;
            rainCollider.size = new Vector3(
                bounds.size.x / transform.lossyScale.x,
                height,
                bounds.size.z / transform.lossyScale.z
            );
            rainCollider.center = new Vector3(0f, height / 2f, 0f); // Центрируем по высоте
        }
        else
        {
            // Резервный вариант - устанавливаем размер вручную
            rainCollider.size = new Vector3(50f, height, 50f);
            rainCollider.center = new Vector3(0f, height / 2f, 0f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Проверяем наличие HealthController
        HealthController health = GetHealthController(other);
        if (health != null)
        {
            playerHealth = health;
            playerInRain = true;
            Debug.Log("Игрок попал под кислотный дождь!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        HealthController health = GetHealthController(other);
        if (health != null && health == playerHealth)
        {
            playerInRain = false;
            Debug.Log("Игрок покинул зону кислотного дождя");
        }
    }

    private HealthController GetHealthController(Collider other)
    {
        // Прямая проверка
        HealthController health = other.GetComponent<HealthController>();
        if (health != null) return health;

        // Проверка родителя (если объект - дочерний)
        health = other.GetComponentInParent<HealthController>();
        if (health != null) return health;

        // Поиск в дочерних (если HealthController на самом игроке)
        health = other.transform.GetChild(0)?.GetComponent<HealthController>();
        
        return health;
    }

    // Метод для ручного включения/выключения дождя
    public void SetRainActive(bool isActive)
    {
        rainCollider.enabled = isActive;
        if (rainParticles != null)
        {
            if (isActive)
                rainParticles.Play();
            else
                rainParticles.Stop();
        }
        
        if (!isActive)
        {
            playerInRain = false;
        }
    }

    // Для отладки
    private void OnValidate()
    {
        if (Application.isPlaying && rainCollider != null)
        {
            UpdateColliderSize();
        }
    }
}