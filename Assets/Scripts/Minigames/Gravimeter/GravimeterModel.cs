using UnityEngine;

[System.Serializable]
public struct WaveParams
{
    public float Amplitude;
    public float Frequency;
    public float PhaseShift;

    public WaveParams(float amplitude, float frequency, float phaseShift)
    {
        Amplitude = amplitude;
        Frequency = frequency;
        PhaseShift = phaseShift;
    }
}

public class GravimeterModel
{
    public float AnomalyTimeLimit = 15.0f;
    private const float MAX_AMPLITUDE = 20.0f;
    private const float MAX_FREQUENCY = 3.0f;
    private const float MAX_PHASE_SHIFT = 2f * Mathf.PI;

    public WaveParams TargetParams { get; private set; }
    public WaveParams PlayerParams { get; private set; }
    public float RemainingTime { get; private set; }
    public float DataQuality { get; private set; }
    private bool _isActive;
    private float _requiredAccuracy;

    public bool HasFailed => RemainingTime <= 0f;

    public void StartMinigame(WaveParams target, float requiredAccuracy)
    {
        TargetParams = target;
        PlayerParams = new WaveParams(0f, 1.5f, 0f); // видимый старт
        RemainingTime = AnomalyTimeLimit;
        _requiredAccuracy = requiredAccuracy;
        _isActive = true;
        CalculateDataQuality();
    }

    public void SetPlayerParams(WaveParams newParams)
    {
        if (!_isActive) return;
        PlayerParams = newParams;
        CalculateDataQuality();
    }

    public void Tick(float deltaTime)
    {
        if (!_isActive) return;
        RemainingTime -= deltaTime;
        if (RemainingTime < 0) RemainingTime = 0;
    }

    private void CalculateDataQuality()
    {
        float ampError = Mathf.Abs(PlayerParams.Amplitude - TargetParams.Amplitude) / MAX_AMPLITUDE;
        float freqError = Mathf.Abs(PlayerParams.Frequency - TargetParams.Frequency) / MAX_FREQUENCY;
        float phaseError = Mathf.Abs(PlayerParams.PhaseShift - TargetParams.PhaseShift) / MAX_PHASE_SHIFT;
        float avgError = (ampError + freqError + phaseError) / 3f;
        DataQuality = Mathf.Clamp01(1f - avgError);
    }

    // Новый метод: проверка точности ВНЕ модели
    public bool IsAccuracySufficient() => DataQuality >= _requiredAccuracy;
}