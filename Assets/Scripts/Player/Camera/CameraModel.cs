using UnityEngine;

namespace Player.Camera
{
    public class CameraModel
    {
        private readonly CameraSettings _settings;
        private readonly Transform _playerTransform;
        private readonly Transform _cameraTransform;

        private float _xRotation = 0f;
        private float _yRotation = 0f; // ƒобавл€ем переменную дл€ горизонтального вращени€ камеры

        public CameraModel(Transform playerTransform, Transform cameraTransform, CameraSettings settings)
        {
            _playerTransform = playerTransform;
            _cameraTransform = cameraTransform;
            _settings = settings;

            // ѕозиционируем камеру на уровне глаз игрока
            _cameraTransform.localPosition = new Vector3(0, _settings.CameraHeight, 0);

            // »нициализируем начальный поворот камеры
            Vector3 startRotation = _cameraTransform.eulerAngles;
            _xRotation = startRotation.x;
            _yRotation = startRotation.y;

            // —крываем и фиксируем курсор
            Cursor.lockState = CursorLockMode.Locked;
        }

        public void RotateCamera(Vector2 mouseInput)
        {
            // ќбрабатываем вращение по горизонтали (камера)
            float mouseX = mouseInput.x * _settings.MouseSensitivity;
            _yRotation += mouseX;

            // ќбрабатываем вращение по вертикали (камера)
            float mouseY = mouseInput.y * _settings.MouseSensitivity;
            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -_settings.MaxVerticalAngle, _settings.MaxVerticalAngle);

            // ѕримен€ем оба вращени€ к камере
            _cameraTransform.localRotation = Quaternion.Euler(_xRotation, _yRotation, 0);
        }
    }
}