using Tornado;
using UnityEngine;

public class TornadoVisualizer : MonoBehaviour
{
    [SerializeField] private ParticleSystem tornadoPS;
    [SerializeField] private float baseEmissionRate = 100f;

    private TornadoModel _model;

    public void Initialize(TornadoModel model)
    {
        _model = model;
        
        // Не создаем Particle System, а используем уже существующий из префаба
        if (tornadoPS == null)
        {
            tornadoPS = GetComponentInChildren<ParticleSystem>();
            if (tornadoPS == null)
            {
                tornadoPS = GetComponent<ParticleSystem>();
            }
        }
    }

    private void Update()
    {
        if (_model != null)
        {
            UpdateVisualEffect();
        }
    }

    private void UpdateVisualEffect()
    {
        if (tornadoPS == null) return;
        
        // Изменяем интенсивность в зависимости от состояния
        float intensity = _model.HasPlayer ? 1.5f : 1f;
        
        var emission = tornadoPS.emission;
        emission.rateOverTime = new ParticleSystem.MinMaxCurve(baseEmissionRate * intensity);

        // Меняем цвет при захвате игрока
        var main = tornadoPS.main;
        if (_model.HasPlayer)
        {
            main.startColor = Color.Lerp(Color.gray, Color.red, 0.3f);
        }
        else
        {
            main.startColor = Color.gray;
        }
    }
}