using UnityEngine;

/// <summary>
/// Управляет перемещением и вращением объекта торнадо в мире.
/// Реализует механику преследования с минимальной дистанцией, чтобы цель могла сбежать.
/// </summary>
public class TornadoMovement : MonoBehaviour
{
    private Transform target;
    private TornadoConfig config;

    [Header("Natural movement tweaks")]
    public float wanderAmplitude = 30f;
    public float wanderFrequency = 0.4f;
    public float maxChaseDistance = 2000f;

    private float wanderTimer;

    /// <summary>
    /// Инициализирует скрипт движения целью и конфигурацией.
    /// </summary>
    /// <param name="mainTarget">Трансформ для следования (например, игрок).</param>
    /// <param name="conf">ScriptableObject с конфигурацией TornadoConfig.</param>
    public void Initialize(Transform mainTarget, TornadoConfig conf)
    {
        target = mainTarget;
        config = conf;
        transform.position = new Vector3(transform.position.x, 5f, transform.position.z);
    }

    /// <summary>
    /// Выполняется в физическом цикле (FixedUpdate). Рассчитывает направление движения на основе цели
    /// и применяет движение и вращение.
    /// </summary>
    void FixedUpdate()
    {
        if (config == null) return;

        Vector3 moveDir;
        float distanceToTarget = target != null ? Vector3.Distance(transform.position, target.position) : float.MaxValue;

        // УСЛОВИЕ ПРЕСЛЕДОВАНИЯ:
        // 1. Цель существует И
        // 2. Цель находится в пределах максимальной дистанции И
        // 3. Цель НЕ находится слишком близко (minChaseDistance) -> позволяет сбежать
        if (target != null && distanceToTarget < maxChaseDistance && distanceToTarget > config.minChaseDistance) 
        {
            Vector3 toTarget = (target.position - transform.position).normalized;
            toTarget.y = 0;

            wanderTimer += Time.fixedDeltaTime * wanderFrequency;
            Vector3 right = Quaternion.Euler(0, 90, 0) * toTarget;
            Vector3 drift = right * (Mathf.Sin(wanderTimer) * (wanderAmplitude / 100f));

            moveDir = (toTarget + drift).normalized;
        }
        else
        {
            moveDir = new Vector3(Mathf.Sin(Time.time * 0.2f), 0, Mathf.Cos(Time.time * 0.2f)).normalized;
        }
        
        transform.position += moveDir * (config.moveSpeed * Time.fixedDeltaTime);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(moveDir),
            Time.fixedDeltaTime * 2f
        );

        transform.Rotate(Vector3.up, config.rotationSpeed * Time.fixedDeltaTime, Space.Self);
    }
}