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

    private float _currentGreenZoneWidth;
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
    
        // --- КОМПЛЕКСНАЯ НАСТРОЙКА СЛОЖНОСТИ ---
        int r = target.rarity; // 1, 2, 3 или 4
    
        // 1. СИЛА ДРЕЙФА: На r4 тянет в 2 раза сильнее, чем на r1
        float adjustedDriftStrength = driftStrength * (1f + (r - 1) * 0.35f);
    
        // 2. ЧАСТОТА РЫВКОВ: На r4 направление меняется в 2.5 раза чаще
        float adjustedDriftFreq = driftChangeFrequency * (1f + (r - 1) * 0.5f);
    
        // 3. ЗЕЛЕНАЯ ЗОНА: На r4 зона на 40% уже, чем на r1
        float adjustedGreenZoneWidth = greenZoneWidth / (1f + (r - 1) * 0.2f);
        // Сохраняем это значение в локальную переменную, чтобы использовать в IsPointInGreenZone
        _currentGreenZoneWidth = adjustedGreenZoneWidth; 
    
        // 4. ТАЙМЕРЫ: Чем выше редкость, тем дольше бурить и тем быстрее ломается
        float adjustedWinTime = winTime + (r - 1) * 2f; 
        float adjustedLoseTime = Mathf.Max(1.5f, loseTime - (r - 1) * 1f);
    
        // Инициализация модели
        model = new BurModel(
            moveSpeed, 
            adjustedDriftStrength, 
            adjustedDriftFreq,
            adjustedWinTime, 
            adjustedLoseTime, 
            0.5f);
    
        // Инициализация View
        view = new BurView(containerRect, movingPoint, greenZone, pointImage,
            winScreen, loseScreen, timerText, statusText);
    
        // Подписки на события
        model.OnPositionChanged += HandlePositionChanged;
        model.OnTimerUpdated += HandleTimerUpdated;
        model.OnWin += HandleWin;
        model.OnLose += HandleLose;
    
        // Ввод
        if (leftClickAction != null) { leftClick = leftClickAction.action; leftClick.Enable(); }
        if (rightClickAction != null) { rightClick = rightClickAction.action; rightClick.Enable(); }
    
        // Настройка интерфейса под новую сложность
        view.SetGreenZoneWidth(adjustedGreenZoneWidth);
        view.SetPointNormalized(model.Position);
        view.ShowWinScreen(false);
        view.ShowLoseScreen(false);
        view.SetStatusText(r > 2 ? "!!! ВНИМАНИЕ: СИЛЬНАЯ ВИБРАЦИЯ !!!" : "Держите бур в зеленой зоне!");
    
        gameObject.SetActive(true);
    }
    
    private bool IsPointInGreenZone()
    {
        // Используем динамически рассчитанную ширину вместо стандартной
        float zoneLeft = -_currentGreenZoneWidth / 2f;
        float zoneRight = _currentGreenZoneWidth / 2f;
        float pointX = view.CurrentPointAnchoredX;
        return pointX >= zoneLeft && pointX <= zoneRight;
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
        //Destroy(_currentTarget.gameObject);
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
        view.SetStatusText("Успешная раскопка!");
        _onFinishedCallback?.Invoke(true, _currentTarget);
        StartCoroutine(DelayedCleanup(2f));
    }

    private void HandleLose()
    {
        if (_isCompleted) return;
        _isCompleted = true;

        view.ShowLoseScreen(true);
        view.SetStatusText("Бур сломался!");
        _onFinishedCallback?.Invoke(false, _currentTarget);
        StartCoroutine(DelayedCleanup(2f));
    }

    private IEnumerator DelayedCleanup(float delay)
    {
        yield return new WaitForSeconds(delay);
        Cleanup();
    }
}