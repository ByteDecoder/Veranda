using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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

		public delegate void ConnectionAdded(Connection connection);
		public event ConnectionAdded OnConnectionAdded;

		public delegate void ConnectionRemoved(Connection connection);
		public event ConnectionRemoved OnConnectionRemoved;

		[SerializeField, HideInInspector]
        private Dictionary<Guid, Connection> _connections = new Dictionary<Guid, Connection>();
        public IEnumerable<Connection> connections {
            get {
                foreach (var key in _connections.Keys.ToList())
                {
                    yield return _connections[key];
                }
            }
        }

#if UNITY_EDITOR
        [DidReloadScripts]
        [InitializeOnLoadMethod]
        internal static void EditorLoad()
        {
            foreach (string guid in AssetDatabase.FindAssets("t:Graph"))
            {
                var asset = AssetDatabase.LoadAssetAtPath<Graph>(AssetDatabase.GUIDToAssetPath(guid));
                if (asset != null) asset.Initialize();
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
			_connections.Add(connection.id, connection);
            MarkDirty();
            OnConnectionAdded?.Invoke(connection);
        }

        internal void RemoveConnection(Connection connection)
        {
            _connections.Remove(connection.id);
			MarkDirty();
            OnConnectionRemoved?.Invoke(connection);
        }

        internal override void InternalInit()
        {
            base.InternalInit();
            CacheGraphPorts();
        }

        public override void OnDirty()
        {
            CacheGraphPorts();
            if (parent == null && AutoExecute) Execute();
        }

        private void CacheGraphPorts()
        {
            dynamicPorts = new Dictionary<Guid, Port>();
            foreach (var node in nodes)
            {
                if (typeof(GraphPortNode).IsAssignableFrom(node.GetType()))
                {
                    GraphPortNode portNode = node as GraphPortNode;
                    foreach (var info in node.portInfos.Values)
                    {
                        var port = info.Get(portNode);
                        dynamicPorts.Add(port.id, new GraphPort(portNode.label, node, info));
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
                nodeTypes.Add(new Tuple<string, Type>("Sub Graph", this.GetType()));
				foreach (Type type in Extensions.ForAllTypes<T>())
				{
					name = type.Name.Replace("Node", "");
					type.WithAttr<NodeTitleAttribute>(a => { name = a.title; });
					nodeTypes.Add(new Tuple<string, Type>(string.Format("Nodes/{0}", ObjectNames.NicifyVariableName(name)), type));
				}
				foreach (Type type in Extensions.ForAllTypes<GraphPortNode>())
				{
					name = type.Name.Replace("GraphInput", "Graph Input/").Replace("GraphOutput", "Graph Ouput/");
					nodeTypes.Add(new Tuple<string, Type>(name, type));
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