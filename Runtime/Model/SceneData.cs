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
        [SerializeField] private SceneReference sceneRef;
        private string sceneName = string.Empty;
#else
        [SerializeField] private string sceneName;
#endif
        [SerializeField] private SceneType sceneType;

#if TF_HAS_SCENEREFERENCE
        public SceneReference SceneRef => sceneRef;
        public string SceneName { get { try { return sceneRef.Name; } catch { return sceneName; } } }
#else
        public string SceneName => sceneName;
#endif
        public SceneType SceneType => sceneType;

        public static SceneData Create(string sceneName, SceneType sceneType)
        {
            var result = CreateInstance<SceneData>();
            result.sceneName = sceneName;
            result.sceneType = sceneType;
            return result;
        }
    }
}
