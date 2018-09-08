using Sirenix.OdinInspector.Editor;
using Sleipnir.Editor;
using UnityEditor;
using UnityEngine;

namespace Sleipnir.Graph.Editor
{
    namespace Collections.NodeEditor.Editor
    {
        public class GraphDrawer<T, TNode, TNodeContent> : OdinValueDrawer<T>
            where T : Graph<TNode, TNodeContent>
            where TNode : Node<TNodeContent>, new()
        {
            protected override void DrawPropertyLayout(GUIContent label)
            {
                if (GUILayout.Button("The right editor opener"))
                {
                    var window = (GraphEditor)EditorWindow.GetWindow(typeof(GraphEditor));
                    ValueEntry.SmartValue.OnGraphLoad();
                    window.LoadGraph(ValueEntry.SmartValue);
                }

                CallNextDrawer(label);
            }
        }
    }
}