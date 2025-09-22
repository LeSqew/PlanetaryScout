using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Movement
{
    public class MovementModel
    {
        private Rigidbody _rb;
        private float acceleration;
        private float maxSpeed;
        private float jumpForce;
        private Transform cameraTransform;

        public MovementModel(Rigidbody rb, MovementSettings settings, Transform cameraTransform)
        {
            _rb = rb;
            acceleration = settings.Acceleration;
            maxSpeed = settings.MaxSpeed;
            jumpForce = settings.JumpForce;
            this.cameraTransform = cameraTransform;
        }

        public void Move(Vector2 direction)
        {
            // ѕолучаем оси направлени€ с камеры (с обнулением Y!)
            Vector3 camForward = cameraTransform.forward;
            camForward.y = 0;
            camForward.Normalize();
            Vector3 camRight = cameraTransform.right;
            camRight.y = 0;
            camRight.Normalize();

            Vector3 moveDir = camForward * direction.y + camRight * direction.x;
            moveDir.Normalize();

            _rb.AddForce(moveDir * acceleration, ForceMode.Acceleration);
            // ѕосле применени€ силы Ч ограничим скорость!
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
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}
