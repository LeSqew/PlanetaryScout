using System;
using UnityEngine;

public class BurModel
{
    // Параметры (immutable after ctor)
    public readonly float MoveSpeed;
    public readonly float DriftStrength;
    public readonly float DriftChangeFrequency;
    public readonly float WinTime;
    public readonly float LoseTime;

    // Состояние
    private float currentPosition; // 0..1
    private float currentDrift;
    private float driftTimer; // seconds until next drift change
    private float timeInGreen;
    private float timeInRed;
    private bool gameActive = true;

    // События
    public event Action<float> OnPositionChanged; // normalized 0..1
    public event Action<bool> OnZoneChanged; // isInGreen
    public event Action<float, bool> OnTimerUpdated; // timeLeft, isInGreen
    public event Action OnWin;
    public event Action OnLose;

    public BurModel(
        float moveSpeed,
        float driftStrength,
        float driftChangeFrequency,
        float winTime,
        float loseTime,
        float startPosition = 0.5f)
    {
        MoveSpeed = moveSpeed;
        DriftStrength = driftStrength;
        DriftChangeFrequency = Mathf.Max(0.0001f, driftChangeFrequency);
        WinTime = winTime;
        LoseTime = loseTime;

        currentPosition = Mathf.Clamp01(startPosition);
        driftTimer = 1f / DriftChangeFrequency;
    }

    public float Position => currentPosition;
    public bool IsGameActive => gameActive;

    public void Reset()
    {
        currentPosition = 0.5f;
        currentDrift = 0f;
        driftTimer = 1f / DriftChangeFrequency;
        timeInGreen = 0f;
        timeInRed = 0f;
        gameActive = true;

        OnPositionChanged?.Invoke(currentPosition);
    }

    // Apply player-driven movement: direction is -1..1 (negative = left, positive = right)
    public void ApplyInput(float direction, float deltaTime)
    {
        if (!gameActive) return;
        if (Mathf.Approximately(direction, 0f)) return;

        float delta = direction * MoveSpeed * deltaTime;
        SetPositionInternal(currentPosition + delta);
    }

    // Drift update: automatically changes drift every 1/driftFrequency seconds
    public void UpdateDrift(float deltaTime)
    {
        if (!gameActive) return;

        driftTimer -= deltaTime;
        if (driftTimer <= 0f)
        {
            currentDrift = UnityEngine.Random.Range(-1f, 1f) * DriftStrength;
            driftTimer = 1f / DriftChangeFrequency;
        }

        if (!Mathf.Approximately(currentDrift, 0f))
        {
            SetPositionInternal(currentPosition + currentDrift * deltaTime);
        }
    }

    // Controller should call this each frame with whether the point is currently in green
    public void UpdateTimers(float deltaTime, bool isInGreen)
    {
        if (!gameActive) return;

        if (isInGreen)
        {
            timeInGreen += deltaTime;
            timeInRed = 0f;

            float timeLeft = Mathf.Max(0f, WinTime - timeInGreen);
            OnTimerUpdated?.Invoke(timeLeft, true);

            if (timeInGreen >= WinTime)
            {
                gameActive = false;
                OnWin?.Invoke();
            }
        }
        else
        {
            timeInRed += deltaTime;
            timeInGreen = 0f;

            float timeLeft = Mathf.Max(0f, LoseTime - timeInRed);
            OnTimerUpdated?.Invoke(timeLeft, false);

            if (timeInRed >= LoseTime)
            {
                gameActive = false;
                OnLose?.Invoke();
            }
        }
    }

    private void SetPositionInternal(float newPos)
    {
        float clamped = Mathf.Clamp01(newPos);
        if (!Mathf.Approximately(clamped, currentPosition))
        {
            currentPosition = clamped;
            OnPositionChanged?.Invoke(currentPosition);
        }
    }
}