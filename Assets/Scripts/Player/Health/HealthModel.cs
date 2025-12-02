using System;
using UnityEngine;

namespace Player.Health
{
    public class HealthModel
    {
        public int CurrentHealth { get; private set; }
        private readonly int _maxHealth;
        public Action<int> OnHealthChanged;
        public HealthModel(HpSetttings settings)
        {
            CurrentHealth = settings.maxHp;
            _maxHealth = settings.maxHp;
        }
        public void TakeDamage(int hit)
        {
            CurrentHealth -= hit;
            CurrentHealth = Mathf.Max(0, CurrentHealth);
            OnHealthChanged?.Invoke(CurrentHealth);
        }
        public void Heal(int heal)
        {
            CurrentHealth += heal;
            CurrentHealth = Mathf.Min(_maxHealth, CurrentHealth);
            OnHealthChanged?.Invoke(CurrentHealth);
        }
    }
}
