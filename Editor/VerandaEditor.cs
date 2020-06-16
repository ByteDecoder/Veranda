using RedOwl.Parlour.Editor;
using RedOwl.Parlour;
using UnityEditor;
using UnityEngine;

namespace RedOwl.Veranda.Editor
{
    [UXML, USS]
    public class VerandaEditor : ParlourEditorWindow
    {
        [MenuItem("Tools/Red Owl/Veranda")]
        public static void ShowExample()
        {
            VerandaEditor wnd = GetWindow<VerandaEditor>();
            wnd.titleContent = new GUIContent("Veranda");
        }
    }
}