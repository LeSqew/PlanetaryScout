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

        public MovementModel(Rigidbody rb, Collider collider, MovementSettings settings, Transform cameraTransform)
        {
            _rb = rb;
            _collider = collider;
            acceleration = settings.Acceleration;
            maxSpeed = settings.MaxSpeed;
            jumpForce = settings.JumpForce;
            jumpRayDistance = settings.JumpRayDistance;
            this.cameraTransform = cameraTransform;
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

    }
}
