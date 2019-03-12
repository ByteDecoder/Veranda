#if UNITY_EDITOR
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
					GraphWindow.instance.Load(Target);
				}
				if (GUILayout.Button("Execute", GUILayout.Height(25)))
				{
					Target.Execute();
				}
			} else {
				if (GUILayout.Button("Open Graph", GUILayout.Height(25)))
				{
					GraphWindow.Open();
					GraphWindow.instance.LoadSubGraph(Target);
				}
			}
		}
	}
}
#endif
