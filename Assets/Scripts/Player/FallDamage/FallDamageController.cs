using UnityEngine;

public class FallDamageController : MonoBehaviour
{
    [Header("Fall Damage Settings")]
    [SerializeField] private float damageSpeedThreshold = -15f; // Порог скорости падения, после которой наносится урон
    [SerializeField] private float damageModifier = 1f;          // Модификатор урона
    [SerializeField] private float raycastDistance = 0.2f;       // Длина луча для проверки касания земли
    [SerializeField] private LayerMask groundLayer;              // Слой, который считается землёй
    [SerializeField] private Transform raycastOrigin;            // Точка, из которой выпускается луч (указать в инспекторе!)

    private HealthController healthController; // Контроллер здоровья игрока
    private Rigidbody rb;                      // Физический компонент игрока
    private float maxFallSpeed = 0f;           // Минимальная (наибольшая по модулю) скорость падения
    private bool wasGrounded = false;          // Был ли игрок на земле в предыдущем кадре

    void Start()
    {
        // Получаем ссылки на нужные компоненты
        healthController = GetComponent<HealthController>();
        rb = GetComponent<Rigidbody>();

        // Проверка, что всё настроено
        if (healthController == null)
        {
            Debug.LogError("HealthController not found on the player!");
        }

        if (rb == null)
        {
            Debug.LogError("Rigidbody not found on the player!");
        }

        if (raycastOrigin == null)
        {
            Debug.LogError("Raycast origin is not assigned in the inspector!");
        }
    }

    void FixedUpdate()
    {
        // Точка, из которой пускаем луч вниз
        Vector3 rayStartPoint = raycastOrigin != null ? raycastOrigin.position : transform.position;

        // Проверяем, стоит ли игрок на земле
        bool isGrounded = CheckGrounded(rayStartPoint);

        // Если игрок в воздухе и падает — обновляем максимальную скорость падения
        if (!isGrounded && rb.linearVelocity.y < maxFallSpeed)
        {
            maxFallSpeed = rb.linearVelocity.y;
        }

        // Если только что коснулся земли (только что приземлился)
        if (isGrounded && !wasGrounded)
        {
            // Проверяем, превысил ли скорость падения порог
            if (maxFallSpeed <= damageSpeedThreshold)
            {
                ApplyFallDamage();
            }

            // Сбрасываем значение для следующего падения
            maxFallSpeed = 0;
        }

        // Запоминаем состояние касания земли для следующего кадра
        wasGrounded = isGrounded;
    }

    /// <summary>
    /// Проверяет, стоит ли игрок на земле, используя Raycast вниз.
    /// </summary>
    private bool CheckGrounded(Vector3 rayStart)
    {
        return Physics.Raycast(rayStart, Vector3.down, raycastDistance, groundLayer);
    }

    /// <summary>
    /// Рассчитывает и применяет урон при падении.
    /// </summary>
    private void ApplyFallDamage()
    {
        // Разница между фактической скоростью и порогом
        float speedOverThreshold = Mathf.Abs(maxFallSpeed) - Mathf.Abs(damageSpeedThreshold);

        // Если разница отрицательная — урона быть не должно
        if (speedOverThreshold <= 0)
            return;

        // Урон = превышение скорости × модификатор
        int damage = Mathf.RoundToInt(speedOverThreshold * damageModifier);

        Debug.Log($"[FallDamage] Player hit the ground! Speed: {maxFallSpeed:F2}, Damage: {damage}");

        // Применяем урон через модель здоровья
        healthController.Model.TakeDamage(damage);
        healthController.Model.OnDeath();

        // Обновляем визуальный индикатор здоровья
        healthController.healthBarView.UpdateHealthBar(
            healthController.Model.currentHealth,
            healthController.Model.maxHealth
        );
    }

    /// <summary>
    /// Рисует луч в редакторе для отладки (красная линия вниз).
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (raycastOrigin == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(raycastOrigin.position, Vector3.down * raycastDistance);
    }
}