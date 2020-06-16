using RedOwl.Veranda.Editor;
using RedOwl.Veranda;
using UnityEditor;
using UnityEngine;

namespace RedOwl.Sleipnir.Editor
{
    [UXML, USS]
    public class SleipnirEditor : VerandaEditorWindow
    {
        [MenuItem("Tools/Red Owl/Sleipnir")]
        public static void ShowExample()
        {
            SleipnirEditor wnd = GetWindow<SleipnirEditor>();
            wnd.titleContent = new GUIContent("Sleipnir");
        }
    }
}