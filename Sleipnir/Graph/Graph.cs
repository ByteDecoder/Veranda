using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using Sleipnir.Graph.Attributes;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Sleipnir.Graph
{
    public abstract class Graph<TNode, TNodeContent> : IGraph
        where TNode : Node<TNodeContent>, new()
    {
        [ShowInInspector, ReadOnly]
        public abstract List<TNode> Nodes { get; set; }

#if UNITY_EDITOR
        #region Sleipnir data

        [SerializeField] private float _scale;

        [SerializeField] private Vector2 _position;

        private Dictionary<string, Type> _availableNodes;
        private List<Tuple<string, Action<TNode>>> _graphContextFunctions;
        private IEnumerable<Action<TNode>> _onNodeDelete;

        public abstract IEnumerable<Connection> Connections();
        public abstract void AddConnection(Connection connection);
        public abstract void RemoveConnection(Connection connection);

        void IGraph.RemoveConnection(Connection connection)
        {
            RemoveConnection(connection);
            var toUpdateKnobs = Nodes.Where(n => n.EditorNode.Knobs.Contains(connection.InputKnob)
                                    || n.EditorNode.Knobs.Contains(connection.OutputKnob));
            foreach (var updateKnob in toUpdateKnobs)
                updateKnob.UpdateKnobs();
        }

        void IGraph.AddConnection(Knob outputKnob, Knob inputKnob)
        {
            AddConnection(new Connection(outputKnob, inputKnob));
            var toUpdateKnobs = Nodes.Where(n => n.EditorNode.Knobs.Contains(inputKnob)
                                              || n.EditorNode.Knobs.Contains(outputKnob));
            foreach (var updateKnob in toUpdateKnobs)
                updateKnob.UpdateKnobs();
        }

        public IEnumerable<string> AvailableNodes()
        {
            return _availableNodes.Keys;
        }

        public bool RemoveNode(Node node)
        {
            var toRemove = Nodes.First(o => o.EditorNode == node);
            var connectionsToRemove = Connections()
                .Where(c => node.Knobs
                    .Any(k => ReferenceEquals(c.InputKnob, k)
                              || ReferenceEquals(c.OutputKnob, k)));

            foreach (var connection in connectionsToRemove)
                RemoveConnection(connection);

            if (_onNodeDelete != null)
                foreach (var action in _onNodeDelete)
                    action?.Invoke(toRemove);

            Nodes.Remove(toRemove);
            UpdateContentProperties();
            return true;
        }

        IList<Node> IGraph.Nodes
        {
            get { return Nodes.Select(o => o.EditorNode).ToList(); }
        }

        public float Scale
        {
            get { return _scale; }
            set { _scale = value; }
        }

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public void OnGraphLoad()
        {
            LoadDelegates();
            UpdateContentProperties();
            foreach (var node in Nodes)
                AddGraphDelegates(node);
        }

        private void LoadDelegates()
        {
            _availableNodes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes(), (assembly, t) => new {assembly, t})
                .Where(t1 => typeof(TNodeContent).IsAssignableFrom(t1.t) && !t1.t.IsAbstract)
                .Select(t1 => t1.t).ToDictionary(o => o.Name);

            _onNodeDelete = GetType().GetMethods()
                .Where(m => m.GetCustomAttributes(typeof(OnNodeDelete), false).Length > 0)
                .Select<MethodInfo, Action<TNode>>(m => node => m.Invoke(this, new object[] { node }))
                .ToList();

            _graphContextFunctions = GetType().GetMethods()
                .Where(m => m.GetCustomAttributes(typeof(ContextFunction), false).Length > 0)
                .Select(m => new Tuple<string, Action<TNode>> 
                    (m.Name, node => m.Invoke(this, new object[] { node })))
                .ToList();
        }

        private void UpdateContentProperties()
        {
            var i = 0;
            foreach (var node in Nodes)
            {
                var index = i;
                i++;
                node.EditorNode.ValueGetter = () => Nodes[index];
                node.EditorNode.ValueSetter = value => Nodes[index] = (TNode) value;
            }
        }

        public void AddNode(string nodeId, Vector2 position)
        {
            var type = _availableNodes[nodeId];
            object content;
            if (typeof(ScriptableObject).IsAssignableFrom(type))
            {
                content = ScriptableObject.CreateInstance(type);
                AssetDatabase.CreateAsset((ScriptableObject)content, "Sleipnir");
                EditorUtility.SetDirty((ScriptableObject)content);
            }
            else if (type.GetConstructor(Type.EmptyTypes) != null)
            {
                content = type.GetConstructor(Type.EmptyTypes)?.Invoke(new object[] { });
            }
            else
            {
                Debug.LogError("Failed to create a node. Make sure your nodes inherit from" +
                               " Scriptable Object or implement parametless constructor.");
                return;
            }

            var node = new TNode
            {
                Content = (TNodeContent) content,
                EditorNode = new Node {Position = position}
            };

            AddGraphDelegates(node);
            node.LoadStartingData();
            node.LoadKnobs();
            var index = Nodes.Count;
            node.EditorNode.ValueGetter = () => Nodes[index];
            node.EditorNode.ValueSetter = value => Nodes[index] = (TNode)value;
            Nodes.Add(node);
        }

        private void AddGraphDelegates(TNode node)
        {
            node.EditorNode.ContextMenuFunctions = new List<Tuple<string, Action>>();
            foreach (var graphContextFunction in _graphContextFunctions)
                node.EditorNode.ContextMenuFunctions.Add(new Tuple<string, Action>(
                    graphContextFunction.Item1,
                    () => graphContextFunction.Item2(node)));

            node.AddNodeDelegates();
        }
        #endregion
#endif
    }
}
