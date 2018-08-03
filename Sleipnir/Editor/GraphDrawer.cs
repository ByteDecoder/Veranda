using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Sleipnir.Editor
{
    namespace Collections.NodeEditor.Editor
    {
        [OdinDrawer]
        public class TestDrawer<T> : OdinValueDrawer<T> where T : IGraph
        {
            protected override void DrawPropertyLayout(IPropertyValueEntry<T> entry, GUIContent label)
            {
                if (GUILayout.Button("Open editor"))
                {
                    var window = (GraphEditor)EditorWindow.GetWindow(typeof(GraphEditor));
                    window.LoadGraph(entry.SmartValue);
                }
                
                CallNextDrawer(entry, label);
            }
        }
    }
}