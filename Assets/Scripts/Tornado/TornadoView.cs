using System.Collections;
using UnityEngine;

namespace Tornado
{
    public class TornadoView : MonoBehaviour
    {
        private TornadoModel _model;

        [Header("Settings")]
        [SerializeField] private string playerTag = "Player";
        [SerializeField] private LayerMask groundLayer;

        [Header("Visuals")]
        [SerializeField] private ParticleSystem tornadoPS;
        [SerializeField] private Color normalColor = Color.gray;
        [SerializeField] private Color catchColor = new(0.6f, 0.4f, 0.3f);
        [SerializeField] private float fadeDuration = 2f;

        public void Initialize(TornadoModel model)
        {
            _model = model;
            _model.OnPlayerThrown += HandleThrow;
            _model.OnMoved += HandleMoved;
        }

        private void Start()
        {
            transform.localScale = Vector3.zero;
        }

        private void Update()
        {
            if (_model == null) return;

            if (transform.localScale.x < 1f)
            {
                transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.one, Time.deltaTime * 0.5f);
            }

            if (tornadoPS != null)
            {
                var mainModule = tornadoPS.main;
                Color targetColor = _model.HasPlayer ? catchColor : normalColor;
                mainModule.startColor = Color.Lerp(mainModule.startColor.color, targetColor, Time.deltaTime * 2f);

                var emission = tornadoPS.emission;
                emission.rateOverTime = _model.HasPlayer ? 100f : 50f;
            }
        }

        private void HandleMoved(TornadoEvents.MovedEventArgs args)
        {
            Vector3 pos = args.NewPosition;

            Ray ray = new Ray(pos + Vector3.up * 10f, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, 20f, groundLayer))
            {
                transform.position = hit.point;
            }
            else
            {
                transform.position = pos;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(playerTag) || other.transform.root.CompareTag(playerTag))
            {
                _model.CatchPlayer();

                var controller = other.GetComponentInParent<PlayerTornadoController>();
                if (controller == null)
                {
                    GameObject target = other.attachedRigidbody != null ? other.attachedRigidbody.gameObject : other.gameObject;
                    controller = target.AddComponent<PlayerTornadoController>();
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

        public void StartDissolving()
        {
            StartCoroutine(DissolveRoutine());
        }

        private IEnumerator DissolveRoutine()
        {
            if (tornadoPS != null)
            {
                var emission = tornadoPS.emission;
                emission.enabled = false; 
            }

            Vector3 initialScale = transform.localScale;
            float elapsed = 0;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, elapsed / fadeDuration);
                yield return null;
            }

            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (_model != null)
            {
                _model.OnPlayerThrown -= HandleThrow;
                _model.OnMoved -= HandleMoved;
            }
        }
    }
}