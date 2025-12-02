using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Компонент отображения (View) для мини-игры "Спектрометр".
/// Отвечает за: 1) Чтение ввода с ползунков и кнопки. 2) Отображение цветов и точности.
/// </summary>
public class SpectrometerView : MonoBehaviour
{
    public Slider sliderR;
    public Slider sliderG;
    public Slider sliderB;
    public Slider sliderUV;
    public Image accuracyFill;
    public TextMeshProUGUI accuracyText;
    public TextMeshProUGUI resultText;

    public Image targetColorImage;
    public Image measuredColorImage;

    public event Action<float, float, float, float> OnSliderChanged;
    public event Action OnConfirmPressed;

    void Start()
    {
        sliderR.onValueChanged.AddListener(_ => NotifySliderChange());
        sliderG.onValueChanged.AddListener(_ => NotifySliderChange());
        sliderB.onValueChanged.AddListener(_ => NotifySliderChange());
        sliderUV.onValueChanged.AddListener(_ => NotifySliderChange());
        NotifySliderChange();
    }
    
    /// <summary>
    /// Собирает текущие значения со всех ползунков и вызывает событие OnSliderChanged.
    /// </summary>
    private void NotifySliderChange()
    {
        OnSliderChanged?.Invoke(
            sliderR.value,
            sliderG.value,
            sliderB.value,
            sliderUV.value
        );
    }
    
    /// <summary>
    /// Обновляет отображение точности (заполнение полосы и текст).
    /// </summary>
    /// <param name="value">Значение точности (0.0 - 1.0).</param>
    public void SetAccuracy(float value)
    {
        if (accuracyFill != null)
            accuracyFill.fillAmount = Mathf.Clamp01(value);
        if (accuracyText != null)
            accuracyText.text = $"Точность: {Mathf.RoundToInt(value * 100)}%";
    }
    
    /// <summary>
    /// Отображает финальный или промежуточный результат анализа.
    /// </summary>
    /// <param name="result">Строка с сообщением.</param>
    public void ShowResult(string result)
    {
        if (resultText != null)
            resultText.text = result;
    }

    /// <summary>
    /// Рендерит целевой и подобранный цвета на соответствующие UI-элементы Image.
    /// </summary>
    /// <param name="measured">Подобранный пользователем цвет (RGB).</param>
    /// <param name="target">Целевой цвет (RGB).</param>
    public void RenderColors(Color measured, Color target)
    {
        // Отображаем целевой цвет
        if (targetColorImage != null)
            targetColorImage.color = target;
        
        // Отображаем подобранный пользователем цвет
        if (measuredColorImage != null)
            measuredColorImage.color = measured;
    }

    public void ResetUI()
    {
        // Сброс слайдеров
        sliderR.value = 0.5f;
        sliderG.value = 0.5f;
        sliderB.value = 0.5f;
        sliderUV.value = 0f;

        // Сброс текста результата
        if (resultText != null)
            resultText.text = "";

        // Сброс точности
        SetAccuracy(0f);

        // Очистка цветов
        if (targetColorImage != null) targetColorImage.color = Color.clear;
        if (measuredColorImage != null) measuredColorImage.color = Color.clear;
    }
    
    public void OnConfirmButton() => OnConfirmPressed?.Invoke();
    
}
