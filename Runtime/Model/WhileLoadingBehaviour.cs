using UnityEngine;

namespace TF.SceneHandler.Model
{
    public abstract class WhileLoadingBehaviour : MonoBehaviour
    {
        public abstract bool IsLoaded { get; }
        public abstract bool IsUnloaded { get; }
        public abstract bool IsLoadingCompleted { get; }

        public abstract void LoadScene();
        public abstract void UnloadScene();
        public abstract void Loading();
    }
}
