using UnityEngine;

namespace Tornado
{
    public class TornadoView : MonoBehaviour
    {
        private TornadoModel _model;
        [SerializeField] private string playerTag = "Player";
        [SerializeField] private TornadoVisualizer visualizer;

        public void Initialize(TornadoModel model)
        {
            _model = model;
            if (visualizer != null) visualizer.Initialize(_model);
            _model.OnPlayerThrown += HandleThrow;
        }
    
        private void Start()
        {
            transform.localScale = Vector3.zero; // Начинаем с нуля
        }
        
        private void Update()
        {
            if (transform.localScale.x < 1f)
            {
                transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.one, Time.deltaTime * 0.5f);
            }
    
            if (_model != null) transform.position = _model.Position;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(playerTag))
            {
                _model.CatchPlayer();
        
                var controller = other.GetComponent<PlayerTornadoController>();
                if (controller == null)
                {
                    controller = other.gameObject.AddComponent<PlayerTornadoController>();
                }
                controller.Attach(_model);
            }
        }

        private void HandleThrow(TornadoEvents.PlayerThrownEventArgs args)
        {
            GameObject player = GameObject.FindGameObjectWithTag(playerTag);
            if (player != null)
            {
                player.GetComponent<PlayerTornadoController>()?.ApplyThrow(args.TornadoPosition, args.ThrowForce);
            }
        }

        private void OnDestroy()
        {
            if (_model != null) _model.OnPlayerThrown -= HandleThrow;
        }
    }
}