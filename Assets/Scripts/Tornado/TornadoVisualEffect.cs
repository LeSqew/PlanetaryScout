using UnityEngine;

namespace Tornado
{
    public class TornadoVisualizer : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private ParticleSystem tornadoPS; 
        [SerializeField] private Color normalColor = Color.gray;
        [SerializeField] private Color catchColor = new Color(0.6f, 0.4f, 0.3f); 

        private TornadoModel _model;

        public void Initialize(TornadoModel model)
        {
            _model = model;
        }

        private void Update()
        {
            if (_model == null || tornadoPS == null) return;

            var mainModule = tornadoPS.main;
            Color targetColor = _model.HasPlayer ? catchColor : normalColor;
            mainModule.startColor = Color.Lerp(mainModule.startColor.color, targetColor, Time.deltaTime * 2f);

            var emission = tornadoPS.emission;
            emission.rateOverTime = _model.HasPlayer ? 100f : 50f;
        }
    }
}