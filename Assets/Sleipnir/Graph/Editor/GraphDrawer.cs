using Sirenix.OdinInspector.Editor;
using Sleipnir.Editor;
using Sleipnir.Editor.Collections.NodeEditor.Editor;
using UnityEditor;
using UnityEngine;

namespace Sleipnir.Graph.Editor
{
    [DrawerPriority(DrawerPriorityLevel.SuperPriority)]
    public class GraphDrawer<T, TNode, TNodeContent> : GraphDrawer<T>
        where T : Graph<TNode, TNodeContent>
        where TNode : GraphNode<TNodeContent>, new()
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (GUILayout.Button("Open editor"))
            {
                var graph = ValueEntry.SmartValue;
                var window = (GraphEditor)EditorWindow.GetWindow(typeof(GraphEditor));
                graph.LoadGraph();
                window.LoadGraph(graph);
            }

            foreach (var property in Property.Children)
                property.Draw();
        }
    }
}