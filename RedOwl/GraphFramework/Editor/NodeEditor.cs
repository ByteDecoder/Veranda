#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using RedOwl.Editor;
using RedOwl.GraphFramework;

namespace RedOwl.GraphFramework.Editor
{
	[CustomEditor(typeof(Node), true)]
	public class NodeEditor : RedOwlEditor
	{
		protected override string[] GetInvisibleInDefaultInspector()
		{
			var filters = new List<string>();
			filters.Add("m_Script");
			return filters.ToArray();
		}
		
		protected override void OnBeforeDefaultInspector()
		{
			EditorGUIUtility.labelWidth = 80;
		}
		
		protected override void OnAfterDefaultInspector()
		{
			EditorGUIUtility.labelWidth = 0;
		}
	}
}
#endif
