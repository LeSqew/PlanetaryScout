using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utils
{
    public class SceneLoader : MonoBehaviour
    {

        /// <summary>
        /// Загружает сцену по её ID (build index).
        /// </summary>
        /// <param name="sceneId">ID сцены из Build Settings.</param>
        public void LoadSceneById(int sceneId)
        {
            if (sceneId < 0 || sceneId >= SceneManager.sceneCountInBuildSettings)
            {
                Debug.LogError($"Scene ID {sceneId} вне диапазона! Проверь Build Settings.");
                return;
            }

            SceneManager.LoadScene(sceneId);
        }

        /// <summary>
        /// Загружает сцену асинхронно (например, для экранов загрузки).
        /// </summary>
        public void LoadSceneByIdAsync(int sceneId)
        {
            if (sceneId < 0 || sceneId >= SceneManager.sceneCountInBuildSettings)
            {
                Debug.LogError($"Scene ID {sceneId} вне диапазона! Проверь Build Settings.");
                return;
            }

            StartCoroutine(LoadAsync(sceneId));
        }

        private System.Collections.IEnumerator LoadAsync(int sceneId)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneId);
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }

        public void LoadCurrentScene()
        {
            LoadSceneById(SceneManager.GetActiveScene().buildIndex);
        }
    }
}