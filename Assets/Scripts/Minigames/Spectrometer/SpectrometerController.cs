using System;
using Minigames.Spectrometer;
using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
/// Контроллер для мини-игры "Спектрометр".
/// Отвечает за связь между пользовательским вводом (View) и игровой логикой (Model).
/// Управляет потоком данных, расчетом точности и обработкой подтверждения.
public class SpectrometerController : MonoBehaviour
{
    public SpectrometerView view;
    public InputActionAsset inputActions; // ваш InputActionAsset
    private InputActionMap _playerMap;

    private SpectrometerModel _model;
    private ScannableObject _currentTarget;

    public event Action<float> OnAccuracyChanged;
    public event Action<string> OnAnalysisCompleted;

    private InputAction _confirm;
    
    private void Awake()
    {
        _playerMap = inputActions.FindActionMap("Player", true);

        view.OnSliderChanged += HandleFiltersChanged;
        view.OnConfirmPressed += HandleConfirm;

        if (inputActions != null)
        {
            var map = inputActions.FindActionMap("Spectrometer", true);
            _confirm = map.FindAction("Confirm", true);
        }
    }
    
    private void OnEnable()
    {
        _confirm.Enable();
    }

    private void OnDisable()
    {
        _confirm.Disable();
    }
    
    public void StartAnalysis(ScannableObject target)
    {
        if (gameObject.activeSelf) return;
        _currentTarget = target;
        _model = new SpectrometerModel();
        _model.InitializeFromObject(target.category, target.rarity); // ← ключевое
        Debug.Log("Активируем Canvas: " + gameObject.name);
        gameObject.SetActive(true);
        _playerMap.Disable(); // ← ключевая строка

        // Показываем и разблокируем курсор
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    /// <summary>
    /// Обрабатывает событие изменения любого ползунка (R, G, B, UV).
    /// </summary>
    private void HandleFiltersChanged(float r, float g, float b, float uv)
    {
        _model.SetFilters(r, g, b, uv);
        float accuracy = _model.CalculateAccuracy();
        view.RenderColors(_model.GetMeasuredColor(), _model.GetTargetColor());
        view.SetAccuracy(accuracy);
        OnAccuracyChanged?.Invoke(accuracy);
    }
    
    /// <summary>
    /// Обрабатывает нажатие кнопки "Подтвердить" (UI или Input System).
    /// </summary>
    private void HandleConfirm()
    {
        if (_model.TryGetResult(out string result))
        {
            view.ShowResult(result);
            // OnAnalysisCompleted?.Invoke(result);
            // _currentTarget?.OnScanCompleted();
            
            DataCollectionEvents.RaiseDataCollected(new ScanResult {
                category = _currentTarget.category,
                rarity = _currentTarget.rarity,
                success = true
            });

        }
        else
        {
            view.ShowResult("Недостаточная точность. Попробуйте подобрать цвет точнее.");
        }
        gameObject.SetActive(false);
        _playerMap.Enable();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
