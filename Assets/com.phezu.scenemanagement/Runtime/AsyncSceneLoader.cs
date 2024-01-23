using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Phezu.Util;

namespace Phezu.SceneManagement {

    [AddComponentMenu("Phezu/Scene Management/Async Scene Loader")]
    public class AsyncSceneLoader : Singleton<AsyncSceneLoader> {

        private bool isloading;
        public string currentLoadedScene;

        private void Awake() {
            currentLoadedScene = SceneManager.GetActiveScene().name;
            SceneManager.sceneLoaded += SetActiveScene;
        }

        public IEnumerator LoadScene(string sceneName) {
            while (isloading)
                yield return null;

            if (!isloading)
                yield return LoadScene_(sceneName, true);
        }

        public IEnumerator LoadSceneAdditive(string sceneName) {
            if (!isloading)
                yield return LoadScene_(sceneName, false);
            else
                Debug.Log("Scene is already loading");
        }

        private IEnumerator LoadScene_(string sceneName, bool unloadCurrent) {
            isloading = true;

            if (unloadCurrent)
                yield return UnloadCurrent();

            yield return LoadNewAsync(sceneName);

            currentLoadedScene = sceneName;

            isloading = false;
        }

        private IEnumerator UnloadCurrent() {
            AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
            while (!unloadOperation.isDone)
                yield return null;
        }

        private IEnumerator LoadNewAsync(string sceneName) {
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            while (!loadOperation.isDone)
                yield return null;
        }

        private void SetActiveScene(Scene scene, LoadSceneMode mode) {
            SceneManager.SetActiveScene(scene);
        }

        protected override void OnDestroy() {
            SceneManager.sceneLoaded -= SetActiveScene;
        }
    }
}