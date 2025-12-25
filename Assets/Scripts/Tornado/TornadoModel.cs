using System;
using UnityEngine;

namespace Tornado
{
    public class TornadoModel
    {
        public event Action<TornadoEvents.PlayerCaughtEventArgs> OnPlayerCaught;
        public event Action<TornadoEvents.PlayerReleasedEventArgs> OnPlayerReleased;
        public event Action<TornadoEvents.PlayerThrownEventArgs> OnPlayerThrown;
        public event Action<TornadoEvents.MovedEventArgs> OnMoved;
        public event Action<TornadoEvents.TargetChangedEventArgs> OnTargetChanged;

        private Vector3 _position;
        private Vector3 _targetPosition;
        private float _moveRadius;
        private float _changeDirectionInterval;
        private float _throwCooldown;
        private float _throwChance;
        private float _throwTimer;
        private float _directionChangeTimer;
        private bool _canThrow = true;
        private bool _hasPlayer = false;

        public float MaxDistance { get; set; } = 20f;
        public float RotationStrength { get; set; } = 50f;
        public float TornadoStrength { get; set; } = 2f;
        public float MoveSpeed { get; set; } = 2f;
        public float ThrowForce { get; set; } = 10f;
        public Vector3 Position => _position;
        public Vector3 TargetPosition => _targetPosition;
        public bool HasPlayer => _hasPlayer;
        public bool CanThrow => _canThrow;

        public TornadoModel(Vector3 initialPosition, float moveRadius = 20f, float changeDirectionInterval = 5f, float throwCooldown = 3f)
        {
            _position = initialPosition;
            _moveRadius = moveRadius;
            _changeDirectionInterval = changeDirectionInterval;
            _throwCooldown = throwCooldown;
            _throwChance = 0.3f; // Добавлено объявление
        
            SetNewTarget();
        }

        public void MoveTo(Vector3 target)
        {
            _targetPosition = target;
            OnTargetChanged?.Invoke(new TornadoEvents.TargetChangedEventArgs(_targetPosition));
        }

        public void SetNewTarget()
        {
            Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * _moveRadius;
            Vector3 newTarget = new Vector3(
                _position.x + randomCircle.x,
                _position.y,
                _position.z + randomCircle.y
            );
        
            MoveTo(newTarget);
        }

        public void CatchPlayer()
        {
            if (!_hasPlayer)
            {
                _hasPlayer = true;
                OnPlayerCaught?.Invoke(new TornadoEvents.PlayerCaughtEventArgs(_position));
            }
        }

        public void ReleasePlayer()
        {
            if (_hasPlayer)
            {
                _hasPlayer = false;
                OnPlayerReleased?.Invoke(new TornadoEvents.PlayerReleasedEventArgs(_position));
            }
        }

        public void ThrowPlayer()
        {
            if (_hasPlayer && _canThrow)
            {
                _hasPlayer = false;
                _canThrow = false;
                _throwTimer = 0f;
            
                var throwArgs = new TornadoEvents.PlayerThrownEventArgs(_position, CalculateThrowDirection());
                OnPlayerThrown?.Invoke(throwArgs);
            }
        }

        private Vector3 CalculateThrowDirection()
        {
            return UnityEngine.Random.onUnitSphere;
        }

        public void Update(float deltaTime)
        {
            // Обновление позиции торнадо
            Vector3 direction = (_targetPosition - _position).normalized;
            Vector3 newPosition = _position + direction * MoveSpeed * deltaTime;
        
            if (Vector3.Distance(newPosition, _targetPosition) < 1f)
            {
                _position = _targetPosition;
            }
            else
            {
                _position = newPosition;
            }

            OnMoved?.Invoke(new TornadoEvents.MovedEventArgs(_position));

            // Обновление таймеров
            _throwTimer += deltaTime;
            if (_throwTimer >= _throwCooldown)
            {
                _canThrow = true;
            }

            _directionChangeTimer += deltaTime;
            if (_directionChangeTimer >= _changeDirectionInterval)
            {
                SetNewTarget();
                _directionChangeTimer = 0f;
            }

            // Проверка случайного "броска"
            if (_hasPlayer && _canThrow)
            {
                float randomValue = UnityEngine.Random.value;
                if (randomValue < _throwChance * deltaTime)
                {
                    ThrowPlayer();
                }
            }
        }
    }
}