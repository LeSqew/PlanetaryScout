using System;
using UnityEngine;

public class AcidRainModel
{
    public event Action<Vector3> OnPositionChanged;
    public event Action<bool> OnRainActiveChanged;
    public event Action<int> OnPlayerHit;

    private Vector3 _startPos;
    private Vector3 _endPos;
    private float _speed;
    private float _damagePerSecond;
    private float _damageInterval;
    private float _resetDelay;
    private bool _isActive = false;
    private float _timer = 0f;
    private float _damageTimer = 0f;

    public bool IsActive => _isActive;
    public Vector3 CurrentPosition { get; private set; }

    public AcidRainModel(Vector3 startPos, Vector3 endPos, float speed, float damagePerSecond, float damageInterval, float resetDelay)
    {
        _startPos = startPos;
        _endPos = endPos;
        _speed = speed;
        _damagePerSecond = damagePerSecond;
        _damageInterval = damageInterval;
        _resetDelay = resetDelay;
        ResetRain();
    }

    public void Update(float deltaTime)
    {
        if (!_isActive) return;

        // Двигаем дождь
        CurrentPosition = Vector3.MoveTowards(CurrentPosition, _endPos, _speed * deltaTime);
        OnPositionChanged?.Invoke(CurrentPosition);

        // Проверяем, достиг ли дождь конечной точки
        if (Vector3.Distance(CurrentPosition, _endPos) < 0.1f)
        {
            Deactivate();
            _timer = 0f;
        }

        // Наносим урон игроку
        _damageTimer += deltaTime;
        if (_damageTimer >= _damageInterval)
        {
            OnPlayerHit?.Invoke(Mathf.RoundToInt(_damagePerSecond * _damageInterval));
            _damageTimer = 0f;
        }
    }

    public void Activate()
    {
        _isActive = true;
        OnRainActiveChanged?.Invoke(true);
    }

    public void Deactivate()
    {
        _isActive = false;
        OnRainActiveChanged?.Invoke(false);
    }

    public void ResetRain()
    {
        CurrentPosition = _startPos;
    }

    public void ManualUpdate(float deltaTime)
    {
        if (!_isActive)
        {
            _timer += deltaTime;
            if (_timer >= _resetDelay)
            {
                ResetRain();
                Activate();
            }
        }
        else
        {
            Update(deltaTime);
        }
    }
}