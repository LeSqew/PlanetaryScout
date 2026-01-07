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
            StartCoroutine(TornadoCycleRoutine());
        }

        private IEnumerator TornadoCycleRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(spawnDelay);

                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
      
                _currentTornado = Instantiate(tornadoPrefab, spawnPoint.position, Quaternion.identity);
                Debug.Log("Торнадо появилось!");

                yield return new WaitForSeconds(lifeTime);

                if (_currentTornado != null)
                {
                    Destroy(_currentTornado);
                    Debug.Log("Торнадо исчезло.");
                }
            }
        }

        public void SpawnTornadoManually()
        {
            if (_currentTornado == null)
            {
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                _currentTornado = Instantiate(tornadoPrefab, spawnPoint.position, Quaternion.identity);
            }
        }
    }
}