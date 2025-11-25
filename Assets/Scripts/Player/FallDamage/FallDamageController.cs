using Player.Health;
using UnityEngine;

namespace Player.FallDamage
{
    public class FallDamageController : MonoBehaviour
    {
        [Header("Fall Damage Settings")]
        [SerializeField] private float damageSpeedThreshold = -15f; // Порог скорости падения, после которой наносится урон
        [SerializeField] private float damageModifier = 1f;          // Модификатор урона
        [SerializeField] private float raycastDistance = 0.2f;       // Длина луча для проверки касания земли
        [SerializeField] private LayerMask groundLayer;              // Слой, который считается землёй
        [SerializeField] private Transform raycastOrigin;            // Точка, из которой выпускается луч (указать в инспекторе!)

        private HealthController _healthController; // Контроллер здоровья игрока
        private Rigidbody _rb;                      // Физический компонент игрока
        private float _maxFallSpeed = 0f;           // Минимальная (наибольшая по модулю) скорость падения
        private bool _wasGrounded = false;          // Был ли игрок на земле в предыдущем кадре

        void Start()
        {
            // Получаем ссылки на нужные компоненты
            _healthController = GetComponent<HealthController>();
            _rb = GetComponent<Rigidbody>();

            // Проверка, что всё настроено
            if (!_healthController)
            {
                Debug.LogError("HealthController not found on the player!");
            }

            if (!_rb)
            {
                Debug.LogError("Rigidbody not found on the player!");
            }

            if (!raycastOrigin)
            {
                Debug.LogError("Raycast origin is not assigned in the inspector!");
            }
        }

        void FixedUpdate()
        {
            // Точка, из которой пускаем луч вниз
            Vector3 rayStartPoint = raycastOrigin ? raycastOrigin.position : transform.position;

            // Проверяем, стоит ли игрок на земле
            bool isGrounded = CheckGrounded(rayStartPoint);

            // Если игрок в воздухе и падает — обновляем максимальную скорость падения
            if (!isGrounded && _rb.linearVelocity.y < _maxFallSpeed)
            {
                _maxFallSpeed = _rb.linearVelocity.y;
            }

            // Если только что коснулся земли (только что приземлился)
            if (isGrounded && !_wasGrounded)
            {
                // Проверяем, превысил ли скорость падения порог
                if (_maxFallSpeed <= damageSpeedThreshold)
                {
                    ApplyFallDamage();
                }

                // Сбрасываем значение для следующего падения
                _maxFallSpeed = 0;
            }

            // Запоминаем состояние касания земли для следующего кадра
            _wasGrounded = isGrounded;
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
            float speedOverThreshold = Mathf.Abs(_maxFallSpeed) - Mathf.Abs(damageSpeedThreshold);

            // Если разница отрицательная — урона быть не должно
            if (speedOverThreshold <= 0)
                return;

            // Урон = превышение скорости × модификатор
            int damage = Mathf.RoundToInt(speedOverThreshold * damageModifier);

            // Debug.Log($"[FallDamage] Player hit the ground! Speed: {_maxFallSpeed:F2}, Damage: {damage}");

            // Применяем урон через модель здоровья
            _healthController.takeDamage?.Invoke(damage);
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
}