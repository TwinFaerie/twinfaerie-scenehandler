using System.Collections.Generic;
using TF.SceneHandler;
using TF.SceneHandler.Model;
using UnityEngine;

namespace TF.Samples.SceneHandler
{
    public class SceneController : MonoBehaviour
    {
        [SerializeField] private SceneHandlerManager scene;
        [SerializeField] private List<SceneData> gameScenes;

        private void Start()
        {
            scene.Init();
            DontDestroyOnLoad(gameObject);
        }

        public void GoToGame()
        {
            scene.ChangeScene(gameScenes);
        }
    }
}
