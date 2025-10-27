using UnityEngine;

public class GravimeterController : MonoBehaviour
{
    private GravimeterModel _model;
    [SerializeField] private float initialAmplitude = 5.0f;
    [SerializeField] private float initialFrequency = 1.0f;
    [SerializeField] private float initialPhaseShift = 0.0f;

    // Свойство для доступа к Модели (например, из View)
    public GravimeterModel Model => _model;

    private void Awake()
    {
        _model = new GravimeterModel();
            
        // Установим начальные значения для игрока (если Модель их не устанавливает)
        _model.SetPlayerAmplitude(initialAmplitude);
        _model.SetPlayerFrequency(initialFrequency);
        _model.SetPlayerPhaseShift(initialPhaseShift);
    }

    // ======================= МЕТОДЫ ДЛЯ UI-КОНТРОЛЛОВ ========================
        
    // Эти методы должны быть привязаны к компонентам UI (например, OnValueChanged у Slider)
        
    /// <summary>
    /// Вызывается при повороте "шестеренки" Амплитуды.
    /// </summary>
    public void SetAmplitude(float newAmplitude)
    {
        _model.SetPlayerAmplitude(newAmplitude);
    }

    /// <summary>
    /// Вызывается при повороте "шестеренки" Частоты.
    /// </summary>
    public void SetFrequency(float newFrequency)
    {
        _model.SetPlayerFrequency(newFrequency);
    }

    /// <summary>
    /// Вызывается при повороте "шестеренки" Сдвига по фазе.
    /// </summary>
    public void SetPhaseShift(float newPhaseShift)
    {
        _model.SetPlayerPhaseShift(newPhaseShift);
    }
}
