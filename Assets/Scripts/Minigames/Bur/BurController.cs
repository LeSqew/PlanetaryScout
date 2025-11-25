using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class BurController : MonoBehaviour, IMinigameController
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
    public RectTransform containerRect;
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
    
    public bool RequiresInputBlocking => true;

    private BurModel model;
    private BurView view;
    private ScannableObject _currentTarget;
    private Coroutine resetCoroutine;
    private bool _isCompleted = false;
    private Action<bool, ScannableObject> _onFinishedCallback;

    public void StartAnalysis(ScannableObject target, Action<bool, ScannableObject> onFinishedCallback)
    {
        if (_isCompleted) return;

        _currentTarget = target;
        _onFinishedCallback = onFinishedCallback;

        // Настройка под редкость (опционально)
        float rarityMultiplier = 1f + (target.rarity - 1) * 0.2f;
        float adjustedWinTime = winTime / rarityMultiplier;
        float adjustedLoseTime = loseTime / rarityMultiplier;

        // Инициализация модели
        model = new BurModel(moveSpeed, driftStrength, driftChangeFrequency,
            adjustedWinTime, adjustedLoseTime, 0.5f);

        // Инициализация View
        //containerRect = GetComponent<RectTransform>();
        view = new BurView(containerRect, movingPoint, greenZone, pointImage,
            winScreen, loseScreen, timerText, statusText);

        // Подписка на события
        model.OnPositionChanged += HandlePositionChanged;
        model.OnTimerUpdated += HandleTimerUpdated;
        model.OnWin += HandleWin;
        model.OnLose += HandleLose;

        // Ввод
        if (leftClickAction != null) { leftClick = leftClickAction.action; leftClick.Enable(); }
        if (rightClickAction != null) { rightClick = rightClickAction.action; rightClick.Enable(); }

        // UI
        view.SetGreenZoneWidth(greenZoneWidth);
        view.SetPointNormalized(model.Position);
        view.ShowWinScreen(false);
        view.ShowLoseScreen(false);
        view.SetStatusText("Держите бур в зеленой зоне!");

        gameObject.SetActive(true);
    }

    public void Cleanup()
    {
        _isCompleted = true;

        if (leftClick != null) leftClick.Disable();
        if (rightClick != null) rightClick.Disable();

        model.OnPositionChanged -= HandlePositionChanged;
        model.OnTimerUpdated -= HandleTimerUpdated;
        model.OnWin -= HandleWin;
        model.OnLose -= HandleLose;

        Destroy(gameObject);
        Destroy(_currentTarget.gameObject);
    }

    void Update()
    {
        if (model == null || !_isCompleted == false || !model.IsGameActive) return;

        float dt = Time.deltaTime;
        float dir = ReadInputDirection();
        model.ApplyInput(dir, dt);
        model.UpdateDrift(dt);

        bool isInGreen = IsPointInGreenZone();
        model.UpdateTimers(dt, isInGreen);
        view.SetPointColor(isInGreen);
    }

    private float ReadInputDirection()
    {
        bool leftHeld = leftClick != null && leftClick.ReadValue<float>() > 0f;
        bool rightHeld = rightClick != null && rightClick.ReadValue<float>() > 0f;

        if (leftHeld && rightHeld) return 0f;
        if (leftHeld) return +1f;
        if (rightHeld) return -1f;
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
        if (_isCompleted) return;
        _isCompleted = true;

        view.ShowWinScreen(true);
        view.SetStatusText("УСПЕШНАЯ РАСКОПКА!");
        _onFinishedCallback?.Invoke(true, _currentTarget);
        StartCoroutine(DelayedCleanup(2f));
    }

    private void HandleLose()
    {
        if (_isCompleted) return;
        _isCompleted = true;

        view.ShowLoseScreen(true);
        view.SetStatusText("БУР СЛОМАЛСЯ!");
        _onFinishedCallback?.Invoke(false, _currentTarget);
        StartCoroutine(DelayedCleanup(2f));
    }

    private IEnumerator DelayedCleanup(float delay)
    {
        yield return new WaitForSeconds(delay);
        Cleanup();
    }
}