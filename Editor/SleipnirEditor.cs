using RedOwl.UIX.Editor;
using RedOwl.UIX;
using UnityEditor;
using UnityEngine;

namespace RedOwl.Sleipnir.Editor
{
    [UXML, USS]
    public class SleipnirEditor : UIXEditorWindow
    {
        [MenuItem("Tools/Sleipnir")]
        public static void ShowExample()
        {
            SleipnirEditor wnd = GetWindow<SleipnirEditor>();
            wnd.titleContent = new GUIContent("Sleipnir");
        }
    }
}