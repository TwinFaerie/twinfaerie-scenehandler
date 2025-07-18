#if TF_HAS_UNITASK
using Cysharp.Threading.Tasks;
#else
using System.Collections;
#endif
using System.Collections.Generic;
using System.Linq;
using TF.SceneHandler.Enum;
using TF.SceneHandler.Model;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TF.SceneHandler
{
    public class SceneHandlerController
    {
        private float loadProgress = 1.0f;
        private float unloadProgress = 1.0f;
        private bool isLoadComplete = true;
        private bool isUnloadComplete = true;

        private Scene lastActiveScene;
        private Scene currentActiveScene;

        public float LoadProgress => loadProgress;
        public float UnloadProgress => unloadProgress;
        public float OverallProgress => (LoadProgress + UnloadProgress) / 2.0f;
        public bool IsLoading => LoadProgress < 1.0f || !isLoadComplete;
        public bool IsUnloading => UnloadProgress < 1.0f || !isUnloadComplete;
        public bool IsOnProgress => IsLoading || IsUnloading;

        public readonly List<SceneData> ActiveScenes = new();
        public IEnumerable<SceneData> ActiveNormalScenes => ActiveScenes.Where(item => item.SceneType is SceneType.NORMAL);
        public IEnumerable<SceneData> ActiveLoadingScenes => ActiveScenes.Where(item => item.SceneType is SceneType.LOADING);
        
#if !TF_HAS_UNITASK
        private MonoBehaviour manager;
#endif

#if TF_HAS_UNITASK
        public SceneHandlerController()
#else
        public SceneHandlerController(MonoBehaviour manager)
#endif
        {
            // assume any scene the game started always is normal scenes
            ActiveScenes.AddRange(GetAllLoadedScene().Select(item => SceneData.Create(item.name, SceneType.NORMAL)));
#if !TF_HAS_UNITASK
            this.manager = manager;
#endif
        }

#if TF_HAS_UNITASK
        public async UniTask UnloadAllNormalScene()
#else
        public IEnumerator UnloadAllNormalScene()
#endif
        {
            if (ActiveNormalScenes.Count() == 0)
            {
#if TF_HAS_UNITASK
                return;
#else
                yield break;
#endif
            }

#if TF_HAS_UNITASK
            await UnloadScene(ActiveNormalScenes);
#else
            yield return manager.StartCoroutine(UnloadScene(ActiveNormalScenes));
#endif
        }

#if TF_HAS_UNITASK
        public async UniTask UnloadAllLoadingScene()
#else
        public IEnumerator UnloadAllLoadingScene()
#endif
        {
            if (ActiveLoadingScenes.Count() == 0)
            {
#if TF_HAS_UNITASK
                return;
#else
                yield break;
#endif
            }

#if TF_HAS_UNITASK
            await UnloadScene(ActiveLoadingScenes);
#else
            yield return manager.StartCoroutine(UnloadScene(ActiveLoadingScenes));
#endif
        }

#if TF_HAS_UNITASK
        public async UniTask SwitchToScene(IEnumerable<SceneData> scenes)
#else
        public IEnumerator SwitchToScene(IEnumerable<SceneData> scenes)
#endif
        {
#if TF_HAS_UNITASK
            await SwitchToSceneProcess(scenes);
#else
            yield return manager.StartCoroutine(SwitchToSceneProcess(scenes));
#endif
        }

#if TF_HAS_UNITASK
        public async UniTask SwitchToScene(SceneData scene)
#else
        public IEnumerator SwitchToScene(SceneData scene)
#endif
        {
            var scenes = new List<SceneData> { scene };
#if TF_HAS_UNITASK
            await SwitchToScene(scenes);
#else
            yield return manager.StartCoroutine(SwitchToScene(scenes));
#endif
        }

#if TF_HAS_UNITASK
        public async UniTask LoadScene(IEnumerable<SceneData> scenes, SceneData mainScene = null, bool waitForActivation = false)
#else
        public IEnumerator LoadScene(IEnumerable<SceneData> scenes, SceneData mainScene = null, bool waitForActivation = false)
#endif
        {
#if TF_HAS_UNITASK
            await LoadSceneProcess(scenes, mainScene, waitForActivation);
#else
            yield return manager.StartCoroutine(LoadSceneProcess(scenes, mainScene, waitForActivation));
#endif
        }

#if TF_HAS_UNITASK
        public async UniTask LoadScene(SceneData scene, bool isMainScene = true, bool waitForActivation = false)
#else
        public IEnumerator LoadScene(SceneData scene, bool isMainScene = true, bool waitForActivation = false)
#endif
        {
            var scenes = new List<SceneData>() { scene };
#if TF_HAS_UNITASK
            await LoadScene(scenes, isMainScene ? scene : null, waitForActivation);
#else
            yield return manager.StartCoroutine(LoadScene(scenes, isMainScene ? scene : null, waitForActivation));
#endif
        }

#if TF_HAS_UNITASK
        public async UniTask UnloadScene(IEnumerable<SceneData> scenes)
#else
        public IEnumerator UnloadScene(IEnumerable<SceneData> scenes)
#endif
        {
#if TF_HAS_UNITASK
            await UnloadSceneProcess(scenes);
#else
            yield return manager.StartCoroutine(UnloadSceneProcess(scenes));
#endif
        }

#if TF_HAS_UNITASK
        public async UniTask UnloadScene(SceneData scene)
#else
        public IEnumerator UnloadScene(SceneData scene)
#endif
        {
            var scenes = new List<SceneData>() { scene };
#if TF_HAS_UNITASK
            await UnloadScene(scenes);
#else
            yield return manager.StartCoroutine(UnloadScene(scenes));
#endif
        }

#if TF_HAS_UNITASK
        public async UniTask LoadSceneProcess(IEnumerable<SceneData> scenes, SceneData mainScene = null, bool waitForActivation = false)
#else
        public IEnumerator LoadSceneProcess(IEnumerable<SceneData> scenes, SceneData mainScene = null, bool waitForActivation = false)
#endif
        {
            if (IsLoading)
            {
#if TF_HAS_UNITASK
                return;
#else
                yield break;
#endif
            }

            isLoadComplete = false;

            var asyncLoads = new Dictionary<SceneData, AsyncOperation>();
            foreach (var item in scenes)
            {
                var asyncLoad = SceneManager.LoadSceneAsync(item.SceneName, LoadSceneMode.Additive);

                if (asyncLoad != null)
                {
                    if (waitForActivation)
                    {
                        asyncLoad.allowSceneActivation = false;
                    }

                    asyncLoads.Add(item, asyncLoad);
                }
            }

            // Wait until the asynchronous scene fully loads and update progress
#if TF_HAS_UNITASK
            await new WaitUntil(() =>
#else
            yield return new WaitUntil(() =>
#endif
            {
                loadProgress = GetAllAsyncProgress(asyncLoads.Values);
                return IsAllAsyncDone(asyncLoads.Values) && loadProgress >= 1;
            });

            foreach (var item in asyncLoads)
            {
                if (waitForActivation)
                {
                    item.Value.allowSceneActivation = true;
                }

                ActiveScenes.Add(item.Key);
            }

#if TF_HAS_UNITASK
            await new WaitUntil(() => 
#else
            yield return new WaitUntil(() =>
#endif
                IsAllSceneLoaded(asyncLoads.Keys)
            );

            var whileLoadingObjects = new List<WhileLoadingBehaviour>();
            foreach (var item in asyncLoads.Keys.Where(item => item.SceneType is SceneType.LOADING))
            {
                var objectsFound = Object.FindObjectsByType<WhileLoadingBehaviour>(FindObjectsSortMode.None);
                foreach (var objectItem in objectsFound)
                {
                    objectItem.LoadScene();
                }

                whileLoadingObjects.AddRange(objectsFound);
            }

#if TF_HAS_UNITASK
            await new WaitUntil(() => 
#else
            yield return new WaitUntil(() =>
#endif
                whileLoadingObjects.All(item => item.IsLoaded)
            );

            if (mainScene is not null)
            {
                var scene = SceneManager.GetSceneByName(mainScene.SceneName);

                lastActiveScene = currentActiveScene;
                currentActiveScene = scene;

                SceneManager.SetActiveScene(scene);
            }

            isLoadComplete = true;
        }

#if TF_HAS_UNITASK
        public async UniTask UnloadSceneProcess(IEnumerable<SceneData> scenes)
#else
        public IEnumerator UnloadSceneProcess(IEnumerable<SceneData> scenes)
#endif
        {
            if (IsLoading)
            {
#if TF_HAS_UNITASK
                return;
#else
                yield break;
#endif
            }

            isUnloadComplete = false;

            var whileLoadingObjects = new List<WhileLoadingBehaviour>();
            foreach (var item in scenes.Where(item => item.SceneType is SceneType.LOADING))
            {
                var objectsFound = Object.FindObjectsByType<WhileLoadingBehaviour>(FindObjectsSortMode.None);
                foreach (var objectItem in objectsFound)
                {
                    objectItem.UnloadScene();
                }

                whileLoadingObjects.AddRange(objectsFound);
            }

#if TF_HAS_UNITASK
            await new WaitUntil(() => 
#else
            yield return new WaitUntil(() =>
#endif
                whileLoadingObjects.All(item => item.IsUnloaded)
            );

            var asyncLoads = new Dictionary<SceneData, AsyncOperation>();
            foreach (var item in scenes)
            {
                var asyncLoad = SceneManager.UnloadSceneAsync(item.SceneName);

                if (asyncLoad != null)
                {
                    asyncLoads.Add(item, asyncLoad);
                }
            }

            // Wait until the asynchronous scene fully loads
#if TF_HAS_UNITASK
            await new WaitUntil(() =>
#else
            yield return new WaitUntil(() =>
#endif
            {
                unloadProgress = GetAllAsyncProgress(asyncLoads.Values);
                return IsAllAsyncDone(asyncLoads.Values) && unloadProgress >= 1;
            });

            foreach (var item in asyncLoads.Keys)
            {
                ActiveScenes.Remove(item);
            }

            if (lastActiveScene.isLoaded)
            {
                currentActiveScene = lastActiveScene;
                SceneManager.SetActiveScene(lastActiveScene);
            }

            isUnloadComplete = true;
        }

#if TF_HAS_UNITASK
        public async UniTask SwitchToSceneProcess(IEnumerable<SceneData> scenes)
#else
        public IEnumerator SwitchToSceneProcess(IEnumerable<SceneData> scenes)
#endif
        {
            var loadingScenes = scenes.Where(item => item.SceneType is SceneType.LOADING);
#if TF_HAS_UNITASK
            await LoadScene(loadingScenes, loadingScenes.First());
#else
            yield return manager.StartCoroutine(LoadScene(loadingScenes, loadingScenes.First()));
#endif

#if TF_HAS_UNITASK
            await new WaitWhile(() =>
#else
            yield return new WaitWhile(() =>
#endif
                IsOnProgress
            );

#if TF_HAS_UNITASK
            await UnloadAllNormalScene();
#else
            yield return manager.StartCoroutine(UnloadAllNormalScene());
#endif
            
            var whileLoadingObjects = new List<WhileLoadingBehaviour>();
            foreach (var item in scenes.Where(item => item.SceneType is SceneType.LOADING))
            {
                var objectsFound = Object.FindObjectsByType<WhileLoadingBehaviour>(FindObjectsSortMode.None);
                foreach (var objectItem in objectsFound)
                {
                    objectItem.Loading();
                }

                whileLoadingObjects.AddRange(objectsFound);
            }

#if TF_HAS_UNITASK
            await new WaitUntil(() => 
#else
            yield return new WaitUntil(() =>
#endif
                whileLoadingObjects.All(item => item.IsLoadingCompleted)
            );

            var newNormalScenes = scenes.Where(item => item.SceneType is SceneType.NORMAL);
#if TF_HAS_UNITASK
            await LoadScene(newNormalScenes, newNormalScenes.First());
#else
            yield return manager.StartCoroutine(LoadScene(newNormalScenes, newNormalScenes.First()));
#endif

#if TF_HAS_UNITASK
            await new WaitWhile(() =>
#else
            yield return new WaitWhile(() =>
#endif
                IsOnProgress
            );

#if TF_HAS_UNITASK
            await UnloadAllLoadingScene();
#else
            yield return manager.StartCoroutine(UnloadAllLoadingScene());
#endif
        }

        private bool IsAllAsyncDone(IEnumerable<AsyncOperation> asyncLoads)
        {
            foreach (var item in asyncLoads)
            {
                if (!item.isDone)
                {
                    return false;
                }
            }

            return true;
        }

        private float GetAllAsyncProgress(IEnumerable<AsyncOperation> asyncLoads)
        {
            var sum = asyncLoads.Sum(item => item.progress);
            return Mathf.Min(sum / asyncLoads.Count(), 1.0f);
        }

        private bool IsAllSceneLoaded(IEnumerable<SceneData> scenes)
        {
            return scenes.All(item => SceneManager.GetSceneByName(item.SceneName).isLoaded);
        }

        private List<Scene> GetAllLoadedScene()
        {
            var loadedScenes = new List<Scene>();

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                loadedScenes.Add(SceneManager.GetSceneAt(i));
            }

            return loadedScenes;
        }
    }
}
