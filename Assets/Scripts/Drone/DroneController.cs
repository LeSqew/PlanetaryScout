using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class DroneController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 6f;          // Скорость горизонтального движения
    [SerializeField] private float verticalSpeed = 4f;      // Скорость вертикального движения (вверх/вниз)
    [SerializeField] private Camera droneCamera;            // Камера, относительно которой происходит движение

    // Компоненты и ссылки
    private Rigidbody rb;                   // Физическое тело дрона
    private InputSystem_Actions controls;   // Система ввода (новый Input System)
    private Vector2 moveInput = Vector2.zero; // Ввод для горизонтального движения (WASD/Стик)
    private float verticalInput = 0f;       // Ввод для вертикального движения (Вверх/Вниз)

    void Awake()
    {
        // Получаем компонент Rigidbody - он обязателен благодаря [RequireComponent]
        rb = GetComponent<Rigidbody>();

        // Создаем экземпляр системы ввода
        controls = new InputSystem_Actions();

        // ПОДПИСКА НА СОБЫТИЯ ВВОДА:
        // Horizontal movement (WASD/Left Stick)
        controls.Drone.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Drone.Move.canceled += ctx => moveInput = Vector2.zero;

        // Vertical movement (Space/Ctrl или кнопки вверх/вниз)
        // Важно: используем отдельные условия для отмены, чтобы не конфликтовать 
        // при одновременном нажатии вверх+вниз
        controls.Drone.Up.performed += ctx => verticalInput = 1f;
        controls.Drone.Up.canceled += ctx => { if (verticalInput > 0) verticalInput = 0f; };
        controls.Drone.Down.performed += ctx => verticalInput = -1f;
        controls.Drone.Down.canceled += ctx => { if (verticalInput < 0) verticalInput = 0f; };
    }

    void OnEnable()
    {
        // АКТИВАЦИЯ УПРАВЛЕНИЯ ДРОНОМ:
        // Если controls не инициализирован (на всякий случай), создаем новый
        if (controls == null) controls = new InputSystem_Actions();

        // Включаем только управление дроном, отключаем другие (например, игрока)
        controls.Drone.Enable();
    }

    void OnDisable()
    {
        // ДЕАКТИВАЦИЯ УПРАВЛЕНИЯ:
        // Отключаем управление дроном при деактивации объекта
        // ?. - оператор null-conditional (если controls null, не пытаться вызывать Disable)
        controls?.Drone.Disable();
    }

    void FixedUpdate()
    {
        // ДВИЖЕНИЕ В FixedUpdate:
        // Все физические взаимодействия, включая движение через Rigidbody,
        // должны быть в FixedUpdate для стабильной работы физики

        // ПОЛУЧАЕМ НАПРАВЛЯЮЩИЕ ВЕКТОРЫ КАМЕРЫ:
        // Движение всегда происходит относительно камеры, чтобы игрок 
        // интуитивно понимал управление (W - вперед от камеры и т.д.)
        Vector3 forward = droneCamera.transform.forward;
        Vector3 right = droneCamera.transform.right;

        // ИГНОРИРУЕМ НАКЛОН КАМЕРЫ:
        // Обнуляем Y-компоненту, чтобы дрон двигался только в горизонтальной плоскости
        // В противном случае при наклоне камеры дрон будет пытаться лететь "вниз" или "вверх"
        forward.y = 0f;
        right.y = 0f;

        // Нормализуем векторы после изменения Y-компоненты
        // (изменение длины вектора может повлиять на скорость движения)
        forward.Normalize();
        right.Normalize();

        // РАСЧЕТ ГОРИЗОНТАЛЬНОГО ДВИЖЕНИЯ:
        // moveInput.y - вперед/назад (W/S), moveInput.x - влево/вправо (A/D)
        // Умножаем на скорость и получаем вектор движения в мировых координатах
        Vector3 horizontalMove = (forward * moveInput.y + right * moveInput.x) * moveSpeed;

        // РАСЧЕТ ВЕРТИКАЛЬНОГО ДВИЖЕНИЯ:
        // verticalInput: 1 = вверх, -1 = вниз, 0 = остановка
        // Умножаем на вертикальную скорость - движение по оси Y в мировых координатах
        Vector3 verticalMove = Vector3.up * verticalInput * verticalSpeed;

        // ПРИМЕНЯЕМ СКОРОСТЬ К Rigidbody:
        // Складываем горизонтальную и вертикальную составляющие
        // linearVelocity - непосредственное задание скорости, обходит физику трения,
        // но дает точный контроль над движением
        rb.linearVelocity = horizontalMove + verticalMove;
    }
}