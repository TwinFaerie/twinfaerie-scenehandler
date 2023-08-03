#if TF_HAS_SCENEREFERENCE
using Eflatun.SceneReference;
#endif
using TF.SceneHandler.Enum;
using UnityEngine;

namespace TF.SceneHandler.Model
{
    [CreateAssetMenu(fileName = "New Scene Data", menuName = "Twin Faerie/SceneHandler/New Scene Data", order = -100)]
    public class SceneData : ScriptableObject
    {
#if TF_HAS_SCENEREFERENCE
        [SerializeField] private SceneReference scene;
        private string sceneName = string.Empty;
#else
        [SerializeField] private string sceneName;
#endif
        [SerializeField] private SceneType sceneType;

#if TF_HAS_SCENEREFERENCE
        public string SceneName => string.IsNullOrEmpty(sceneName) ? scene.Name : sceneName;
#else
        public string SceneName => sceneName;
#endif
        public SceneType SceneType => sceneType;

        protected void Setup(string sceneName, SceneType sceneType)
        {
            this.sceneName = sceneName;
            this.sceneType = sceneType;
        }

        public static SceneData Create(string sceneName, SceneType sceneType)
        {
            var result = CreateInstance<SceneData>();
            result.Setup(sceneName, sceneType);
            return result;
        }
    }
}
