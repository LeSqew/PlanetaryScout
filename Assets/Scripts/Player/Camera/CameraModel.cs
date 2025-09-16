using UnityEngine;

namespace Player.Camera
{
    public class CameraModel
    {
        private readonly CameraSettings _settings;
        private readonly Transform _playerTransform;
        private readonly Transform _cameraTransform;

        private float _xRotation = 0f;

        public CameraModel(Transform playerTransform, Transform cameraTransform, CameraSettings settings)
        {
            _playerTransform = playerTransform;
            _cameraTransform = cameraTransform;
            _settings = settings;

            // ѕозиционируем камеру на уровне глаз игрока
            _cameraTransform.localPosition = new Vector3(0, _settings.CameraHeight, 0);

            // —крываем и фиксируем курсор
            Cursor.lockState = CursorLockMode.Locked;
        }

        public void RotateCamera(Vector2 mouseInput)
        {
            // ќбрабатываем вращение по горизонтали (игрок)
            float mouseX = mouseInput.x * _settings.MouseSensitivity;
            _playerTransform.Rotate(Vector3.up * mouseX);

            // ќбрабатываем вращение по вертикали (камера)
            float mouseY = mouseInput.y * _settings.MouseSensitivity;
            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -_settings.MaxVerticalAngle, _settings.MaxVerticalAngle);

            _cameraTransform.localRotation = Quaternion.Euler(_xRotation, 0, 0);
        }
    }
}