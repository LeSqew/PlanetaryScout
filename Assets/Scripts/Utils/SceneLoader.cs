using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Utils
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private InputActionAsset inputActionAsset; // назначьте в инспекторе

        private void Awake()
        {
            Application.targetFrameRate = 60;
        }

        /// <summary>
        /// Загружает сцену по её ID (build index).
        /// </summary>
        /// <param name="sceneId">ID сцены из Build Settings.</param>
        public void LoadSceneById(int sceneId)
        {
            Debug.Log($"[SCENE LOADER] Загрузка сцены {sceneId} из: {new System.Diagnostics.StackTrace(1, true)}");

            if (sceneId < 0 || sceneId >= SceneManager.sceneCountInBuildSettings)
            {
                Debug.LogError($"Scene ID {sceneId} вне диапазона! Проверь Build Settings.");
                return;
            }

            var playerMap = inputActionAsset.FindActionMap("Player");
            if (playerMap != null)
            {
                playerMap.Enable();
                Time.timeScale = 1f;
                Debug.Log("Player input map enabled.");
            }
            else
            {
                Debug.LogError("Player action map not found in InputActionAsset!");
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