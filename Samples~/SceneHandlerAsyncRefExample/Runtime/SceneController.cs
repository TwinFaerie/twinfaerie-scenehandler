using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TF.SceneHandler;
using TF.SceneHandler.Model;
using UnityEngine;

namespace TF.Samples.SceneHandler
{
    public class SceneController : MonoBehaviour
    {
        private SceneHandlerManager scene;
        [SerializeField] private List<SceneData> gameScenes;

        private void Start()
        {
            scene = new SceneHandlerManager();
            scene.Init();

            DontDestroyOnLoad(gameObject);
        }

        public void GoToGame()
        {
            scene.ChangeScene(gameScenes).Forget();
        }
    }
}
