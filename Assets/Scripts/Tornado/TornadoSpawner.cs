using UnityEngine;
using System.Collections;

namespace Tornado
{
    public class TornadoSpawner : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private GameObject tornadoPrefab;
        [SerializeField] private float spawnDelay = 10f; // Через сколько появится
        [SerializeField] private float lifeTime = 30f;   // Сколько просуществует
        [SerializeField] private Transform[] spawnPoints; // Точки появления

        private GameObject _currentTornado;

        private void Start()
        {
            if (tornadoPrefab == null || spawnPoints.Length == 0)
            {
                Debug.LogError("TornadoSpawner: Проверьте префаб и точки спавна в Инспекторе!");
                return;
            }
            StartCoroutine(TornadoCycleRoutine());
        }

        private IEnumerator TornadoCycleRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(spawnDelay);

                if (_currentTornado == null)
                {
                    SpawnTornado();
                }

                yield return new WaitForSeconds(lifeTime);

                DespawnCurrentTornado();
            }
        }

        public void SpawnTornadoManually()
        {
            if (_currentTornado == null)
            {
                SpawnTornado();
            }
        }

        private void SpawnTornado()
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            _currentTornado = Instantiate(tornadoPrefab, spawnPoint.position, Quaternion.identity);

            
            Debug.Log("<color=green>Торнадо появилось!</color>");
        }

        private void DespawnCurrentTornado()
        {
            if (_currentTornado != null)
            {
                if (_currentTornado.TryGetComponent<TornadoView>(out var view))
                {
                    view.StartDissolving();
                    Debug.Log("<color=yellow>Торнадо начало затухать...</color>");
                }
                else
                {
                    Destroy(_currentTornado);
                }

                _currentTornado = null;
            }
        }
    }
}