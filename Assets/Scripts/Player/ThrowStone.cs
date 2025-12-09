using UnityEngine;
using UnityEngine.InputSystem;

public class ThrowStone : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference throwAction;

    [Header("Throw Settings")]
    public Transform throwOrigin;
    public Camera playerCamera;
    public GameObject stonePrefab;
    public float throwDistance = 10f;
    public float throwArcHeight = 2f; // Просто вертикальный импульс
    public float throwForceMultiplier = 1f;
    public float throwCooldown = 1f; // Время перезарядки

    [Header("Sound")]
    public AudioClip stoneFallSound; // звук падения камня
    public float soundRadius = 10f; // радиус, на который реагируют собаки
    public LayerMask dogLayer; // слой для поиска собак

    private bool canThrow = true;

    private void OnEnable()
    {
        if (throwAction != null)
            throwAction.action.Enable();
    }

    private void OnDisable()
    {
        if (throwAction != null)
            throwAction.action.Disable();
    }

    private void Update()
    {
        if (canThrow && throwAction != null && throwAction.action.WasPressedThisFrame())
        {
            ThrowStoneMethod();
        }
    }

    private void ThrowStoneMethod()
    {
        if (stonePrefab == null || throwOrigin == null || playerCamera == null)
        {
            Debug.LogWarning("Не назначен префаб, throwOrigin или камера!");
            return;
        }

        GameObject stone = Instantiate(stonePrefab, throwOrigin.position, Quaternion.identity);

        Rigidbody rb = stone.GetComponent<Rigidbody>();
        if (rb == null)
            rb = stone.AddComponent<Rigidbody>();

        rb.mass = 1f;  // нормальная масса
        rb.linearDamping = 0.1f; // немного сопротивления воздуха
        rb.angularDamping = 0.05f;
        rb.useGravity = true;

        // Рассчитываем направление
        Vector3 targetDir = playerCamera.transform.forward;

        // Добавляем вертикальный компонент для дуги
        targetDir.y += throwArcHeight / throwDistance; // регулируем высоту дуги

        // Применяем силу импульсом
        rb.AddForce(targetDir.normalized * throwDistance * throwForceMultiplier, ForceMode.VelocityChange);

        // Добавляем компонент для падения камня
        StoneImpact impact = stone.AddComponent<StoneImpact>();
        impact.fallSound = stoneFallSound;
        impact.soundRadius = soundRadius;
        impact.dogLayer = dogLayer;

        // блокировка и перезарядка
        canThrow = false;
        Invoke(nameof(ResetThrow), throwCooldown);
    }

    private void ResetThrow()
    {
        canThrow = true;
    }
}