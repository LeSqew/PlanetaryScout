using UnityEngine;

namespace Tornado
{
    public class TornadoTestController : MonoBehaviour
    {
        private TornadoModel _model;
        private TornadoView _view;
        [SerializeField] private LayerMask groundLayer;

        [Header("Settings")]
        public float moveRadius = 20f;
        public float moveSpeed = 5f;

        private void Start()
        { 
            _model = new TornadoModel(transform.position, groundLayer);
            _model.MoveSpeed = moveSpeed;

            _view = GetComponent<TornadoView>();
            if (_view != null)
            {
                _view.Initialize(_model);
            }
        }

        private void Update()
        {
            if (_model != null)
            {
                _model.Update(Time.deltaTime);
            }
        }
    }
}