using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class DroneController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float verticalSpeed = 4f;
    [SerializeField] private Camera droneCamera;

    private Rigidbody rb;
    private InputSystem_Actions controls;
    private Vector2 moveInput = Vector2.zero;
    private float verticalInput = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        controls = new InputSystem_Actions();

        controls.Drone.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Drone.Move.canceled += ctx => moveInput = Vector2.zero;
        controls.Drone.Up.performed += ctx => verticalInput = 1f;
        controls.Drone.Up.canceled += ctx => { if (verticalInput > 0) verticalInput = 0f; };
        controls.Drone.Down.performed += ctx => verticalInput = -1f;
        controls.Drone.Down.canceled += ctx => { if (verticalInput < 0) verticalInput = 0f; };
    }

    void OnEnable()
    {
        if (controls == null) controls = new InputSystem_Actions();
        controls.Player.Disable();
        controls.Drone.Enable();
    }

    void OnDisable()
    {
        if (controls == null) return;
        controls.Drone.Disable();
        controls.Player.Enable();
    }

    void FixedUpdate()
    {
        // Определяем горизонтальное направление в глобальных координатах (движение всегда относительно камеры!)
        Vector3 forward = droneCamera.transform.forward;
        forward.y = 0f;
        forward.Normalize();
        Vector3 right = droneCamera.transform.right;
        right.y = 0f;
        right.Normalize();

        Vector3 horizontal = (forward * moveInput.y + right * moveInput.x) * moveSpeed;

        // Вертикальное движение отдельно, не зависит от камеры
        float y = verticalInput * verticalSpeed;

        // Если ничего не нажато — стоим!
        Vector3 velocity = new Vector3(horizontal.x, y, horizontal.z);

        // Не стоит задавать напрямую велосити
        
        rb.linearVelocity = velocity;
    }
}