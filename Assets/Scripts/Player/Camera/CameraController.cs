using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Camera
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform playerTransform;
        [SerializeField] private CameraSettings cameraSettings;
        [SerializeField] private InputActionReference lookAction;

        private CameraModel _model;
        //private CameraView _view;

        private void Awake()
        {
            _model = new CameraModel(playerTransform, transform, cameraSettings);
            //_view = new CameraView();
        }

        private void OnEnable()
        {
            lookAction.action.Enable();
        }

        private void OnDisable()
        {
            lookAction.action.Disable();
        }

        private void Update()
        {
            // Получаем ввод мыши и обрабатываем вращение камеры
            Vector2 lookValue = lookAction.action.ReadValue<Vector2>();
            _model.RotateCamera(lookValue);
        }
    }
}