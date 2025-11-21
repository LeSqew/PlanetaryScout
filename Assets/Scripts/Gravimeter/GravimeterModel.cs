using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Структура, хранящая три ключевых параметра гравитационной волны.
/// </summary>
[Serializable]
public struct WaveParams
{
    public float Amplitude;
    public float Frequency;
    public float PhaseShift; // Фазовый сдвиг (в радианах, 0 до 2*PI)

    public WaveParams(float amplitude, float frequency, float phaseShift)
    {
        Amplitude = amplitude;
        Frequency = frequency;
        PhaseShift = phaseShift;
    }
}

/// <summary>
/// Структура для передачи всех данных из Модели в View за один раз.
/// </summary>
public struct WaveData
{
    public WaveParams PlayerParams;
    public WaveParams TargetParams;
    public float RemainingTime;      // Оставшееся время (для таймера)
    public float DataQuality;        // Качество данных (от 0.0 до 1.0)
}

public class GravimeterModel
{
    public float AnomalyTimeLimit = 15.0f; 
    private const float MATCH_TOLERANCE = 0.05f; 
    private const float MAX_AMPLITUDE = 20.0f; // Для нормализации
    private const float MAX_FREQUENCY = 3.0f;  // Для нормализации
    private const float MAX_PHASE_SHIFT = 2f * Mathf.PI; // Для нормализации

    // События (Action) остаются прежними
    public event Action<WaveData> OnWaveParametersChanged;
    public event Action OnMatchSuccess;
    public event Action OnAnomalyMissed;     

    // Свойства остаются прежними
    public bool IsMatched { get; private set; } = false;
    public WaveParams TargetParams { get; private set; }
    public WaveParams PlayerParams { get; private set; }
    public float RemainingTime { get; private set; }
    public float DataQuality { get; private set; } = 0.0f;

    // Приватные поля
    private bool _isActive = false;

    // --------------------------------------------------------
    // МЕТОДЫ ЖИЗНЕННОГО ЦИКЛА И ТАЙМЕРА (Tick)
    // --------------------------------------------------------

    /// <summary>
    /// НОВЫЙ МЕТОД: Вызывается Контроллером на каждом кадре для обновления таймера.
    /// </summary>
    public void Tick(float deltaTime)
    {
        if (!_isActive || IsMatched)
            return;

        RemainingTime -= deltaTime;

        if (RemainingTime <= 0f)
        {
            RemainingTime = 0f;
            _isActive = false;
            
            OnAnomalyMissed?.Invoke();
            SendUpdateToView();
        }

        // Отправляем данные во View (для отображения таймера и качества)
        SendUpdateToView();
    }

    public void StartMinigame(WaveParams target)
    {
        TargetParams = target;
        PlayerParams = new WaveParams(0f, 0f, 0f); 
        RemainingTime = AnomalyTimeLimit;
        IsMatched = false;
        _isActive = true;
        DataQuality = 0.0f;
        SendUpdateToView();
    }

    public void SetPlayerParams(WaveParams newParams)
    {
        if (!_isActive || IsMatched) return;

        PlayerParams = newParams;
        CheckMatchCondition();
    }

    private void CheckMatchCondition()
    {
        // --- Логика расчета качества данных (DataQuality) ---
        float ampError = Mathf.Abs(PlayerParams.Amplitude - TargetParams.Amplitude) / MAX_AMPLITUDE;
        float freqError = Mathf.Abs(PlayerParams.Frequency - TargetParams.Frequency) / MAX_FREQUENCY;
        float phaseError = Mathf.Abs(PlayerParams.PhaseShift - TargetParams.PhaseShift) / MAX_PHASE_SHIFT;

        float averageError = (ampError + freqError + phaseError) / 3f;
        DataQuality = Mathf.Clamp01(1.0f - averageError);

        // --- Логика проверки совпадения ---
        bool ampMatch = Mathf.Abs(PlayerParams.Amplitude - TargetParams.Amplitude) <= MATCH_TOLERANCE;
        bool freqMatch = Mathf.Abs(PlayerParams.Frequency - TargetParams.Frequency) <= MATCH_TOLERANCE;
        bool phaseMatch = Mathf.Abs(PlayerParams.PhaseShift - TargetParams.PhaseShift) <= MATCH_TOLERANCE; 

        bool newMatchState = ampMatch && freqMatch && phaseMatch;

        if (newMatchState && !IsMatched)
        {
            IsMatched = true;
            _isActive = false;
            OnMatchSuccess?.Invoke();
        }

        SendUpdateToView();
    }
    
    private void SendUpdateToView()
    {
        OnWaveParametersChanged?.Invoke(new WaveData
        {
            PlayerParams = PlayerParams,
            TargetParams = TargetParams,
            RemainingTime = RemainingTime,
            DataQuality = DataQuality
        });
    }
        
}
