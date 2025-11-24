using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System.Collections;

[RequireComponent(typeof(RectTransform))]
public class BurController : MonoBehaviour
{
    [Header("Game Settings")]
    [Tooltip("Speed of the point (normalized units per second)")]
    public float moveSpeed = 0.3f;
    public float driftStrength = 0.1f;
    public float driftChangeFrequency = 2f;
    public float greenZoneWidth = 200f;
    public float winTime = 10f;
    public float loseTime = 5f;

    [Header("UI References")]
    public RectTransform movingPoint;
    public RectTransform greenZone;
    public Image pointImage;
    public GameObject winScreen;
    public GameObject loseScreen;
    public TMP_Text timerText;
    public TMP_Text statusText;

    [Header("Input Actions")]
    public InputActionReference leftClickAction;
    public InputActionReference rightClickAction;

    // Internal
    private InputAction leftClick;
    private InputAction rightClick;
    private RectTransform containerRect;

    private BurModel model;
    private BurView view;

    private Coroutine resetCoroutine;

    void Awake()
    {
        containerRect = GetComponent<RectTransform>();

        // Init model and view
        model = new BurModel(moveSpeed, driftStrength, driftChangeFrequency, winTime, loseTime, 0.5f);
        view = new BurView(containerRect, movingPoint, greenZone, pointImage, winScreen, loseScreen, timerText, statusText);

        // Wire events
        model.OnPositionChanged += HandlePositionChanged;
        model.OnTimerUpdated += HandleTimerUpdated;
        model.OnWin += HandleWin;
        model.OnLose += HandleLose;

        // Input actions
        if (leftClickAction != null) { leftClick = leftClickAction.action; leftClick.Enable(); }
        if (rightClickAction != null) { rightClick = rightClickAction.action; rightClick.Enable(); }

        // Initial UI
        view.SetGreenZoneWidth(greenZoneWidth);
        view.SetPointNormalized(model.Position);
        view.ShowWinScreen(false);
        view.ShowLoseScreen(false);
        view.SetStatusText("Держите бур в зеленой зоне!");
    }

    void OnDestroy()
    {
        if (leftClick != null) leftClick.Disable();
        if (rightClick != null) rightClick.Disable();

        // Unsubscribe to avoid leaks
        model.OnPositionChanged -= HandlePositionChanged;
        model.OnTimerUpdated -= HandleTimerUpdated;
        model.OnWin -= HandleWin;
        model.OnLose -= HandleLose;
    }

    void Update()
    {
        if (!model.IsGameActive) return;

        float dt = Time.deltaTime;

        // Handle input (left moves right in your original; right moves left)
        float dir = ReadInputDirection();
        model.ApplyInput(dir, dt);

        // Drift
        model.UpdateDrift(dt);

        // Determine whether current point (in pixels) sits within green zone
        bool isInGreen = IsPointInGreenZone();

        // Update timers and potentially trigger win/lose
        model.UpdateTimers(dt, isInGreen);

        // Update point color (visual)
        view.SetPointColor(isInGreen);
    }

    private float ReadInputDirection()
    {
        bool leftHeld = leftClick != null && leftClick.ReadValue<float>() > 0f;
        bool rightHeld = rightClick != null && rightClick.ReadValue<float>() > 0f;

        if (leftHeld && rightHeld) return 0f;
        if (leftHeld) return +1f;   // original: left click moves right
        if (rightHeld) return -1f;  // original: right click moves left
        return 0f;
    }

    private bool IsPointInGreenZone()
    {
        float zoneLeft = -greenZoneWidth / 2f;
        float zoneRight = greenZoneWidth / 2f;
        float pointX = view.CurrentPointAnchoredX; // anchoredPosition.x
        return pointX >= zoneLeft && pointX <= zoneRight;
    }

    // Model event handlers
    private void HandlePositionChanged(float normalized)
    {
        view.SetPointNormalized(normalized);
    }

    private void HandleTimerUpdated(float timeLeft, bool inGreen)
    {
        if (inGreen)
        {
            view.SetTimerText($"До раскопки: {timeLeft:F1}с", true);
        }
        else
        {
            view.SetTimerText($"До поломки: {timeLeft:F1}с", false);
        }
    }

    private void HandleWin()
    {
        // stop and show win
        view.ShowWinScreen(true);
        view.SetStatusText("УСПЕШНАЯ РАСКОПКА!");
        if (resetCoroutine != null) StopCoroutine(resetCoroutine);
        resetCoroutine = StartCoroutine(ResetAfterDelay(3f));
    }

    private void HandleLose()
    {
        view.ShowLoseScreen(true);
        view.SetStatusText("БУР СЛОМАЛСЯ!");
        if (resetCoroutine != null) StopCoroutine(resetCoroutine);
        resetCoroutine = StartCoroutine(ResetAfterDelay(3f));
    }

    private IEnumerator ResetAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        DoReset();
    }

    private void DoReset()
    {
        view.ShowWinScreen(false);
        view.ShowLoseScreen(false);
        model.Reset();
        view.SetGreenZoneWidth(greenZoneWidth);
        view.SetStatusText("Держите бур в зеленой зоне!");
    }
}