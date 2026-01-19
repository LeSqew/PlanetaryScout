using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Управляет поведением игрока при приседании:
/// - меняет высоту коллайдера и визуальный масштаб модели;
/// - хранит текущее состояние (стоит / присел);
/// - передаёт информацию о приседе для систем видимости.
/// </summary>
public class PlayerStealthController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Трансформ капсулы, представляющей тело игрока.")]
    [SerializeField] private Transform playerBody;

    [Tooltip("Капсульный коллайдер на модели игрока.")]
    [SerializeField] private CapsuleCollider playerCollider;

    [Tooltip("Input System действие для приседа.")]
    [SerializeField] private InputActionReference crouchAction;

    [Header("Crouch Settings")]
    [Tooltip("Высота коллайдера в положении стоя.")]
    [SerializeField] private float standHeight = 2f;

    [Tooltip("Высота коллайдера в положении приседа.")]
    [SerializeField] private float crouchHeight = 1f;

    [Tooltip("Скорость плавного перехода между состояниями.")]
    [SerializeField] private float transitionSpeed = 8f;

    /// <summary>Текущее состояние приседа (true = игрок присел).</summary>
    [HideInInspector] public bool isCrouching = false;

    private float targetHeight;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        targetHeight = standHeight;
    }

    private void Update()
    {
        HandleCrouchInput();
        SmoothUpdateColliderHeight();
    }

    /// <summary>
    /// Обрабатывает ввод для приседа — переключает между состояниями стоя/присел.
    /// </summary>
    private void HandleCrouchInput()
    {
        if (crouchAction.action.WasPressedThisFrame())
        {
            isCrouching = !isCrouching;
            targetHeight = isCrouching ? crouchHeight : standHeight;
        }
    }

    /// <summary>
    /// Плавно изменяет высоту коллайдера и масштаб модели игрока.
    /// </summary>
    private void SmoothUpdateColliderHeight()
    {
        if (playerCollider == null) return;

        // Плавное изменение высоты
        playerCollider.height = Mathf.Lerp(playerCollider.height, targetHeight, Time.deltaTime * transitionSpeed);

        // Центрируем коллайдер
        playerCollider.center = new Vector3(playerCollider.center.x, playerCollider.height / 2f, playerCollider.center.z);

        // Масштабируем визуальную модель
        if (playerBody != null)
        {
            float scaleFactor = playerCollider.height / standHeight;
            playerBody.localScale = new Vector3(1f, scaleFactor, 1f);
        }
    }
}