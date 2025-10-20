using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform droneTransform;
    public float distance = 8f;
    public float height = 8f;
    public float smoothSpeed = 8f;

    [Header("Camera Rotation")]
    [SerializeField] private InputActionReference cameraRotateAction;
    [SerializeField] private float rotationSensitivity = 2f;
    [SerializeField] private float verticalMinAngle = 10f;
    [SerializeField] private float verticalMaxAngle = 80f;

    private Vector3 currentVelocity;
    private float currentYaw = 0f;
    private float currentPitch = 45f; // Начальный угол 45 градусов

    void Start()
    {
        // ИНИЦИАЛИЗАЦИЯ УГЛОВ КАМЕРЫ:
        // Вычисляем начальные углы на основе текущего положения камеры
        if (droneTransform != null)
        {
            Vector3 toCamera = transform.position - droneTransform.position;
            currentYaw = Mathf.Atan2(toCamera.x, toCamera.z) * Mathf.Rad2Deg;
            currentPitch = Mathf.Asin(toCamera.y / toCamera.magnitude) * Mathf.Rad2Deg;
        }
    }

    void OnEnable()
    {
        // ВКЛЮЧАЕМ INPUT ACTION ДЛЯ ВРАЩЕНИЯ КАМЕРЫ:
        cameraRotateAction?.action.Enable();
    }

    void OnDisable()
    {
        // ВЫКЛЮЧАЕМ INPUT ACTION ДЛЯ ВРАЩЕНИЯ КАМЕРЫ:
        cameraRotateAction?.action.Disable();
    }

    void LateUpdate()
    {
        if (droneTransform == null) return;

        HandleCameraRotation();
        UpdateCameraPosition();
    }

    private void HandleCameraRotation()
    {
        // ПОЛУЧАЕМ ВВОД ОТ МЫШИ:
        // ReadValue<Vector2>() возвращает изменение позиции мыши за кадр
        Vector2 mouseDelta = cameraRotateAction?.action.ReadValue<Vector2>() ?? Vector2.zero;

        // ОБНОВЛЯЕМ УГЛЫ КАМЕРЫ НА ОСНОВЕ ДВИЖЕНИЯ МЫШИ:
        // Умножаем на чувствительность и Time.deltaTime для независимости от частоты кадров
        currentYaw += mouseDelta.x * rotationSensitivity * Time.deltaTime;
        currentPitch -= mouseDelta.y * rotationSensitivity * Time.deltaTime;

        // ОГРАНИЧИВАЕМ ВЕРТИКАЛЬНЫЙ УГОЛ:
        // Чтобы камера не переворачивалась через верх или не уходила слишком низко
        currentPitch = Mathf.Clamp(currentPitch, verticalMinAngle, verticalMaxAngle);
    }

    private void UpdateCameraPosition()
    {
        // ВЫЧИСЛЯЕМ ПОЗИЦИЮ КАМЕРЫ НА ОСНОВЕ УГЛОВ:
        // Создаем вращение из углов Эйлера (pitch, yaw, 0)
        Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);

        // ВЫЧИСЛЯЕМ СМЕЩЕНИЕ КАМЕРЫ:
        // Поворачиваем вектор (0,0,-distance) на вычисленное вращение
        // Это дает нам позицию камеры на сфере вокруг дрона
        Vector3 offset = rotation * new Vector3(0, 0, -distance);

        // ДОБАВЛЯЕМ ВЫСОТУ:
        // Добавляем высоту к позиции камеры
        Vector3 targetPosition = droneTransform.position + offset + Vector3.up * height;

        // ПЛАВНОЕ ПЕРЕМЕЩЕНИЕ КАМЕРЫ:
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref currentVelocity,
            1f / smoothSpeed
        );

        // КАМЕРА СМОТРИТ НА ДРОН:
        // Всегда направляем камеру на дрон (с небольшим смещением вверх для лучшего обзора)
        transform.LookAt(droneTransform.position + Vector3.up * 1f);
    }
}