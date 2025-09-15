using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Movement
{
    [RequireComponent(typeof(Rigidbody))]
    // Класс, отвечающий
    public class MovementController : MonoBehaviour
    {
        // События из новой системы ввода, привязываются в Inspector
        [SerializeField] private InputActionReference move;
        [SerializeField] private InputActionReference jump;
        // Объект с настройками для передвижения
        [SerializeField] private MovementSettings movementSettings;
        // Представление, отвечающее, за анимации
        private MovementView _view;
        // Модель, отвечает за физику
        private MovementModel _model;
        // Срабатывает во время инициализации сцены
        private void Awake()
        {
            _model = new MovementModel(GetComponent<Rigidbody>(), movementSettings);
            _view = new MovementView();

            jump.action.performed += _model.Jump;
        }
        void Start()
        {
            
        }
        
        // Update is called once per frame
        void Update()
        {
        
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
