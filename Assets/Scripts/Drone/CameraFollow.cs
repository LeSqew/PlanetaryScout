using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform droneTransform;     // Цель слежения (трансформ дрона)
    public float height = 8f;            // Высота камеры над дроном
    public float distance = 8f;          // Дистанция от дрона по Z-оси
    public float angle = 45f;            // Угол наклона камеры в градусах
    public float smoothSpeed = 8f;       // Скорость сглаживания (чем больше - тем быстрее)

    // СЛЕДОВАНИЕ КАМЕРЫ В LateUpdate:
    // LateUpdate вызывается после всех Update, чтобы камера
    // точно обрабатывала уже обновленную позицию дрона

    // ДЛЯ SMOOTH DAMP:
    // Vector3 для хранения текущей скорости - требуется для SmoothDamp
    // Не путать с velocity Rigidbody - это скорость изменения позиции камеры
    private Vector3 currentVelocity;

    void LateUpdate()
    {
        // ЗАЩИТА ОТ ОТСУТСТВИЯ ЦЕЛИ:
        // Если дрон не назначен, выходим чтобы избежать ошибок
        if (droneTransform == null) return;

        // ВЫЧИСЛЕНИЕ ПОЗИЦИИ КАМЕРЫ:
        // Создаем вращение на основе угла наклона и поворота дрона по Y
        // Quaternion.Euler преобразует углы Эйлера в кватернион
        // angle - наклон вниз, droneTransform.eulerAngles.y - поворот камеры за дроном
        Quaternion rotation = Quaternion.Euler(angle, droneTransform.eulerAngles.y, 0);

        // Создаем смещение: поворачиваем вектор (0,0,-distance) на вычисленное вращение
        // Это дает нам позицию позади и выше дрона с учетом его поворота
        Vector3 offset = rotation * new Vector3(0, 0, -distance);

        // ФИНАЛЬНАЯ ПОЗИЦИЯ КАМЕРЫ:
        // Позиция дрона + смещение + дополнительная высота
        Vector3 targetPosition = droneTransform.position + offset + Vector3.up * height;

        // ПЛАВНОЕ ПЕРЕМЕЩЕНИЕ КАМЕРЫ:
        // Vector3.SmoothDamp плавно изменяет позицию камеры к целевой позиции
        // Параметры:
        // - transform.position: текущая позиция камеры
        // - targetPosition: желаемая позиция камеры  
        // - ref currentVelocity: ссылка на переменную скорости (изменяется внутри метода)
        // - 1f / smoothSpeed: время достижения цели (обратная зависимость)
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref currentVelocity,
            1f / smoothSpeed
        );

        // КАМЕРА СМОТРИТ НА ДРОН:
        // Заставляем камеру всегда смотреть на точку немного выше центра дрона
        // Vector3.up * 1f - смотрим на высоту 1 единица над дроном для лучшего обзора
        transform.LookAt(droneTransform.position + Vector3.up * 1f);
    }
}