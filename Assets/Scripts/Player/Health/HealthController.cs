using System;
using UnityEngine;
using UnityEngine.Events;

namespace Player.Health
{
    [RequireComponent(typeof(HealthBarView))]
    public class HealthController : MonoBehaviour
    {
        [SerializeField] private HpSetttings hpSettings;
        private HealthModel _model;
        private HealthBarView _view;

        public UnityEvent<int> heal;
        public UnityEvent<int> takeDamage;
        public UnityEvent death;

        private bool IsFullHp => _model.CurrentHealth == hpSettings.maxHp;

        public UnityEvent stopRegeneration;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Awake()
        {
            _model = new HealthModel(hpSettings);
            _view = GetComponent<HealthBarView>();
            _view.Init(hpSettings.maxHp);
        
            takeDamage.AddListener(_model.TakeDamage);
            heal.AddListener(_model.Heal);

            _model.OnHealthChanged += _view.UpdateHealthBar;
            _model.OnHealthChanged += _view.PrintHp;
        }

        private void Update()
        {
            if (IsFullHp)
            {
                stopRegeneration?.Invoke();
            }
            if (_model.CurrentHealth <= 0) death?.Invoke();
        }
    }
}
