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

    public abstract partial class Graph : Node
    {
        public bool AutoExecute;

		[HideInInspector]
        public List<Connection> connections = new List<Connection>();

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

        internal void AddConnection(Connection connection)
        {
			connections.Add(connection);
			FireConnectionAdded(connection);
            MarkDirty();
        }

        internal void RemoveConnection(int index)
        {
            Connection connection = connections[index];
            connections.RemoveAt(index);
            FireConnectionRemoved(connection);
			MarkDirty();
        }

        internal override void OnInit()
        {
            base.OnInit();
            foreach (var node in nodes)
            {
                if (typeof(IGraphPort).IsAssignableFrom(node.GetType()))
                {
                    foreach (var keypair in node.portInfos)
                    {
                        portInfos.Add(keypair.Key, keypair.Value);
                    }
                }
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
                nodeTypes.Add(new Tuple<string, Type>("Sub Graph", this.GetType()));
                nodeTypes.Add(new Tuple<string, Type>("Graph Input - float", typeof(FloatGraphInput)));
                nodeTypes.Add(new Tuple<string, Type>("Graph Output - float", typeof(FloatGraphOutput)));
                // Add Builtin Types here
			}
			foreach (var item in nodeTypes)
			{
				yield return item;
			}
		}
#endif
    }
}