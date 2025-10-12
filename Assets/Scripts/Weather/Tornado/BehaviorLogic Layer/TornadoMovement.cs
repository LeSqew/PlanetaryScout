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
    
            if (target != null && Vector3.Distance(transform.position, target.position) < maxChaseDistance)
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
                // свободное блуждание
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
