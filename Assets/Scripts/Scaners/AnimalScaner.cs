using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Контроллер сканера животных. 
/// Реализует механику удержания прицела с учетом инерционной тряски и затухания прогресса.
/// </summary>
public class AnimalScanner : MonoBehaviour, IMinigameController
{
    [Header("Base Scan Settings")]
    [SerializeField] private float scanTime = 2f;
    [SerializeField] private float scanDistance = 4f;
    [SerializeField] private Image scanProgressCircle;
    [SerializeField] private LayerMask scanLayerMask;

    [Header("Difficulty Settings")]
    [Tooltip("Скорость потери прогресса при потере цели")]
    [SerializeField] private float baseStabilityDecay = 0.2f; 
    [Tooltip("Базовая интенсивность дрейфа прицела")]
    [SerializeField] private float baseShakeIntensity = 0.007f; 

    [Header("Shake Smoothing (Eye Comfort)")]
    [Tooltip("Насколько плавно прицел стремится к точке смещения")]
    [SerializeField] private float shakeSmoothSpeed = 8f; 
    [Tooltip("Как часто выбирается новая точка дрейфа")]
    [SerializeField] private float shakeUpdateFrequency = 0.15f; 
    [Tooltip("Множитель для визуального смещения кружка в UI")]
    [SerializeField] private float uiVisualMultiplier = 600f;

    private Camera _mainCamera;
    private ScannableObject _target;
    private float _progress = 0f;
    private bool _isCompleted = false;
    private Action<bool, ScannableObject> _onFinishedCallback;

    // Внутренние переменные для плавности
    private Vector3 _currentShakeOffset;
    private Vector3 _targetShakeOffset;
    private float _shakeTimer;

    public bool RequiresInputBlocking => false;

    void Awake()
    {
        _mainCamera = Camera.main;
        if (_mainCamera == null) Debug.LogError("AnimalScanner: Main Camera not found!");
    }

    void Start()
    {
        if (scanProgressCircle != null)
        {
            scanProgressCircle.fillAmount = 0f;
            // Центрируем кружок через RectTransform
            scanProgressCircle.rectTransform.anchoredPosition = Vector2.zero;
        }
    }

    void Update()
    {
        if (_isCompleted || _target == null || _mainCamera == null) return;

        bool isHitting = IsCrosshairHittingTarget();
        bool isPressing = Input.GetMouseButton(0);
        int rarity = _target.rarity;

        if (isPressing && isHitting)
        {
            // Успешное сканирование
            _progress += Time.deltaTime / scanTime;

            // Обновляем целевую точку дрейфа по таймеру
            _shakeTimer -= Time.deltaTime;
            if (_shakeTimer <= 0)
            {
                // Рассчитываем силу тряски на основе редкости
                float currentIntensity = baseShakeIntensity * (1f + (rarity - 1) * 0.6f);
                _targetShakeOffset = new Vector3(
                    UnityEngine.Random.Range(-1f, 1f),
                    UnityEngine.Random.Range(-1f, 1f),
                    0) * currentIntensity;

                _shakeTimer = shakeUpdateFrequency;
            }
        }
        else
        {
            // Прогресс "тает", если цель потеряна или кнопка отпущена
            float currentDecay = baseStabilityDecay * rarity;
            _progress -= Time.deltaTime * currentDecay;

            // Возвращаем прицел в центр (стабилизация)
            _targetShakeOffset = Vector3.zero;
        }

        // Плавное движение текущего смещения к целевому (Lerp)
        _currentShakeOffset = Vector3.Lerp(_currentShakeOffset, _targetShakeOffset, Time.deltaTime * shakeSmoothSpeed);

        _progress = Mathf.Clamp01(_progress);

        UpdateUI(isHitting);

        if (_progress >= 1f) OnSuccess();
    }

    private void UpdateUI(bool isHitting)
    {
        if (scanProgressCircle == null) return;

        scanProgressCircle.fillAmount = _progress;
        
        // Цвет индикатора
        scanProgressCircle.color = isHitting ? Color.green : Color.red;

        // Визуальное смещение кружка (синхронизировано с физикой луча)
        scanProgressCircle.rectTransform.anchoredPosition = _currentShakeOffset * uiVisualMultiplier;
    }

    private bool IsCrosshairHittingTarget()
    {
        // Пускаем луч со смещением, которое видит игрок на кружке
        Vector3 screenPoint = new Vector3(0.5f, 0.5f, 0) + _currentShakeOffset;
        Ray ray = _mainCamera.ViewportPointToRay(screenPoint);

        if (Physics.Raycast(ray, out RaycastHit hit, scanDistance, scanLayerMask))
        {
            return hit.collider.gameObject == _target.gameObject;
        }

        return false;
    }

    public void StartAnalysis(ScannableObject target, Action<bool, ScannableObject> onFinishedCallback)
    {
        _target = target;
        _onFinishedCallback = onFinishedCallback;
        _isCompleted = false;
        _progress = 0f;
        _targetShakeOffset = Vector3.zero;
        _currentShakeOffset = Vector3.zero;

        // --- НАСТРОЙКА СЛОЖНОСТИ ---
        int r = _target.rarity;
        this.scanTime = 2f + (r - 1) * 1.5f; // Больше редкость — дольше скан
        this.scanDistance = 4f - (r - 1) * 0.5f; // Больше редкость — нужно быть ближе

        if (scanProgressCircle != null) scanProgressCircle.fillAmount = 0f;
        gameObject.SetActive(true);
    }

    public void Cleanup()
    {
        _isCompleted = true;
        _target = null;
        _onFinishedCallback = null;
        if (this != null) Destroy(gameObject);
    }

    private void OnSuccess()
    {
        if (_isCompleted) return;
        _isCompleted = true;
        _onFinishedCallback?.Invoke(true, _target);
    }
}