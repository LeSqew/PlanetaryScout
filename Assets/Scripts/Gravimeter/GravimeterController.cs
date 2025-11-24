using System.Collections;
using System.Linq;
using UnityEngine;

public class GravimeterController : MonoBehaviour
{
    [SerializeField] private GravimeterView view;
    private GravimeterModel _model;
    private ScannableObject _currentTarget;
    private bool _isCompleted = false;

    private float _lastUIUpdateTime = 0f;
    private const float UI_UPDATE_INTERVAL = 0.05f;

    // private GravimeterModel Model
    // {
    //     get
    //     {
    //         if (_model == null)
    //             _model = new GravimeterModel();
    //         return _model;
    //     }
    // }

    void Update()
    {
        if (_model == null || _isCompleted) return;

        _model.Tick(Time.deltaTime);

        // Провал по таймеру
        if (_model.HasFailed)
        {
            OnFailure("Время вышло!");
            return;
        }

        // Обновляем UI
        if (Time.time - _lastUIUpdateTime >= UI_UPDATE_INTERVAL)
        {
            UpdateView();
            _lastUIUpdateTime = Time.time;
        }
    }

    public void StartAnalysis(ScannableObject target)
    {
        if (gameObject.activeSelf) return;
        
        _isCompleted = false;
        gameObject.SetActive(true);
        _currentTarget = target;

        float amplitude = 10f + _currentTarget.rarity * 2.5f;
        float frequency = Mathf.Max(0.5f, 1f + _currentTarget.rarity * 0.3f);
        float phaseShift = _currentTarget.rarity * 1.0f;
        float requiredAccuracy = GetRequiredAccuracy(_currentTarget.rarity);

        _model = new GravimeterModel();
        _model.StartMinigame(new WaveParams(amplitude, frequency, phaseShift), requiredAccuracy);

        MinigameManager.Instance.EnterMinigame();
        UpdateView();
    }
    
    private float GetRequiredAccuracy(int rarity)
    {
        return rarity switch
        {
            1 => 0.85f,
            2 => 0.90f,
            3 => 0.95f,
            4 => 0.98f,
            _ => 0.85f
        };
    }
    
    private void UpdateView()
    {
        if (view != null && _model != null && !_isCompleted)
        {
            view.SetWaveData(
                _model.TargetParams,
                _model.PlayerParams,
                _model.RemainingTime,
                _model.DataQuality
            );
        }
    }
    
    public void OnConfirmPressed()
    {
        if (_model == null || _isCompleted || _model.HasFailed) return;

        if (_model.IsAccuracySufficient())
        {
            OnSuccess();
        }
        else
        {
            OnFailure("Недостаточная точность!");
        }
    }
    void OnSuccess()
    {
        if (_isCompleted || _currentTarget == null) return;
        _isCompleted = true;
        view?.ShowSuccess();
        
    
        var category = _currentTarget.category;
        _currentTarget.OnScanCompleted(); // ← Destroy внутри
        _currentTarget = null;
        int remaining = FindObjectsOfType<ScannableObject>().Count(obj => obj != null && obj.category == category) - 1;
        DataCollectionEvents.RaiseObjectDestroyed(category, remaining);
        StartCoroutine(DeactivateAfterFrame());
    }

    void OnFailure(string reason)
    {
        if (_currentTarget == null) return;

        var category = _currentTarget.category;
        Destroy(_currentTarget.gameObject);
        _currentTarget = null;
        view?.ShowFailure();
        int remaining = FindObjectsOfType<ScannableObject>().Count(obj => obj != null && obj.category == category) - 1;
        DataCollectionEvents.RaiseObjectDestroyed(category, remaining);
        StartCoroutine(DeactivateAfterFrame());
    }
    private IEnumerator DeactivateAfterFrame()
    {
        yield return null;

        MinigameManager.Instance.ExitMinigame();
        gameObject.SetActive(false);
    }


    public void SetAmplitude(float value) => UpdatePlayerParam(value, _model.PlayerParams.Frequency, _model.PlayerParams.PhaseShift);
    public void SetFrequency(float value) => UpdatePlayerParam(_model.PlayerParams.Amplitude, value, _model.PlayerParams.PhaseShift);
    public void SetPhaseShift(float value) => UpdatePlayerParam(_model.PlayerParams.Amplitude, _model.PlayerParams.Frequency, value);

    private void UpdatePlayerParam(float a, float f, float p)
    {
        if (_model != null && !_isCompleted)
        {
            _model.SetPlayerParams(new WaveParams(a, f, p));
            UpdateView();
        }
    }
}