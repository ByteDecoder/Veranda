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

        internal override void OnInit()
        {
            base.OnInit();
        }

        public override void OnDirty()
        {
            if (parent == null && AutoExecute) Execute();
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