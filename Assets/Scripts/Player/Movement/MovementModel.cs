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

        public MovementModel(Rigidbody rb, MovementSettings settings)
        {
            _rb = rb;
            acceleration = settings.Acceleration;
            maxSpeed = settings.MaxSpeed;
            jumpForce = settings.JumpForce;
        }

        public void Move(Vector2 direction)
        {
            _rb.AddForce(new Vector3(direction.x, 0, direction.y).normalized * acceleration, ForceMode.Acceleration);
        }

        public void Jump(InputAction.CallbackContext context)
        {
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}
