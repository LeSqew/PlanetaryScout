using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Rigidbody droneRb; // Перетащи СЮДА Rigidbody дрона
    public float height = 8f;
    public float distance = 8f;
    public float angle = 45f;
    public float smoothSpeed = 8f;

    void LateUpdate()
    {
        if (droneRb == null) return;

        // Высчитываем позицию камеры над дрона под углом
        Quaternion rot = Quaternion.Euler(angle, droneRb.transform.eulerAngles.y, 0);
        Vector3 offset = rot * new Vector3(0, 0, -distance);
        offset.y += height;

        Vector3 desiredPos = droneRb.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * smoothSpeed);

        // Смотрим на дрона
        transform.LookAt(droneRb.position + Vector3.up * 1f);
    }
}