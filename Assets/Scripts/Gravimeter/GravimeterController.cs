using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class GravimeterController : MonoBehaviour, IMinigameController
{
    [SerializeField] private GravimeterView view;
    private GravimeterModel _model;
    private ScannableObject _currentTarget;
    private bool _isCompleted = false;
    public bool RequiresInputBlocking => true;

    private float _lastUIUpdateTime = 0f;
    private const float UI_UPDATE_INTERVAL = 0.05f;
    private Action<bool, ScannableObject> _onFinishedCallback;

    void Update()
    {
        if (_model == null || _isCompleted) return;

        _model.Tick(Time.deltaTime);

        if (_model.HasFailed)
        {
            OnFailure("Время вышло!");
            return;
        }

        if (Time.time - _lastUIUpdateTime >= UI_UPDATE_INTERVAL)
        {
            UpdateView();
            _lastUIUpdateTime = Time.time;
        }
    }

    public void StartAnalysis(ScannableObject target, Action<bool, ScannableObject> onFinishedCallback)
    {
        _onFinishedCallback = onFinishedCallback;
        _currentTarget = target;
        _isCompleted = false;

        int r = _currentTarget.rarity;

        float amplitude = UnityEngine.Random.Range(10f, 20f);
        float frequency = UnityEngine.Random.Range(0.5f, 1f + r * 0.5f); 
        float phaseShift = UnityEngine.Random.Range(0f, 2f * Mathf.PI);
    
        // 2. Время на сканирование (чем выше редкость, тем меньше времени)
        // r1 = 20s, r2 = 15s, r3 = 10s, r4 = 7s
        float timeLimit = 25f - (r * 5f); 

        float requiredAccuracy = GetRequiredAccuracy(r);
    
        _model = new GravimeterModel();
        _model.AnomalyTimeLimit = timeLimit; 
        
        _model.StartMinigame(new WaveParams(amplitude, frequency, phaseShift), requiredAccuracy);
        
        gameObject.SetActive(true);
        UpdateView();
    }
    
    private float GetRequiredAccuracy(int rarity)
    {
        return rarity switch
        {
            1 => 0.85f,
            2 => 0.90f,
            3 => 0.94f,
            4 => 0.97f,
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
        if (_currentTarget == null) return;

        _isCompleted = true;
        view?.ShowSuccess();

        _onFinishedCallback?.Invoke(true, _currentTarget);
        Cleanup();
        //StartCoroutine(ShowResultAndCleanup());
    }

    void OnFailure(string reason)
    {
        if (_currentTarget == null) return;

        _isCompleted = true;
        view?.ShowFailure();

        _onFinishedCallback?.Invoke(false, _currentTarget); 

        Cleanup();

        //StartCoroutine(ShowResultAndCleanup());
    }

    public void Cleanup() 
    {
        StopAllCoroutines();
        _model = null;
        
        _isCompleted = false;
        gameObject.SetActive(false);
        Destroy(gameObject);
        //_currentTarget = null;
    }

    public void SetAmplitude(float value)
    {
        if (_model == null) return; 
        UpdatePlayerParam(value, _model.PlayerParams.Frequency, _model.PlayerParams.PhaseShift);
    }

    public void SetFrequency(float value)
    {
        if (_model == null) return; 
        UpdatePlayerParam(_model.PlayerParams.Amplitude, value, _model.PlayerParams.PhaseShift);
    }

    public void SetPhaseShift(float value)
    {
        if (_model == null) return; 
        UpdatePlayerParam(_model.PlayerParams.Amplitude, _model.PlayerParams.Frequency, value);
    }
    private void UpdatePlayerParam(float a, float f, float p)
    {
        if (_model != null && !_isCompleted)
        {
            _model.SetPlayerParams(new WaveParams(a, f, p));
            UpdateView();
        }
    }
}