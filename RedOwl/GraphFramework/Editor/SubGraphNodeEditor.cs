#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using RedOwl.Editor;
using RedOwl.GraphFramework;

namespace RedOwl.GraphFramework.Editor
{
	[CustomEditor(typeof(SubGraphNode), true)]
	public class SubGraphNodeEditor : RedOwlEditor<SubGraphNode>
	{
        protected override void OnGUI()
        {
            var node = Target;
			if (node.asset == null)
            {
                GUILayout.BeginHorizontal();
                node.style = (SubGraphStyles)EditorGUILayout.EnumPopup(node.style);
                if (node.style != SubGraphStyles.Embed) 
                {
                    node.asset = (Graph)EditorGUILayout.ObjectField(node.asset, typeof(Graph), false);
                    if (node.asset == node.graph) node.asset = null;
                }
                GUILayout.EndHorizontal();

                if (node.style == SubGraphStyles.Embed)
                {
                    if(GUILayout.Button("Create Graph", GUILayout.Height(25)))
                        node.asset = (Graph)Graph.CreateInstance(node.graph.GetType());
                }
            } else {
                if (node.style == SubGraphStyles.Embed)
                {
                    node.asset.name = GUILayout.TextField(node.asset.name);
                } else {
                    EditorGUI.BeginDisabledGroup(true);
                    GUILayout.TextField(node.asset.name);
                    EditorGUI.EndDisabledGroup();
                }
                if(GUILayout.Button("Open Graph", GUILayout.Height(25))) GraphWindow.instance.LoadSubGraph(node.asset);
			}
        }

        protected override void OnChange()
        {
            EditorUtility.SetDirty(Target.graph);
        }
	}
}
#endif
