#if TF_HAS_TFODINEXTENDER
using TF.OdinExtendedInspector.Editor;
using TF.SceneHandler.Model;

namespace TF.SceneHandler.Editor
{
    public class CreateNewSceneData : BaseCreatableSO<SceneData>
    {
        public CreateNewSceneData()
        {
            name = "New Scene Data";
            path = "Assets/Settings/Scene Data";
        }
    }
}
#endif