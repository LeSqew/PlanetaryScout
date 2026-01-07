using System;
using UnityEngine;

namespace Tornado
{
    public class TornadoModel
    {
        public event Action<TornadoEvents.PlayerCaughtEventArgs> OnPlayerCaught;
        public event Action<TornadoEvents.PlayerThrownEventArgs> OnPlayerThrown;
        public event Action<TornadoEvents.MovedEventArgs> OnMoved;

        private Vector3 _position;
        private Vector3 _targetPosition;
        private float _moveRadius;
        private float _changeDirectionInterval;
        private float _directionTimer;
        private float _throwTimer;
        private float _throwCooldown;
        private bool _hasPlayer;

        public float MoveSpeed { get; set; } = 3f;
        public float RotationStrength { get; set; } = 40f;
        public float SuckingStrength { get; set; } = 20f;
        public float TornadoStrength { get; set; } = 5f; 
        public float ThrowForce { get; set; } = 30f;
        public Vector3 Position => _position;
        public bool HasPlayer => _hasPlayer;

        private LayerMask _groundLayer;


        public TornadoModel(Vector3 startPos, LayerMask groundLayer, float radius = 30f, float changeInt = 5f, float throwCd = 4f)
        {
            _position = startPos;
            _groundLayer = groundLayer; 
            _moveRadius = radius;
            _changeDirectionInterval = changeInt;
            _throwCooldown = throwCd;
            SetNewTarget();
        }

        public void Update(float deltaTime)
        {
            Vector3 nextStep = Vector3.MoveTowards(_position, _targetPosition, MoveSpeed * deltaTime);
            
            Ray ray = new Ray(nextStep + Vector3.up * 10f, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, 20f, _groundLayer))
            {
                _position = hit.point;
            }
            else
            {
                _position = nextStep;
            }

            OnMoved?.Invoke(new TornadoEvents.MovedEventArgs(_position));

            if (Vector3.Distance(_position, _targetPosition) < 0.5f || _directionTimer >= _changeDirectionInterval)
            {
                SetNewTarget();
                _directionTimer = 0;
            }
            _directionTimer += deltaTime;

            if (_hasPlayer)
            {
                _throwTimer += deltaTime;
                if (_throwTimer >= _throwCooldown)
                {
                    ThrowPlayer();
                }
            }
        }

        private void SetNewTarget()
        {
            // Генерируем случайную точку в круге
            Vector2 rand = UnityEngine.Random.insideUnitCircle * _moveRadius;
    
            // ВАЖНО: rand.y мы ставим в координату Z, 
            // чтобы торнадо двигалось по горизонтальной поверхности, а не взлетало вверх
            _targetPosition = new Vector3(
                _position.x + rand.x, 
                _position.y, // Оставляем текущую высоту (Y не меняем)
                _position.z + rand.y
            );
        }

        public void CatchPlayer()
        {
            _hasPlayer = true;
            _throwTimer = 0;
            OnPlayerCaught?.Invoke(new TornadoEvents.PlayerCaughtEventArgs(_position));
        }

        public void ThrowPlayer()
        {
            _hasPlayer = false;
            OnPlayerThrown?.Invoke(new TornadoEvents.PlayerThrownEventArgs(_position, ThrowForce));
        }
    }
}