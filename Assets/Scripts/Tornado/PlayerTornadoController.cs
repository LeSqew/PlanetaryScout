using Player.Movement;
using UnityEngine;

namespace Tornado
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerTornadoController : MonoBehaviour
    {
        [Header("Player Settings")]
        [SerializeField] private string playerTag = "Player";
        
        private TornadoModel _model;
        private Rigidbody _rb;
        private MovementController _cc;
        private SpringJoint _joint;
        private bool _isAttached = false;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            if (_rb == null)
            {
                Debug.LogError("PlayerTornadoController: На объекте отсутствует Rigidbody!");
                enabled = false;
            }
        }

        public void Attach(TornadoModel model)
        {
            if (_isAttached) return;
            
            _model = model;
            _cc = GetComponent<MovementController>();

            if (_cc != null) 
                _cc.enabled = false;
            
            _rb.isKinematic = false;
            _rb.useGravity = false;
            _rb.linearDamping = 0.5f; // Для более реалистичного движения
            
            Vector3 toTornado = (_model.Position - transform.position).normalized;
            _rb.AddForce(toTornado * 15f, ForceMode.VelocityChange);

            _joint = gameObject.AddComponent<SpringJoint>();
            _joint.autoConfigureConnectedAnchor = false;
            _joint.connectedAnchor = _model.Position;
            _joint.spring = _model.TornadoStrength * 10f;
            _joint.damper = 5f;
            _joint.maxDistance = 2f; // Ограничиваем расстояние от центра
            
            _isAttached = true;
        }

        private void FixedUpdate()
        {
            if (!_isAttached || _model == null || _rb == null) 
                return;

            // Обновляем позицию пружины
            _joint.connectedAnchor = _model.Position;

            Vector3 diff = _model.Position - transform.position;
            Vector3 horizontalDiff = new Vector3(diff.x, 0, diff.z);
            float dist = horizontalDiff.magnitude;
            
            // Сила засасывания
            float suctionPower = _model.SuckingStrength * (1f / (dist + 0.5f));
            suctionPower = Mathf.Clamp(suctionPower, _model.SuckingStrength * 0.3f, _model.SuckingStrength);
            _rb.AddForce(horizontalDiff.normalized * suctionPower, ForceMode.Force);

            // Вращательная сила
            Vector3 orbitDir = Quaternion.AngleAxis(90, Vector3.up) * horizontalDiff.normalized;
            _rb.AddForce(orbitDir * (_model.RotationStrength * 0.5f), ForceMode.Force);

            // Вертикальное движение вверх (как настоящий торнадо)
            float verticalLift = 25f / (dist + 2f); // Мягкий подъем
            _rb.AddForce(Vector3.up * verticalLift, ForceMode.Force);

            // Турбулентность
            Vector3 turbulence = new Vector3(
                Random.Range(-0.5f, 0.5f),
                Random.Range(0.3f, 0.7f),
                Random.Range(-0.5f, 0.5f)
            ) * 2f;
            _rb.AddForce(turbulence, ForceMode.Force);
        }

        public void ApplyThrow(Vector3 tornadoPos, float force)
        {
            if (!_isAttached) return;
            
            Vector3 throwDir = (transform.position - tornadoPos).normalized;
            throwDir = new Vector3(throwDir.x, 0.5f, throwDir.z).normalized; 

            Detach();
            _rb.AddForce(throwDir * force, ForceMode.Impulse);
        }

        public void Detach()
        {
            if (!_isAttached) return;
            
            if (_joint != null) 
                Destroy(_joint);
                
            if (_cc != null) 
                _cc.enabled = true;
                
            if (_rb != null)
            {
                _rb.useGravity = true;
                _rb.linearDamping = 0f;
            }
            
            _model = null;
            _isAttached = false;
        }

        private void OnDestroy()
        {
            Detach();
        }
    }
}