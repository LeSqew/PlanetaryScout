using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Movement
{
    [RequireComponent(typeof(Rigidbody))]
    public class MovementController : MonoBehaviour
    {
        [SerializeField] private InputActionReference move;
        [SerializeField] private InputActionReference jump;
        [SerializeField] private MovementSettings movementSettings;
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private Collider playerCollider; // Коллайдер назначается в инспекторе

        private MovementView _view;
        private MovementModel _model;

        private void Awake()
        {
            Rigidbody rb = GetComponent<Rigidbody>();

            // Проверяем, назначен ли коллайдер в инспекторе
            if (playerCollider == null)
            {
                Debug.LogError("Player Collider not assigned in MovementController!");
                return;
            }

            _model = new MovementModel(rb, playerCollider, movementSettings, cameraTransform);
            _view = new MovementView();

            jump.action.performed += _model.Jump;
        }

        private void OnDestroy()
        {
            if (jump?.action != null)
                jump.action.performed -= _model.Jump;
        }

        private void FixedUpdate()
        {
            if (move.action.IsPressed())
            {
                _model.Move(move.action.ReadValue<Vector2>());
            }
        }
    }
}