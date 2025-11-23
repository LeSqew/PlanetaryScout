using UnityEngine;
using Random = UnityEngine.Random;

namespace Minigames.Spectrometer
{
    /// <summary>
    /// Модель данных и логики для мини-игры "Спектрометр" в режиме подбора RGB-цвета.
    /// Отвечает за генерацию цели, хранение текущего ввода и расчет точности.
    /// </summary>
    public class SpectrometerModel
    {
        private DataCategory _category;
        private int _rarity;
        private Color _targetColor;
        private float _targetUV;
        public float RedValue { get; private set; }
        public float GreenValue { get; private set; }
        public float BlueValue { get; private set; }
        public float UVValue { get; private set; }
        
        public void InitializeFromObject(DataCategory category, int rarity)
        {
            _category = category;
            _rarity = rarity;

            float factor = (rarity - 1) / 3f; // 1→0, 4→1
            _targetColor = new Color(
                Random.Range(factor * 0.2f, 1f),
                Random.Range(factor * 0.2f, 1f),
                Random.Range(factor * 0.2f, 1f)
            );
            _targetColor.r = Mathf.Clamp01(_targetColor.r * 1.2f);
            _targetColor.g = Mathf.Clamp01(_targetColor.g * 1.2f);
            _targetColor.b = Mathf.Clamp01(_targetColor.b * 1.2f);
            _targetUV = Random.Range(factor * 0.5f, 1.0f);
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
            
            float dist = Mathf.Sqrt(dr * dr + dg * dg + db * db + duv * duv);
            return Mathf.Clamp01(1f - dist / 2f);
        }
        
        private float GetRequiredAccuracy()
        {
            return _rarity switch
            {
                1 => 0.85f,
                2 => 0.90f,
                3 => 0.95f,
                4 => 0.98f,
                _ => 0.85f
            };
        }

        /// <summary>
        /// Попытка завершить анализ. Проверяет, достигнута ли необходимая точность.
        /// </summary>
        /// <param name="result">Выходная строка с названием материала и процентом точности.</param>
        /// <returns>True, если точность достаточна для текущего материала, иначе False.</returns>
        public bool TryGetResult(out string result)
        {
            float accuracy = CalculateAccuracy();
            float requiredAccuracy = GetRequiredAccuracy();

            if (accuracy >= requiredAccuracy)
            {
                result = $"Анализ завершён." +
                         $"Точность анализа: {Mathf.RoundToInt(accuracy * 100)}% (Требуется: {Mathf.RoundToInt(requiredAccuracy * 100)}%)";
                return true;
            }
            result = string.Empty;
            return false;
        }

    }
}
