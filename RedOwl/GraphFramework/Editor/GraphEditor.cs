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
		protected override string[] GetInvisibleInDefaultInspector()
		{
			return new string[]{"m_Script"};
		}
		
		protected override void OnAfterDefaultInspector()
		{
			var color = GUI.backgroundColor;
			GUI.backgroundColor = Color.green;
			if (GUILayout.Button("Execute", GUILayout.Height(35))) ((Graph)target).Execute();
			GUI.backgroundColor = color;
		}
	}
}
#endif
