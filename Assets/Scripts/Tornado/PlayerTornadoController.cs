using UnityEngine;

namespace Tornado
{
    public class PlayerTornadoController : MonoBehaviour
    {
        private TornadoModel _tornadoModel;
        private Rigidbody _playerRigidbody;
        private CharacterController _characterController;
        private bool _wasControlEnabled = true;
        private SpringJoint _springJoint;

        private void Start()
        {
            _playerRigidbody = GetComponent<Rigidbody>();
            _characterController = GetComponent<CharacterController>();
        }

        public void AttachToTornado(TornadoModel tornadoModel)
        {
            _tornadoModel = tornadoModel;
        
            if (_characterController != null)
            {
                _wasControlEnabled = _characterController.enabled;
                _characterController.enabled = false;
            }

            CreateSpringJoint();
        }

        public void DetachFromTornado()
        {
            if (_springJoint != null)
            {
                Destroy(_springJoint);
                _springJoint = null;
            }

            if (_characterController != null)
            {
                _characterController.enabled = _wasControlEnabled;
            }

            _tornadoModel = null;
        }

        private void CreateSpringJoint()
        {
            if (_playerRigidbody != null && _tornadoModel != null)
            {
                _springJoint = gameObject.AddComponent<SpringJoint>();
                _springJoint.spring = _tornadoModel.TornadoStrength;
                _springJoint.damper = _tornadoModel.TornadoStrength * 0.1f;
                _springJoint.autoConfigureConnectedAnchor = false;
                _springJoint.connectedAnchor = _tornadoModel.Position;
            }
        }

        private void FixedUpdate()
        {
            if (_tornadoModel != null && _springJoint != null && _playerRigidbody != null)
            {
                // Обновляем точку подключения
                _springJoint.connectedAnchor = _tornadoModel.Position;

                // Применяем силу вращения (упрощенная версия из оригинального скрипта)
                Vector3 direction = transform.position - _tornadoModel.Position;
                Vector3 projection = Vector3.ProjectOnPlane(direction, Vector3.up); // Ось вращения Y
            
                if (projection.magnitude > 0.1f) // Защита от деления на ноль
                {
                    projection.Normalize();
                
                    // Поворот на 130 градусов для направления вращения
                    Vector3 normal = Quaternion.AngleAxis(130, Vector3.up) * projection;
                
                    // Добавляем силу вращения
                    _playerRigidbody.AddForce(normal * _tornadoModel.RotationStrength, ForceMode.Force);
                }
            }
        }
    }
}