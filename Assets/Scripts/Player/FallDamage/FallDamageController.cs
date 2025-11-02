using UnityEngine;

public class FallDamageController : MonoBehaviour
{
    [Header("Fall Damage Settings")]
    [SerializeField] private float damageSpeedThreshold = -15f; // Порог скорости, после которой наносим урон
    [SerializeField] private float damageModifier = 1f;          // Модификатор урона
    [SerializeField] private float raycastDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform raycastOrigin;

    private HealthController healthController;
    private Rigidbody rb;
    private float maxFallSpeed; // Самая высокая скорость падения (отрицательная)
    private bool wasGrounded;

    void Start()
    {
        healthController = GetComponent<HealthController>();
        rb = GetComponent<Rigidbody>();

        // Если луч не задан, пробуем взять коллайдер дочернего объекта
        if (raycastOrigin == null)
        {
            Collider childCollider = GetComponentInChildren<Collider>();
            if (childCollider != null)
            {
                raycastOrigin = childCollider.transform;
                Debug.Log($"Auto-assigned raycast origin to: {raycastOrigin.name}");
            }
            else
            {
                Debug.LogError("No raycast origin assigned and no child collider found!");
            }
        }
    }

    void Update()
    {
        Vector3 rayStartPoint = raycastOrigin != null ? raycastOrigin.position : transform.position;
        bool isGrounded = CheckGrounded(rayStartPoint);

        // Если игрок в воздухе и падает — отслеживаем минимальную (наибольшую по модулю) скорость
        if (!isGrounded && rb.linearVelocity.y < maxFallSpeed)
        {
            maxFallSpeed = rb.linearVelocity.y;
        }

        // При касании земли проверяем, нужно ли нанести урон
        if (isGrounded && !wasGrounded)
        {
            if (maxFallSpeed <= damageSpeedThreshold)
            {
                ApplyFallDamage();
            }

            maxFallSpeed = 0; // сброс после приземления
        }

        wasGrounded = isGrounded;
    }

    private bool CheckGrounded(Vector3 rayStart)
    {
        return Physics.Raycast(rayStart, Vector3.down, raycastDistance, groundLayer);
    }

    private void ApplyFallDamage()
    {
        float speedOverThreshold = Mathf.Abs(maxFallSpeed) - Mathf.Abs(damageSpeedThreshold);
        int damage = Mathf.RoundToInt(speedOverThreshold * damageModifier);

        Debug.Log($"Fall damage applied! Speed: {maxFallSpeed}, Damage: {damage}");

        // ✅ Обращаемся к модели напрямую, так как событие недоступно снаружи
        healthController.Model.TakeDamage(damage);
        healthController.Model.OnDeath();

        // Обновляем здоровье на HUD через HealthBarView
        healthController.healthBarView.UpdateHealthBar(
            healthController.Model.currentHealth,
            healthController.Model.maxHealth
        );
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 rayStartPoint = raycastOrigin != null ? raycastOrigin.position : transform.position;
        Gizmos.color = Color.red;
        Gizmos.DrawRay(rayStartPoint, Vector3.down * raycastDistance);
    }
}