using RedOwl.UIEX.Editor;
using RedOwl.UIEX.Engine;
using UnityEditor;
using UnityEngine;

namespace RedOwl.Sleipnir.Editor
{
    [UXML, USS]
    public class SleipnirEditor : UIEXEditorWindow
    {
        [MenuItem("Tools/Sleipnir")]
        public static void ShowExample()
        {
            SleipnirEditor wnd = GetWindow<SleipnirEditor>();
            wnd.titleContent = new GUIContent("Sleipnir");
        }
    }
}