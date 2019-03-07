#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using RedOwl.Editor;
using RedOwl.GraphFramework;

namespace RedOwl.GraphFramework.Editor
{
	[CustomEditor(typeof(Graph), true)]
	public class GraphEditor : RedOwlEditor
	{
		public override void OnInspectorGUI()
		{
			if (GUILayout.Button("Open Graph Editor", GUILayout.Height(25)))
			{
				GraphWindow.Open();
				GraphWindow.instance.Load((Graph)target);
			}
			if (GUILayout.Button("Execute Graph", GUILayout.Height(25)))
			{
				((Graph)target).Execute();
			}
		}
	}
}
#endif
