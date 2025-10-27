using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct WaveParameters
{
    public float Amplitude;     // Амплитуда
    public float Frequency;     // Частота
    public float PhaseShift;    // Сдвиг по фазе (в радианах)
}

// Класс для передачи данных о графиках в View
public class WaveData
{
    public WaveParameters TargetParams;
    public WaveParameters PlayerParams;
    public List<float> XValues;
    public List<float> TargetYValues;
    public List<float> PlayerYValues;
}

public class GravimeterModel
{
    // Событие: Изменились данные волны игрока (View должен перерисовать графики)
    public event Action<WaveData> OnWaveDataChanged;
    // Событие: Игрок успешно подогнал графики (Победа)
    public event Action OnMatchSuccess;
    
    private WaveParameters _targetWaveParams;
    private WaveParameters _playerWaveParams;
    
    private const float MatchTolerance = 0.05f; // 5% допуск
    private const int TotalPoints = 200;        // Количество точек для построения графика
    private const float PlotDuration = 2.0f;    // Длительность по оси X
    
    public bool IsMatched { get; private set; }
    
    public GravimeterModel()
    {
        // Установка целевых параметров
        _targetWaveParams = new WaveParameters {
            Amplitude = 10.0f,
            Frequency = 1.5f,
            PhaseShift = 0.8f // Радианы
        };

        // Начальные параметры игрока (отличные от цели)
        _playerWaveParams = new WaveParameters {
            Amplitude = 5.0f,
            Frequency = 1.0f,
            PhaseShift = 0.0f
        };
    }
    
    /// <summary>
    /// Основная математическая функция волны: Y(t) = A * sin(2*pi*f*t + phi)
    /// </summary>
    public float GetWavePoint(float t, WaveParameters p)
    {
        return p.Amplitude * (float)Math.Sin(2 * Math.PI * p.Frequency * t + p.PhaseShift);
    }

    public void SetPlayerAmplitude(float value)
    {
        _playerWaveParams.Amplitude = value;
        CheckAndNotify();
    }

    public void SetPlayerFrequency(float value)
    {
        _playerWaveParams.Frequency = value;
        CheckAndNotify();
    }

    public void SetPlayerPhaseShift(float value)
    {
        // Нормализация фазы в пределах [0, 2*PI]
        _playerWaveParams.PhaseShift = value % (2.0f * (float)Math.PI);
        if (_playerWaveParams.PhaseShift < 0)
        {
            _playerWaveParams.PhaseShift += 2.0f * (float)Math.PI;
        }
        CheckAndNotify();
    }

    private void CheckAndNotify()
    {
        // 1. Проверка победы
        if (!IsMatched)
        {
            IsMatched = CheckMatchCondition();
            if (IsMatched)
            {
                OnMatchSuccess?.Invoke();
            }
        }

        // 2. Уведомление View о новых данных
        OnWaveDataChanged?.Invoke(GenerateWaveData());
    }

    private WaveData GenerateWaveData()
    {
        var data = new WaveData {
            TargetParams = _targetWaveParams,
            PlayerParams = _playerWaveParams,
            XValues = new List<float>(TotalPoints),
            TargetYValues = new List<float>(TotalPoints),
            PlayerYValues = new List<float>(TotalPoints)
        };

        for (int i = 0; i < TotalPoints; i++)
        {
            float t = i * PlotDuration / TotalPoints;
            data.XValues.Add(t);
            data.TargetYValues.Add(GetWavePoint(t, _targetWaveParams));
            data.PlayerYValues.Add(GetWavePoint(t, _playerWaveParams));
        }

        return data;
    }

    private bool CheckMatchCondition()
    {
        // Сравниваем параметры с учетом допуска
        bool ampMatch = Math.Abs(_targetWaveParams.Amplitude - _playerWaveParams.Amplitude) < _targetWaveParams.Amplitude * MatchTolerance;
        bool freqMatch = Math.Abs(_targetWaveParams.Frequency - _playerWaveParams.Frequency) < _targetWaveParams.Frequency * MatchTolerance;
        
        // Проверка фазы с учетом 2*PI цикла
        float phaseDiff = Math.Abs(_targetWaveParams.PhaseShift - _playerWaveParams.PhaseShift);
        bool phaseMatch = phaseDiff < MatchTolerance || Math.Abs(phaseDiff - 2.0f * (float)Math.PI) < MatchTolerance;

        return ampMatch && freqMatch && phaseMatch;
    }
}
