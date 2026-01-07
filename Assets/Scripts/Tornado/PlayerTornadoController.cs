using UnityEngine;

namespace Tornado
{
    public class PlayerTornadoController : MonoBehaviour
    {
        private TornadoModel _model;
        private Rigidbody _rb;
        private CharacterController _cc;
        private SpringJoint _joint;

        public void Attach(TornadoModel model)
        {
            _model = model;
            _rb = GetComponent<Rigidbody>();
            _cc = GetComponent<CharacterController>();

            if (_cc != null) _cc.enabled = false;
            if (_rb != null)
            {
                _rb.isKinematic = false;
                _rb.useGravity = false;
                _rb.linearVelocity = Vector3.zero; 
            }

            _joint = gameObject.AddComponent<SpringJoint>();
            _joint.autoConfigureConnectedAnchor = false;

            _joint.spring = _model.TornadoStrength * 0.5f; 
            _joint.damper = 15f; 
           
            _joint.minDistance = 0f;
            _joint.maxDistance = 1.5f;
        }

        private void FixedUpdate()
        {
            if (_model == null || _rb == null) return;

            Vector3 tornadoPos = _model.Position;
            _joint.connectedAnchor = tornadoPos;

            Vector3 diff = tornadoPos - transform.position;
            Vector3 horizontalDiff = new Vector3(diff.x, 0, diff.z);
            float dist = horizontalDiff.magnitude;

            float captureRadius = 12f; 
            float smoothFactor = Mathf.Clamp01(1f - (dist / captureRadius));

            _rb.AddForce(horizontalDiff.normalized * _model.SuckingStrength * smoothFactor, ForceMode.Acceleration);

            Vector3 orbitDir = Quaternion.AngleAxis(130, Vector3.up) * horizontalDiff.normalized;
            _rb.AddForce(orbitDir * _model.RotationStrength * smoothFactor, ForceMode.Acceleration);

    
            float liftBase = Mathf.Clamp(8f / (dist + 1f), 0f, 10f);
            _rb.AddForce(Vector3.up * liftBase * smoothFactor, ForceMode.Acceleration);
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