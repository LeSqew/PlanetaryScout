using UnityEngine;
using UnityEngine.UI;

namespace Player.Health
{
    public class HealthBarView : MonoBehaviour 
    {
        [SerializeField]
        private Image fillImage;
        private int _maxHealth;

        public void Init(int maxHealth)
        {
            _maxHealth = maxHealth;
        }
    
        public void PrintHp(int currentHealth)
        {
            Debug.Log($"Текущее здоровье: {currentHealth}");
        }
    
        public void UpdateHealthBar(int currentHealth) 
        {
            if (!fillImage)
            {
                Debug.LogError("Заполнение изображения не назначено в инспекторе HealthBarView.");
                return;
            }

            float fillAmount = (float)currentHealth / _maxHealth;
        
            fillImage.fillAmount = fillAmount;
        }
    }
}