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
    public InputActionAsset inputActions;

    private SpectrometerModel _model;

    public event Action<float> OnAccuracyChanged;
    public event Action<string> OnAnalysisCompleted;

    private InputAction _confirm;
    
    private void Awake()
    {
        _model = new SpectrometerModel();

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
            OnAnalysisCompleted?.Invoke(result);
        }
        else
        {
            view.ShowResult("Недостаточная точность. Попробуйте подобрать цвет точнее.");
        }
    }
}
