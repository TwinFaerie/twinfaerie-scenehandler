#if TF_HAS_UNITASK
using Cysharp.Threading.Tasks;
#else
using UnityEngine;
#endif
using TF.SceneHandler.Model;
using System.Collections.Generic;

namespace TF.SceneHandler
{
#if TF_HAS_UNITASK
    public class SceneHandlerManager
#else
    public class SceneHandlerManager : MonoBehaviour
#endif
    {
        private bool isInitialized = false;
        private SceneHandlerController controller;

        public bool IsSystemReady => isInitialized;
        public SceneHandlerController Controller => controller;

        public void Init()
        {
#if TF_HAS_UNITASK
            controller = new SceneHandlerController();
#else
            controller = new SceneHandlerController(this);
#endif
            isInitialized = true;
        }

#if TF_HAS_UNITASK
        public async UniTask ChangeScene(SceneData scene)
#else
        public void ChangeScene(SceneData scene)
#endif
        {
            if (!IsSystemReady)
            { return; }

#if TF_HAS_UNITASK
            await controller.SwitchToScene(scene);
#else
            StartCoroutine(controller.SwitchToScene(scene));
#endif
        }

#if TF_HAS_UNITASK
        public async UniTask ChangeScene(List<SceneData> scene)
#else
        public void ChangeScene(List<SceneData> scene)
#endif
        {
            if (!IsSystemReady)
            { return; }

#if TF_HAS_UNITASK
            await controller.SwitchToScene(scene);
#else
            StartCoroutine(controller.SwitchToScene(scene));
#endif
        }

#if TF_HAS_UNITASK
        public async UniTask LoadSceneAdditive(SceneData scene)
#else
        public void LoadSceneAdditive(SceneData scene)
#endif
        {
            if (!IsSystemReady)
            { return; }

#if TF_HAS_UNITASK
            await controller.LoadScene(scene);
#else
            StartCoroutine(controller.LoadScene(scene));
#endif
        }

#if TF_HAS_UNITASK
        public async UniTask UnloadSceneAdditive(SceneData scene)
#else
        public void UnloadSceneAdditive(SceneData scene)
#endif
        {
            if (!IsSystemReady)
            { return; }

#if TF_HAS_UNITASK
            await controller.UnloadScene(scene);
#else
            StartCoroutine(controller.UnloadScene(scene));
#endif
        }
    }
}