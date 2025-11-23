using System.Text;
using TMPro;
using UnityEngine;

public class GravimeterView : MonoBehaviour
{
    [Header("Связи с Контроллером")]
        [SerializeField] private GravimeterController controller;
        
        [Header("UI Элементы")]
        [SerializeField] private TextMeshProUGUI parametersText; // Игрок vs Цель
        [SerializeField] private TextMeshProUGUI statusText;     // Совпало / Не совпало / Сбой
        [SerializeField] private TextMeshProUGUI timerText;      // Оставшееся время
        [SerializeField] private TextMeshProUGUI qualityText;    // Качество данных (Искажение)
        
        private GravimeterModel _model;

        void Start()
        {
            if (controller == null)
            {
                Debug.LogError("VIEW: Controller не назначен.");
                enabled = false;
                return;
            }

            // *** НОВЫЙ КОД: Получаем экземпляр Модели через Контроллер ***
            _model = controller.Model;
            if (_model == null)
            {
                Debug.LogError("VIEW: Model не инициализирована Контроллером.");
                enabled = false;
                return;
            }
            // Подписка на события Модели
            _model.OnWaveParametersChanged += UpdateGraphsAndParameters;
            _model.OnMatchSuccess += DisplaySuccess;
            _model.OnAnomalyMissed += DisplayAnomalyMissed; 
        }

        /// <summary>
        /// Обновление UI на основе текущих данных Модели.
        /// </summary>
        private void UpdateGraphsAndParameters(WaveData data)
        {
            // 1. Обновление текстового поля с параметрами
            if (parametersText != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("--- ПАРАМЕТРЫ ВОЛН ---");
                sb.AppendLine($"ЦЕЛЬ: A:{data.TargetParams.Amplitude:F2} F:{data.TargetParams.Frequency:F2} P:{data.TargetParams.PhaseShift:F2}");
                sb.AppendLine($"ИГРОК: A:{data.PlayerParams.Amplitude:F2} F:{data.PlayerParams.Frequency:F2} P:{data.PlayerParams.PhaseShift:F2}");
                parametersText.text = sb.ToString();
            }

            // 2. Отображение Таймера
            if (timerText != null)
            {
                timerText.text = $"ОСТАВШЕЕСЯ ВРЕМЯ: {data.RemainingTime:F1} сек.";
                // Желтый, когда меньше 5 секунд
                timerText.color = data.RemainingTime <= 5.0f && !_model.IsMatched ? Color.yellow : Color.white;
            }

            // 3. Отображение Качества данных (Искажение)
            if (qualityText != null)
            {
                qualityText.text = $"КАЧЕСТВО ДАННЫХ: {data.DataQuality:P1}"; // P1 - формат процентов
                // Интерполяция цвета от Красного (0%) до Зеленого (100%)
                qualityText.color = Color.Lerp(Color.red, Color.green, data.DataQuality);
            }
            
            // 4. Отображение текущего Статуса
            if (statusText != null && !controller.Model.IsMatched && controller.Model.RemainingTime > 0)
            {
                statusText.text = "❌ ИДЕТ СОВМЕЩЕНИЕ...";
                statusText.color = Color.red;
            }
        }

        /// <summary>
        /// Вызывается при успешном совмещении (Победа).
        /// </summary>
        private void DisplaySuccess()
        {
            if (statusText != null)
            {
                statusText.text = "!!! ✅ УСПЕХ! АНОМАЛИЯ ЗАФИКСИРОВАНА !!!";
                statusText.color = Color.green;
            }
            Debug.LogWarning("!!! ПОБЕДА! АНОМАЛИЯ ЗАФИКСИРОВАНА !!!");
        }
        
        /// <summary>
        /// Вызывается при провале по времени (Сбой).
        /// </summary>
        private void DisplayAnomalyMissed()
        {
            if (statusText != null)
            {
                statusText.text = "!!! 🚨 СБОЙ! АНОМАЛИЯ ПРОПУЩЕНА. ДАННЫЕ ИСКАЖЕНЫ !!!";
                statusText.color = Color.magenta;
            }
            // Здесь может быть вызов метода для записи штрафа или провала миссии.
        }
}
