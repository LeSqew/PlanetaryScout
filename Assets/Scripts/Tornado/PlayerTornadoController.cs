using Player.Movement;
using UnityEngine;

namespace Tornado
{
    public class PlayerTornadoController : MonoBehaviour
    {
        private TornadoModel _model;
        private Rigidbody _rb;
        private MovementController _cc;
        private SpringJoint _joint;

        public void Attach(TornadoModel model)
        {
            _model = model;
            _rb = GetComponent<Rigidbody>();
            _cc = GetComponent<MovementController>();

            if (_cc != null) _cc.enabled = false;
            if (_rb != null)
            {
                _rb.isKinematic = false;
                _rb.useGravity = false;
                Vector3 toTornado = (_model.Position - transform.position).normalized;
                _rb.AddForce(toTornado * 15f, ForceMode.VelocityChange);
            }

            _joint = gameObject.AddComponent<SpringJoint>();
            _joint.autoConfigureConnectedAnchor = false;
            _joint.spring = _model.TornadoStrength * 10f; // Делаем пружину сильнее
            _joint.damper = 5f;
        }

        private void FixedUpdate()
        {
            if (_model == null || _rb == null) return;

            Vector3 tornadoPos = _model.Position;
            _joint.connectedAnchor = tornadoPos;

            Vector3 diff = tornadoPos - transform.position;
            Vector3 horizontalDiff = new Vector3(diff.x, 0, diff.z);
            float dist = horizontalDiff.magnitude;
            
            float suctionPower = _model.SuckingStrength * (1.5f / (dist + 0.5f)); 
            suctionPower = Mathf.Clamp(suctionPower, _model.SuckingStrength, _model.SuckingStrength * 3f);

            _rb.AddForce(horizontalDiff.normalized * suctionPower, ForceMode.Acceleration);

            Vector3 orbitDir = Quaternion.AngleAxis(110, Vector3.up) * horizontalDiff.normalized;
            _rb.AddForce(orbitDir * _model.RotationStrength, ForceMode.Acceleration);

            _joint.spring = Mathf.Lerp(_model.TornadoStrength * 5f, _model.TornadoStrength, dist / 10f);
            
            Vector3 turbulence = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(0.5f, 1.5f),
                Random.Range(-1f, 1f)
            ) * 5f;
            _rb.AddForce(turbulence, ForceMode.Acceleration);

            float lift = 10f / (dist + 1f);
            _rb.AddForce(Vector3.up * lift, ForceMode.Acceleration);
        }

        public void ApplyThrow(Vector3 tornadoPos, float force)
        {
            Vector3 throwDir = (transform.position - tornadoPos).normalized;
            throwDir.y = 0.5f; 

            Detach();
            _rb.AddForce(throwDir.normalized * force, ForceMode.Impulse);
        }

        public void Detach()
        {
            if (_joint != null) Destroy(_joint);
            if (_cc != null) _cc.enabled = true;
            if (_rb != null)
            {
                _rb.useGravity = true;
                _rb.linearDamping = 0.5f; 
            }
            _model = null;
            Destroy(this, 1f);
        }
    }
}