using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Minigames.Spectrometer
{
    /// <summary>
    /// Перечислитель редкости материала (Common, Uncommon, Rare, Legendary). Используется для определения сложности подбора цвета и результата.
    /// </summary>
    public enum RarityLevel
    {
        Common,   // Обычный (Легче подобрать)
        Uncommon, // Необычный
        Rare,     // Редкий
        Legendary // Легендарный (Сложнее всего подобрать)
    }
    
    /// <summary>
    /// Модель данных и логики для мини-игры "Спектрометр" в режиме подбора RGB-цвета.
    /// Отвечает за генерацию цели, хранение текущего ввода и расчет точности.
    /// </summary>
    public class SpectrometerModel
    {
        public float RedValue { get; private set; }
        public float GreenValue { get; private set; }
        public float BlueValue { get; private set; }
        public float UVValue { get; private set; }

        private Color _targetColor;
        private RarityLevel _rarity;
        private float _targetUV;
        
        /// <summary>
        /// Статические данные о редкости, определяющие необходимый порог точности
        /// и имя материала для каждого уровня RarityLevel.
        /// </summary>
        private static readonly Dictionary<RarityLevel, (float RequiredAccuracy, string Name)> RarityData =
            new()
            {
                // Чем реже, тем выше RequiredAccuracy (требуемая точность)
                { RarityLevel.Common, (0.85f, "Оксид Железа (Fe)") },
                { RarityLevel.Uncommon, (0.90f, "Кремний (Si)") },
                { RarityLevel.Rare, (0.95f, "Минерал Ксенона (Xr)") },
                { RarityLevel.Legendary, (0.98f, "Артефактная Энергия") }
            };
        
        /// <summary>
        /// Конструктор модели. Инициализирует новую игровую сессию.
        /// </summary>
        public SpectrometerModel()
        {
            GenerateTarget();
        }

        /// <summary>
        /// Генерирует новую целевую задачу: случайный RGB-цвет, уровень редкости и целевое UV-значение.
        /// Все параметры генерируются с учетом фактора редкости.
        /// </summary>
        public void GenerateTarget()
        {
            _rarity = (RarityLevel)Random.Range(0, Enum.GetNames(typeof(RarityLevel)).Length);
            float rarityFactor = (float)_rarity / (Enum.GetNames(typeof(RarityLevel)).Length - 1);
            _targetColor = new Color(
                Random.Range(rarityFactor * 0.2f, 1f),
                Random.Range(rarityFactor * 0.2f, 1f),
                Random.Range(rarityFactor * 0.2f, 1f)
            );
            
            _targetColor = Color.Lerp(_targetColor, Color.white, rarityFactor * 0.1f);
            _targetColor.r = Mathf.Clamp01(_targetColor.r * 1.2f);
            _targetColor.g = Mathf.Clamp01(_targetColor.g * 1.2f);
            _targetColor.b = Mathf.Clamp01(_targetColor.b * 1.2f);
            _targetUV = Random.Range(rarityFactor * 0.5f, 1.0f);
        }
        
        /// <summary>
        /// Устанавливает текущие значения фильтров (подобранный пользователем цвет и UV).
        /// </summary>
        /// <param name="red">Значение R-ползунка (0-1).</param>
        /// <param name="green">Значение G-ползунка (0-1).</param>
        /// <param name="blue">Значение B-ползунка (0-1).</param>
        /// <param name="uv">Значение UV-ползунка (0-1).</param>
        public void SetFilters(float red, float green, float blue, float uv = 0f)
        {
            RedValue = Mathf.Clamp01(red);
            GreenValue = Mathf.Clamp01(green);
            BlueValue = Mathf.Clamp01(blue);
            UVValue = Mathf.Clamp01(uv);
        }
        
        /// <summary>
        /// Возвращает целевой цвет (RGB), который должен подобрать игрок.
        /// </summary>
        /// <returns>Целевой цвет (Color).</returns>
        public Color GetTargetColor() => _targetColor;
        
        /// <summary>
        /// Возвращает цвет (RGB), который подобран игроком (текущие значения ползунков).
        /// </summary>
        /// <returns>Подобранный цвет (Color).</returns>
        public Color GetMeasuredColor() => new(RedValue, GreenValue, BlueValue, 1f);
        
        /// <summary>
        /// Возвращает целевое значение UV, которое нужно подобрать.
        /// </summary>
        /// <returns>Целевое UV (float).</returns>
        public float GetTargetUV() => _targetUV;
        
        /// <summary>
        /// Рассчитывает точность совпадения, включая 4-е измерение (UV).
        /// Использует формулу Евклидова расстояния в 4D-пространстве (R+G+B+UV).
        /// </summary>
        /// <returns>Точность от 0.0 (нет совпадения) до 1.0 (идеальное совпадение).</returns>
        public float CalculateAccuracy()
        {
            float dr = _targetColor.r - RedValue;
            float dg = _targetColor.g - GreenValue;
            float db = _targetColor.b - BlueValue;
            float duv = _targetUV - UVValue;
            
            float distance = Mathf.Sqrt(dr * dr + dg * dg + db * db + duv * duv);
            float maxDistance = 2.0f;

            float accuracy = 1.0f - (distance / maxDistance);
            return Mathf.Clamp01(accuracy);
        }

        /// <summary>
        /// Попытка завершить анализ. Проверяет, достигнута ли необходимая точность.
        /// </summary>
        /// <param name="result">Выходная строка с названием материала и процентом точности.</param>
        /// <returns>True, если точность достаточна для текущего материала, иначе False.</returns>
        public bool TryGetResult(out string result)
        {
            float accuracy = CalculateAccuracy();
            var data = RarityData[_rarity];
            float requiredAccuracy = data.RequiredAccuracy;

            if (accuracy >= requiredAccuracy)
            {
                result = $"Обнаружен: **{data.Name}** ({_rarity})\n" +
                         $"Точность анализа: {Mathf.RoundToInt(accuracy * 100)}% (Требуется: {Mathf.RoundToInt(requiredAccuracy * 100)}%)";
                return true;
            }
            result = string.Empty;
            return false;
        }

    }
}
