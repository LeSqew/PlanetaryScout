using System;
using Minigames.Spectrometer;
using Player.Health;
using UnityEngine;


/// <summary>
/// Контроллер для мини-игры "Спектрометр".
/// Отвечает за связь между пользовательским вводом (View) и игровой логикой (Model).
/// Управляет потоком данных, расчетом точности и обработкой подтверждения.
public class SpectrometerController : MonoBehaviour
{
    public SpectrometerView view;
    private HealthController _playerHealth;

    private SpectrometerModel _model;
    private ScannableObject _currentTarget;

    public event Action<float> OnAccuracyChanged;

    //private InputAction _confirm;
    
    private void Awake()
    {
        view.OnSliderChanged += HandleFiltersChanged;
        view.OnConfirmPressed += HandleConfirm;
        _playerHealth = FindObjectOfType<HealthController>();
        if (_playerHealth == null)
        {
            Debug.LogError("HealthController не найден на сцене!");
        }
    }
    
    public void StartAnalysis(ScannableObject target)
    {
        if (gameObject.activeSelf) return;

        _currentTarget = target;
        _model = new SpectrometerModel();
        _model.InitializeFromObject(target.category, target.rarity);
        
        view.ResetUI();
        
        view.RenderColors(
            _model.GetMeasuredColor(), 
            _model.GetTargetColor()
        );
        
        MinigameManager.Instance.EnterMinigame(); // ← единая точка входа
        gameObject.SetActive(true);
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
            
            _currentTarget.OnScanCompleted();
        }
        else
        {
            view.ShowResult("Ошибка анализа! Спектрометр дал сбой.");
            _playerHealth.takeDamage?.Invoke(25);
        }
        MinigameManager.Instance.ExitMinigame();
        gameObject.SetActive(false);
    }
}
