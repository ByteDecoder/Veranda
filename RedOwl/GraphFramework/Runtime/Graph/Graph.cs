using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
#endif
using RedOwl.Serialization;


namespace RedOwl.GraphFramework
{
    public interface IGraph
    {
        IEnumerable<Tuple<string, Type>> GetNodesTypes();
    }

    public abstract partial class Graph : SerializedScriptableObject, IEnumerable<Node>
    {
#if UNITY_EDITOR
        [DidReloadScripts]
        [InitializeOnLoadMethod]
        internal static void EditorLoad()
        {
            foreach (string guid in AssetDatabase.FindAssets("t:Graph"))
            {
                var asset = AssetDatabase.LoadAssetAtPath<Graph>(AssetDatabase.GUIDToAssetPath(guid));
                asset.Initialize();
            }
        }
#endif

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        internal static void RuntimeLoad()
        {
            foreach (var asset in Resources.FindObjectsOfTypeAll<Graph>())
            {
                asset.Initialize();
            }
        }

        [NonSerialized]
		private bool IsInitialized;

        internal void Initialize()
        {
            if (IsInitialized) return;
            Debug.LogFormat("Initialize Graph: {0}", name);
            foreach (var node in nodes.Values)
            {
                node.Initialize();
            }
            IsInitialized = true;
        }

        IEnumerator<Node> IEnumerable<Node>.GetEnumerator()
        {
            foreach (var node in nodes.Values)
            {
                yield return node;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var node in nodes.Values)
            {
                yield return node;
            }
        }
    }

    public abstract class Graph<T> : Graph, IGraph where T : Node
    {
#if UNITY_EDITOR
        private List<Tuple<string, Type>> nodeTypes;

		public IEnumerable<Tuple<string, Type>> GetNodesTypes()
		{
			if (nodeTypes == null)
			{
				nodeTypes = new List<Tuple<string, Type>>();
				string name;
				foreach (Type type in Extensions.ForAllTypes<T>())
				{
					name = type.Name.Replace("Node", "");
					type.WithAttr<NodeTitleAttribute>(a => { name = a.title; });
					nodeTypes.Add(new Tuple<string, Type>(ObjectNames.NicifyVariableName(name), type));
				}
			}
			foreach (var item in nodeTypes)
			{
				yield return item;
			}
		}
#endif
    }
}