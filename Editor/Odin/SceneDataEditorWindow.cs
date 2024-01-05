#if TF_HAS_TFODINEXTENDER
using Sirenix.Utilities.Editor;
using TF.OdinExtendedInspector.Editor;
using TF.SceneHandler.Model;
using UnityEditor;
using UnityEngine;

namespace TF.SceneHandler.Editor
{
    public class SceneDataEditorWindow : AssetViewerMenu<SceneData, CreateNewSceneData>
    {
        [MenuItem("TwinFaerie/Scene Data/Open Scene Data Setting", priority = -199)]
        public static void ShowMenu()
        {
            EditorWindow window = GetWindow<SceneDataEditorWindow>();

            window.titleContent = new GUIContent("Scene Data Setting", EditorIcons.SettingsCog.Raw);
            window.minSize = new Vector2(1000, 600);
        }
    }
}
#endif