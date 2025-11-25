using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Movement
{
    public class MovementModel
    {
        private Rigidbody _rb;
        private Collider _collider; 
        private float acceleration;
        private float maxSpeed;
        private float jumpForce;
        private float jumpRayDistance;
        private Transform cameraTransform;
        private LayerMask groundMask;
        private float groundCheckOffset = 0.1f;
        private float sphereRadiusMultiplier = 0.9f;

        public MovementModel(Rigidbody rb, Collider collider, MovementSettings settings, Transform cameraTransform)
        {
            _rb = rb;
            _collider = collider;
            acceleration = settings.Acceleration;
            maxSpeed = settings.MaxSpeed;
            jumpForce = settings.JumpForce;
            jumpRayDistance = settings.JumpRayDistance;
            this.cameraTransform = cameraTransform;
            groundMask = settings.GroundLayerMask;
        }

        public void Move(Vector2 direction)
        {
            // �������� ��� ����������� � ������ (� ���������� Y!)
            Vector3 camForward = cameraTransform.forward;
            camForward.y = 0;
            camForward.Normalize();
            Vector3 camRight = cameraTransform.right;
            camRight.y = 0;
            camRight.Normalize();

            Vector3 moveDir = camForward * direction.y + camRight * direction.x;
            moveDir.Normalize();

            _rb.AddForce(moveDir * acceleration, ForceMode.Acceleration);
            // ����� ���������� ���� � ��������� ��������!
            LimitSpeed();
        }

        private void LimitSpeed()
        {
            Vector3 horizontalVel = _rb.linearVelocity;
            horizontalVel.y = 0;
            if (horizontalVel.magnitude > maxSpeed)
            {
                Vector3 clampedVel = horizontalVel.normalized * maxSpeed;
                _rb.linearVelocity = new Vector3(clampedVel.x, _rb.linearVelocity.y, clampedVel.z);
            }
        }

        public void Jump(InputAction.CallbackContext context)
        {
            Debug.Log(IsGrounded);
            if (context.performed && IsGrounded)
            {
                _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }

        public bool IsGrounded
        {
            get
            {
                // ��������� ����� ������ ���� (������ ����� ����������)
                Vector3 origin = _collider.bounds.center - new Vector3(0, _collider.bounds.extents.y - 0.1f, 0);

                // �����-������������ ����
                Debug.DrawRay(origin, Vector3.down * jumpRayDistance, Color.red);
                Debug.Log(origin);

                // ������� ��� ����
                var isGrounded = Physics.Raycast(
                    origin,
                    Vector3.down,
                    out var hit,
                    jumpRayDistance
                );
                
                // Debug.Log(hit.collider.name);

                return isGrounded && !hit.collider.CompareTag("Player") && !hit.collider.isTrigger;
            }
        }
        
        /* public bool IsGrounded
        {
            get
            {
                float radius = _collider.bounds.extents.x * 0.8f;
                Vector3 pos = _collider.bounds.center + Vector3.down * (_collider.bounds.extents.y - 0.05f);

                bool grounded = Physics.CheckSphere(pos, radius, groundLayerMask, QueryTriggerInteraction.Ignore);

                return grounded;
            }
        }
        public bool IsGrounded
        {
            get
            {
                float radius = _collider.bounds.extents.x * 0.8f; // немного меньше половины ширины
                Vector3 origin = _collider.bounds.center - new Vector3(0, _collider.bounds.extents.y - 0.1f, 0);

                Debug.DrawRay(origin, Vector3.down * jumpRayDistance, Color.yellow);

                if (Physics.SphereCast(
                        origin,
                        radius,
                        Vector3.down,
                        out RaycastHit hit,
                        jumpRayDistance
                    ))
                {
                    // Фильтруем: не игрок и не триггеры
                    if (!hit.collider.isTrigger && !hit.collider.CompareTag("Player"))
                        return true;
                }

                return false;
            }
        }
        
        public bool IsGrounded
        {
            get
            {
                // Радиус: чуть меньше ширины коллайдера
                float radius = _collider.bounds.extents.x * sphereRadiusMultiplier;

                // Точка старта SphereCast — центр коллайдера
                Vector3 origin = _collider.bounds.center;

                // Длина кастинга
                float distance = _collider.bounds.extents.y + groundCheckOffset;

                Debug.DrawRay(origin, Vector3.down * distance, Color.green);

                // 1) SphereCast — основной способ
                if (Physics.SphereCast(
                        origin,
                        radius,
                        Vector3.down,
                        out RaycastHit hit,
                        distance,
                        groundMask,
                        QueryTriggerInteraction.Ignore
                    ))
                {
                    return true;
                }

                // 2) CheckSphere внизу — ловит редкие пропуски
                Vector3 bottom = _collider.bounds.center - new Vector3(0, _collider.bounds.extents.y - radius * 0.5f, 0);

                if (Physics.CheckSphere(
                        bottom,
                        radius * 0.9f,
                        groundMask,
                        QueryTriggerInteraction.Ignore
                    ))
                {
                    return true;
                }

                return false;
            }
        }*/


    }
}
