using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class DroneController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float verticalSpeed = 4f;
    [SerializeField] private float rotationSensitivity = 0.5f; // Чувствительность поворота мышью
    [SerializeField] private Camera droneCamera;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference upAction;
    [SerializeField] private InputActionReference downAction;
    [SerializeField] private InputActionReference droneRotateAction; // Поворот дрона мышью

    // Компоненты и переменные
    private Rigidbody rb;
    private Vector2 moveInput = Vector2.zero;
    private float verticalInput = 0f;

    // Переменные для управления поворотом
    private float currentYaw = 0f;

    // Сохраняем начальную ориентацию дрона
    private Vector3 initialOrientation;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // СОХРАНЯЕМ НАЧАЛЬНУЮ ОРИЕНТАЦИЮ ДРОНА:
        initialOrientation = transform.eulerAngles;

        // Инициализируем текущий угол поворота
        currentYaw = transform.eulerAngles.y;

        // Настройка Rigidbody для плавности
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void OnEnable()
    {
        moveAction?.action.Enable();
        upAction?.action.Enable();
        downAction?.action.Enable();
        droneRotateAction?.action.Enable();
    }

    void OnDisable()
    {
        moveAction?.action.Disable();
        upAction?.action.Disable();
        downAction?.action.Disable();
        droneRotateAction?.action.Disable();
    }

    void Update()
    {
        // ОБРАБОТКА ВВОДА ДЛЯ ДВИЖЕНИЯ:
        if (moveAction != null)
            moveInput = moveAction.action.ReadValue<Vector2>();

        verticalInput = 0f;
        if (upAction != null && upAction.action.ReadValue<float>() > 0.1f)
            verticalInput = 1f;
        if (downAction != null && downAction.action.ReadValue<float>() > 0.1f)
            verticalInput = -1f;

        // ОБРАБОТКА ПОВОРОТА МЫШЬЮ:
        if (droneRotateAction != null)
        {
            // Получаем движение мыши
            Vector2 mouseDelta = droneRotateAction.action.ReadValue<Vector2>();

            // Применяем поворот только по горизонтали (ось X мыши)
            currentYaw += mouseDelta.x * rotationSensitivity;

            // Нормализуем угол в диапазон 0-360
            currentYaw = Mathf.Repeat(currentYaw, 360f);

            // НЕМЕДЛЕННО ПРИМЕНЯЕМ ПОВОРОТ:
            // Сохраняем начальные X и Z, меняем только Y
            transform.rotation = Quaternion.Euler(initialOrientation.x, currentYaw, initialOrientation.z);
        }
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        // ДВИЖЕНИЕ ОТНОСИТЕЛЬНО КАМЕРЫ:
        Vector3 forward = droneCamera.transform.forward;
        Vector3 right = droneCamera.transform.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 horizontalMove = (forward * moveInput.y + right * moveInput.x) * moveSpeed;
        Vector3 verticalMove = Vector3.up * verticalInput * verticalSpeed;

        rb.linearVelocity = horizontalMove + verticalMove;
    }
}