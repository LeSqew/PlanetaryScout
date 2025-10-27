using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform droneTransform;
    public float height = 15f; // Увеличили высоту для обзора сверху
    public float distance = 10f; // Увеличили дистанцию
    public float smoothSpeed = 8f;

    [Header("Camera Angle")]
    [SerializeField] private float cameraPitch = 60f; // Угол наклона камеры (смотрим сверху)

    private Vector3 currentVelocity;

    void LateUpdate()
    {
        if (droneTransform == null) return;

        // КАМЕРА ВСЕГДА СЛЕДУЕТ ЗА ДРОНОМ СВЕРХУ:
        // Используем поворот дрона для определения направления камеры
        float droneYaw = droneTransform.eulerAngles.y;

        // Создаем вращение камеры на основе угла дрона и фиксированного наклона
        Quaternion rotation = Quaternion.Euler(cameraPitch, droneYaw, 0f);

        // Вычисляем смещение камеры
        Vector3 offset = rotation * new Vector3(0, 0, -distance);

        // Целевая позиция камеры
        Vector3 targetPosition = droneTransform.position + offset + Vector3.up * height;

        // Плавное перемещение камеры
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref currentVelocity,
            1f / smoothSpeed
        );

        // КАМЕРА ВСЕГДА СМОТРИТ НА ДРОН:
        // Смотрим на точку немного выше дрона для лучшего обзора
        transform.LookAt(droneTransform.position + Vector3.up * 2f);
    }
}