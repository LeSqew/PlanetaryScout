using UnityEngine;

namespace Tornado
{
    public class TornadoView : MonoBehaviour
    {
        [SerializeField] private float maxDistance = 20f;
        [SerializeField] private string playerTag = "Player";
    
        private TornadoModel _model;
        private TornadoVisualizer _visualizer;
        private Collider _triggerCollider;
        private bool _isPlayerCaught = false;

        public void Initialize(TornadoModel model)
        {
            _model = model;
            transform.position = _model.Position;
        
            // Настройка коллайдера
            _triggerCollider = GetComponent<Collider>();
            if (_triggerCollider != null)
            {
                _triggerCollider.isTrigger = true;
            }
        
            // Создаем визуализатор
            _visualizer = gameObject.AddComponent<TornadoVisualizer>();
            _visualizer.Initialize(_model);
        }

        private void Update()
        {
            if (_model != null)
            {
                transform.position = _model.Position;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(playerTag) && !_isPlayerCaught)
            {
                _isPlayerCaught = true;
            
                _model.CatchPlayer();
            
                var playerController = other.GetComponent<PlayerTornadoController>();
                if (playerController == null)
                {
                    playerController = other.gameObject.AddComponent<PlayerTornadoController>();
                }
                playerController.AttachToTornado(_model);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(playerTag) && _isPlayerCaught)
            {
                _isPlayerCaught = false;
            
                _model.ReleasePlayer();
            
                var playerController = other.GetComponent<PlayerTornadoController>();
                if (playerController != null)
                {
                    playerController.DetachFromTornado();
                }
            }
        }
    }
}