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
        private Vector3 _spawnPosition;
        
        private float _moveRadius;
        private float _changeDirectionInterval;
        private float _directionTimer;
        private float _throwTimer;
        private float _throwCooldown;
        private bool _hasPlayer;
        private LayerMask _groundLayer;
        
        public float MoveSpeed { get; set; } = 2f;
        public float RotationStrength { get; set; } = 60f;
        public float SuckingStrength { get; set; } = 70f;
        public float TornadoStrength { get; set; } = 20f; 
        public float ThrowForce { get; set; } = 30f;
        public Vector3 Position => _position;
        public bool HasPlayer => _hasPlayer;

        public TornadoModel(Vector3 startPos, LayerMask groundLayer, float radius = 30f, float changeInt = 5f, float throwCd = 4f)
        {
            _spawnPosition = startPos;
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

            if (Vector3.Distance(new Vector3(_position.x, 0, _position.z), 
                    new Vector3(_targetPosition.x, 0, _targetPosition.z)) < 0.5f 
                || _directionTimer >= _changeDirectionInterval)
            {
                SetNewTarget();
                _directionTimer = 0;
            }
            _directionTimer += deltaTime;

            if (_hasPlayer)
            {
                _throwTimer += deltaTime;
                if (_throwTimer >= _throwCooldown) ThrowPlayer();
            }
        }

        private void SetNewTarget()
        {
            Vector2 randOffset = UnityEngine.Random.insideUnitCircle * _moveRadius;

            Vector3 potentialTarget = new Vector3(
                _spawnPosition.x + randOffset.x,
                _spawnPosition.y,
                _spawnPosition.z + randOffset.y
            );

            if (Physics.Raycast(potentialTarget + Vector3.up * 10f, Vector3.down, out RaycastHit hit, 20f, _groundLayer))
            {
                _targetPosition = hit.point;
            }
            else
            {
                _targetPosition = potentialTarget;
            }
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