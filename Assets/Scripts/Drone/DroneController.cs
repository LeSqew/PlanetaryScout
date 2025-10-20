using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class DroneController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float verticalSpeed = 4f;
    [SerializeField] private float rotationStep = 45f;
    [SerializeField] private float rotationDuration = 0.5f;
    [SerializeField] private Camera droneCamera;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference upAction;
    [SerializeField] private InputActionReference downAction;
    //unite as one
    [SerializeField] private InputActionReference rotateLeftAction;
    [SerializeField] private InputActionReference rotateRightAction;

    // Компоненты и переменные
    private Rigidbody rb;
    private Vector2 moveInput = Vector2.zero;
    private float verticalInput = 0f;

    // Переменные для управления поворотом
    private bool isRotating = false;
    private float targetYaw = 0f;
    private float rotationStartTime = 0f;
    private float startYaw = 0f;

    // Сохраняем начальную ориентацию дрона
    private Vector3 initialOrientation;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // СОХРАНЯЕМ НАЧАЛЬНУЮ ОРИЕНТАЦИЮ ДРОНА:
        // Это важно для цилиндра, который обычно имеет поворот по X = 90°
        // Мы сохраняем начальные углы поворота по X и Z, чтобы не потерять ориентацию
        initialOrientation = transform.eulerAngles;

        // Инициализируем целевой угол текущим углом поворота дрона по Y
        // заглушка для переменной targetYaw
        targetYaw = transform.eulerAngles.y;
    }

    void OnEnable()
    {
        moveAction?.action.Enable();
        upAction?.action.Enable();
        downAction?.action.Enable();
        rotateLeftAction?.action.Enable();
        rotateRightAction?.action.Enable();

        if (rotateLeftAction != null)
            rotateLeftAction.action.started += OnRotateLeft;

        if (rotateRightAction != null)
            rotateRightAction.action.started += OnRotateRight;
    }

    void OnDisable()
    {
        moveAction?.action.Disable();
        upAction?.action.Disable();
        downAction?.action.Disable();
        rotateLeftAction?.action.Disable();
        rotateRightAction?.action.Disable();

        if (rotateLeftAction != null)
            rotateLeftAction.action.started -= OnRotateLeft;

        if (rotateRightAction != null)
            rotateRightAction.action.started -= OnRotateRight;
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

        // ОБРАБОТКА ПОВОРОТА:
        if (isRotating)
        {
            float progress = (Time.time - rotationStartTime) / rotationDuration;

            // поменять местами if и else
            if (progress >= 1f)
            {
                // УСТАНАВЛИВАЕМ ПОВОРОТ С СОХРАНЕНИЕМ ОРИЕНТАЦИИ:
                // Сохраняем начальные X и Z, меняем только Y
                transform.rotation = Quaternion.Euler(initialOrientation.x, targetYaw, initialOrientation.z);
                isRotating = false;
            }
            else
            {
                // ИНТЕРПОЛИРУЕМ ТОЛЬКО УГОЛ Y:
                // Сохраняем начальные X и Z ориентации, плавно меняем только Y
                float currentYaw = Mathf.LerpAngle(startYaw, targetYaw, progress);
                transform.rotation = Quaternion.Euler(initialOrientation.x, currentYaw, initialOrientation.z);
            }
        }
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    private void OnRotateLeft(InputAction.CallbackContext context)
    {
        if (!isRotating)
        {
            // ПОВОРАЧИВАЕМ ТОЛЬКО ПО ОСИ Y:
            targetYaw = Mathf.Repeat(targetYaw - rotationStep, 360f);
            startYaw = transform.eulerAngles.y;
            rotationStartTime = Time.time;
            isRotating = true;
        }
    }

    private void OnRotateRight(InputAction.CallbackContext context)
    {
        if (!isRotating)
        {
            // ПОВОРАЧИВАЕМ ТОЛЬКО ПО ОСИ Y:
            targetYaw = Mathf.Repeat(targetYaw + rotationStep, 360f);
            startYaw = transform.eulerAngles.y;
            rotationStartTime = Time.time;
            isRotating = true;
        }
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

        // maybe change to addforce
        rb.linearVelocity = horizontalMove + verticalMove;
    }
}