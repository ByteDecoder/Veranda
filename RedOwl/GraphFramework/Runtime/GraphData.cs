using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using RedOwl.Serialization;

namespace RedOwl.GraphFramework
{
    public abstract partial class Graph
    {
        public bool AutoExecute;

		[HideInInspector]
        internal Dictionary<Guid, Node> nodes = new Dictionary<Guid, Node>();
		[HideInInspector]
        internal List<Connection> connections = new List<Connection>();

		/// <summary>
		/// Returns the node with the given GUID
		/// </summary>
		/// <value>GUID</value>
		public Node this[Guid key] {
			get {
				return nodes[key];
			}
		}
		
		[Conditional("UNITY_EDITOR")]
		internal void MarkDirty()
		{
			EditorUtility.SetDirty(this);
		}

		[Conditional("UNITY_EDITOR")]
		internal void MarkDirtyAll()
		{
			EditorUtility.SetDirty(this);
			foreach (var node in nodes.Values)
			{
				EditorUtility.SetDirty(node);
			}
		}
		
		[Conditional("UNITY_EDITOR")]
		internal void ClearSubAssets()
		{
			var subAssets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(this));
			for (int i = 0; i < subAssets.Length; i++)
			{ // Delete all subassets except the main one to preserve references
				if (subAssets[i] != this) DestroyImmediate(subAssets[i], true);
			}
			EditorUtility.SetDirty(this);
		}
		
		[Conditional("UNITY_EDITOR")]
		internal void AddSubAsset(Node obj)
		{
			obj.name = obj.id.ToString();
			AssetDatabase.AddObjectToAsset(obj, this);
			obj.hideFlags = HideFlags.HideInHierarchy;
			EditorUtility.SetDirty(this);
			EditorUtility.SetDirty(obj);
		}

		[Conditional("UNITY_EDITOR")]
		internal void RemoveSubAsset(Guid id)
		{
			var subAssets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(this));
			for (int i = 0; i < subAssets.Length; i++)
			{
				if (subAssets[i] != this && subAssets[i].name == id.ToString()) DestroyImmediate(subAssets[i], true);
			}
			EditorUtility.SetDirty(this);
		}
    }
}
