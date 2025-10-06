using System;
using Minigames.Spectrometer;
using UnityEngine;
using UnityEngine.InputSystem;

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

        // Подписка на UI события
        view.OnSliderChanged += HandleFiltersChanged;
        view.OnConfirmPressed += HandleConfirm;

        // Настройка Input System
        if (inputActions != null)
        {
            var map = inputActions.FindActionMap("Spectrometer", true);
            _confirm = map.FindAction("Confirm", true);
        }
    }
    
    private void OnEnable()
    {
        _confirm?.Enable();
    }

    private void OnDisable()
    {
        _confirm?.Disable();
    }

    private void HandleFiltersChanged(float r, float g, float b, float uv)
    {
        _model.SetFilters(r, g, b, uv);
        float accuracy = _model.CalculateAccuracy();
        view.RenderSpectrum(_model.GetMeasuredSpectrum(), _model.GetTargetSpectrum());
        view.SetAccuracy(accuracy);
        OnAccuracyChanged?.Invoke(accuracy);
    }
    private void HandleConfirm()
    {
        if (_model.TryGetResult(out string result))
        {
            view.ShowResult(result);
            OnAnalysisCompleted?.Invoke(result);
        }
    }
}
