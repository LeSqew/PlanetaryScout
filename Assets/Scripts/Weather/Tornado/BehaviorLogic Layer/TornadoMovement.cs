using UnityEngine;

public class TornadoMovement : MonoBehaviour
{
    private Transform target;
    private TornadoConfig config;

    [Header("Natural movement tweaks")]
    public float wanderAmplitude = 30f;
    public float wanderFrequency = 0.4f;
    public float maxChaseDistance = 2000f;

    private float wanderTimer;

    public void Initialize(Transform mainTarget, TornadoConfig conf)
    {
        target = mainTarget;
        config = conf;
        transform.position = new Vector3(transform.position.x, 5f, transform.position.z);
    }

    void FixedUpdate()
    {
        if (config == null) return;

        Vector3 moveDir;
        // Расчет дистанции один раз для оптимизации
        float distanceToTarget = target != null ? Vector3.Distance(transform.position, target.position) : float.MaxValue;

        // УСЛОВИЕ ПРЕСЛЕДОВАНИЯ:
        // 1. Цель существует И 
        // 2. Цель находится в пределах максимальной дистанции И
        // 3. Цель НЕ находится слишком близко (minChaseDistance)
        if (target != null && distanceToTarget < maxChaseDistance && distanceToTarget > config.minChaseDistance) // <-- ИЗМЕНЕНИЕ
        {
            Vector3 toTarget = (target.position - transform.position).normalized;
            toTarget.y = 0;

            // дрейф ветра
            wanderTimer += Time.fixedDeltaTime * wanderFrequency;
            Vector3 right = Quaternion.Euler(0, 90, 0) * toTarget;
            Vector3 drift = right * (Mathf.Sin(wanderTimer) * (wanderAmplitude / 100f));

            moveDir = (toTarget + drift).normalized;
        }
        else
        {
            // Свободное блуждание, если цель слишком близко, слишком далеко или отсутствует.
            moveDir = new Vector3(Mathf.Sin(Time.time * 0.2f), 0, Mathf.Cos(Time.time * 0.2f)).normalized;
        }

        // движение и поворот
        transform.position += moveDir * (config.moveSpeed * Time.fixedDeltaTime);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(moveDir),
            Time.fixedDeltaTime * 2f
        );

        transform.Rotate(Vector3.up, config.rotationSpeed * Time.fixedDeltaTime, Space.Self);
    }
}