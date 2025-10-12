using UnityEngine;

/// <summary>
/// ScriptableObject, служащий для централизованного хранения всех настраиваемых параметров,
/// управляющих поведением, уроном, спавном и движением торнадо.
/// </summary>
[CreateAssetMenu(fileName = "TornadoConfig", menuName = "Weather/TornadoConfig")]
public class TornadoConfig : ScriptableObject
{
    [Header("Постоянная Вероятность и Интервал")]
    [Tooltip("Базовый шанс (%) выпадения торнадо при каждой проверке.")]
    public float constantTornadoChance = 5f;
    [Tooltip("Интервал (секунды) между проверками.")]
    public float checkIntervalSeconds = 300f; 
    public float warningDurationSeconds = 180f; 

    [Header("Активность и Спавн")]
    public float minActiveDurationSeconds = 60f;
    public float maxActiveDurationSeconds = 120f;
    public float minSpawnRadius = 500f;
    public float maxSpawnRadius = 1500f;

    [Header("Параметры Движения и Урона")]
    public float moveSpeed = 10f;
    public float rotationSpeed = 100f;
    public float lightDamagePerSecond = 5f; // Урон игроку во внешней зоне
    public float heavyDamagePerSecond = 20f; // Урон игроку в ядре
    public float destructionForce = 1000f; // Сила, применяемая к объектам
    [Tooltip("Сила притяжения, применяемая к игроку и легким Rigidbody")]
    public float playerPullForce = 50f;
    
    // <summary>
    /// Минимальное расстояние, на котором торнадо будет преследовать цель.
    /// Если цель ближе этого расстояния, торнадо начинает блуждать, давая шанс сбежать.
    /// </summary>
    [Tooltip("Минимальная дистанция, на которой торнадо будет преследовать цель.")]
    public float minChaseDistance = 50f;
}
