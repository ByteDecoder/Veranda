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
			if (GUILayout.Button("Execute")) ((Graph)target).Execute();
		}
	}
}
#endif
