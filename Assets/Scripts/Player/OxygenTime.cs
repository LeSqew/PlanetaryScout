using Player.Health;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class OxygenTime : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Image fillImage;
        [SerializeField] private TextMeshProUGUI timerText;

        [Header("Timer Settings")]
        [SerializeField] private float maxTime = 120f;     // 2 минуты
        private float _remainingTime;
    
        [Header("Health Controller")]
        private HealthController _healthController;

        private void Start()
        {
            _remainingTime = maxTime;
            _healthController = GetComponent<HealthController>();
        }

        private void Update()
        {
            UpdateTimer();
            UpdateBar();
            UpdateText();
        }

        private void UpdateTimer()
        {
            if (_remainingTime > 0)
            {
                _remainingTime -= Time.deltaTime;
                _remainingTime = Mathf.Max(0, _remainingTime);
            }
            else
            {
                _healthController.death?.Invoke();;
            }
        }

        private void UpdateBar()
        {
            if (!fillImage)
            {
                // Debug.Log("fillImage is null");
                return;
            }

            float fillAmount = _remainingTime / maxTime;
            fillImage.fillAmount = fillAmount;
        }

        private void UpdateText()
        {
            if (!timerText) return;

            int minutes = Mathf.FloorToInt(_remainingTime / 60);
            int seconds = Mathf.FloorToInt(_remainingTime % 60);

            timerText.text = $"{minutes:00}:{seconds:00}";
        }
    }
}

