using UnityEngine;
using UnityEditor;
using RedOwl.Editor;
using RedOwl.GraphFramework;

namespace RedOwl.GraphFramework.Editor
{
	[CustomEditor(typeof(Graph), true)]
	public class GraphEditor : RedOwlEditor<Graph>
	{
		protected override void OnGUI()
		{
			if (Target.parent == null)
			{
				if (GUILayout.Button("Open Editor", GUILayout.Height(25)))
				{
					GraphWindow.Open();
					GraphWindow.LoadGraph(Target);
				}
				if (GUILayout.Button("Execute", GUILayout.Height(25)))
				{
					GraphWindow.Execute();
				}
			} else {
				if (GUILayout.Button("Open Graph", GUILayout.Height(25)))
				{
					GraphWindow.Open();
					GraphWindow.LoadSubGraph(Target);
				}
			}
		}
	}
}
