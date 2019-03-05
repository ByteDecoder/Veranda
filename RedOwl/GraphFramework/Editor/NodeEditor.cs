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
			foreach (var item in ((Node)target))
			{
				filters.Add(item.name);
			}
			return filters.ToArray();
		}
		
		protected override void OnBeforeDefaultInspector()
		{
			EditorGUIUtility.labelWidth = ((Node)target).view.labelWidth;
			EditorGUI.BeginChangeCheck();
		}
		
		protected override void OnAfterDefaultInspector()
		{
			if (EditorGUI.EndChangeCheck())
			{
				Debug.LogFormat("Found change on node: {0}", target.name);
				Node node = target as Node;
				if (node != null) 
				{
					EditorUtility.SetDirty(node);
					if (node.graph.AutoExecute) node.graph.Execute();
				}
			}
			EditorGUIUtility.labelWidth = 0;
		}
	}
}
#endif
