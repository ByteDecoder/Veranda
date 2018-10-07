using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Sleipnir.Editor
{
    [DrawerPriority(0, 0, 10)]
    public class GraphDrawer<TGraph, TNode> : OdinValueDrawer<TGraph> where TGraph : OdinGraph<TNode>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (GUILayout.Button("Open editor"))
            {
                var window = (GraphEditor)EditorWindow.GetWindow(typeof(GraphEditor));
                window.title = "Sleipnir";
                ValueEntry.SmartValue.LoadDrawingData();
                window.LoadGraph(ValueEntry.SmartValue);
            }
            if (GUILayout.Button("Evaluate"))
            {
                ValueEntry.SmartValue.Evaluate();
            }
        }
    }
}