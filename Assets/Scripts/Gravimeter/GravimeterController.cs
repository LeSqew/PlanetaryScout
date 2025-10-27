using UnityEngine;

public class GravimeterController : MonoBehaviour
{
    private GravimeterModel _model; 

    // Публичный доступ к Модели для View (только Get)
    public GravimeterModel Model => _model;

    void Awake()
    {
        // НОВЫЙ КОД: Создаем экземпляр Модели
        _model = new GravimeterModel();
    }

    void Start()
    {
        // Запускаем тестовую аномалию, используя созданный _model
        _model.StartMinigame(new WaveParams(
            amplitude: 12.5f,
            frequency: 2.1f,
            phaseShift: 4.0f
        ));
    }
    
    void Update()
    {
        // Вызываем Tick на Модели, передавая дельту времени Unity
        _model.Tick(Time.deltaTime);
    }

    // --------------------------------------------------------
    // ПУБЛИЧНЫЕ МЕТОДЫ ДЛЯ ВЫЗОВА ЧЕРЕЗ UI.Slider (On Value Changed)
    // --------------------------------------------------------

    public void SetAmplitude(float value)
    {
        WaveParams currentInput = _model.PlayerParams;
        currentInput.Amplitude = value;
        _model.SetPlayerParams(currentInput);
    }

    public void SetFrequency(float value)
    {
        WaveParams currentInput = _model.PlayerParams;
        currentInput.Frequency = value;
        _model.SetPlayerParams(currentInput);
    }

    public void SetPhaseShift(float value)
    {
        WaveParams currentInput = _model.PlayerParams;
        currentInput.PhaseShift = value;
        _model.SetPlayerParams(currentInput);
    }
}
