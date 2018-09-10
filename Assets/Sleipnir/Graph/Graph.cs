using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Sleipnir.Graph.Attributes;
using UnityEngine;

namespace Sleipnir.Graph
{
    public abstract class Graph<TNode, TNodeContent> : IGraph
        where TNode : GraphNode<TNodeContent>, new()
    {
        [ShowInInspector, ReadOnly]
        public abstract List<TNode> Nodes { get; set; }

#if UNITY_EDITOR
        #region Sleipnir data

        [SerializeField]
        private float _scale;

        [SerializeField]
        private Vector2 _position;

        private Dictionary<string, Type> _availableNodes;
        private List<Tuple<string, Action<TNode>>> _graphContextFunctions;
        private IEnumerable<Action<TNode>> _onNodeDelete;

        public abstract IEnumerable<Connection> Connections();
        public abstract void AddConnection(Connection connection);
        public abstract void RemoveConnection(Connection connection);
        
        public void LoadGraph()
        {
            LoadDelegates();
            LoadContentProperties();
            foreach (var node in Nodes)
                LoadContextFunctions(node);
        }

        private void LoadDelegates()
        {
            _availableNodes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes(), (assembly, type) => new { assembly, type })
                .Where(t => typeof(TNodeContent).IsAssignableFrom(t.type) && !t.type.IsAbstract)
                .Select(t => t.type)
                .Where(t => t.GetConstructors().Any(a => a.GetParameters().Length == 0))
                .Where(t => !t.InheritsFrom(typeof(UnityEngine.Object)))
                .ToDictionary(o => o.Name);

            var graphMethods = GetType().GetMethods();

            _onNodeDelete = graphMethods
                .Where(m => m.GetCustomAttribute<OnNodeDelete>() != null)
                .Select<MethodInfo, Action<TNode>>(m => node => m.Invoke(this, new object[] { node }))
                .ToList();

            _graphContextFunctions = graphMethods
                .Where(m => m.GetCustomAttribute<ContextFunction>() != null)
                .Select(m => new Tuple<string, Action<TNode>>
                    (m.Name, node => m.Invoke(this, new object[] {node})))
                .ToList();

            foreach (var node in Nodes)
                node.LoadDelegates();
        }

        private void LoadContextFunctions(TNode node)
        {
            node.EditorNode.ContextMenuFunctions = _graphContextFunctions
                .Select(f => new Tuple<string, Action>(f.Item1, () => f.Item2(node)))
                .ToList();

            node.AddNodeContextFunctions();
        }

        private void LoadContentProperties()
        {
            var i = 0;
            foreach (var node in Nodes)
            {
                var index = i;
                LoadContentProperties(node, index);
                i++;
            }
        }

        private void LoadContentProperties(TNode node, int index)
        {
            node.EditorNode.ValueGetter = () => Nodes[index];
            node.EditorNode.ValueSetter = value => Nodes[index] = (TNode)value;
        }

        IList<Node> IGraph.Nodes
        {
            get
            {
                foreach (var node in Nodes)
                    node.OnDraw();

                return Nodes.Select(o => o.EditorNode).ToList();
            }
        }
        
        public void AddNode(string nodeId, Vector2 position)
        {
            var type = _availableNodes[nodeId];
            var content = 
                type.GetConstructor(Type.EmptyTypes)?.Invoke(new object[] { });
            
            var node = new TNode
            {
                Content = (TNodeContent)content,
                EditorNode = new Node { Position = position }
            };

            node.LoadVisuals();
            node.LoadKnobs();
            LoadContextFunctions(node);
            LoadContentProperties(node, Nodes.Count);
            Nodes.Add(node);
        }
        
        public bool RemoveNode(Node node)
        {
            var nodeToRemove = Nodes.First(o => o.EditorNode == node);
            var connectionsToRemove = Connections()
                .Where(c => node.Knobs
                    .Any(k => ReferenceEquals(c.InputKnob, k) || ReferenceEquals(c.OutputKnob, k)));

            foreach (var connection in connectionsToRemove)
                RemoveConnection(connection);

            if (_onNodeDelete != null)
                foreach (var action in _onNodeDelete)
                    action?.Invoke(nodeToRemove);

            Nodes.Remove(nodeToRemove);
            LoadContentProperties();
            return true;
        }

        void IGraph.AddConnection(Connection connection)
        {
            AddConnection(connection);
            var toUpdateKnobs = Nodes
                .Where(n => n.EditorNode.Knobs.Contains(connection.InputKnob)
                         || n.EditorNode.Knobs.Contains(connection.OutputKnob));

            foreach (var updateKnob in toUpdateKnobs)
                updateKnob.OnConnectionUpdate();
        }
        
        void IGraph.RemoveConnection(Connection connection)
        {
            RemoveConnection(connection);
            var toUpdateKnobs = Nodes
                .Where(n => n.EditorNode.Knobs.Contains(connection.InputKnob)
                         || n.EditorNode.Knobs.Contains(connection.OutputKnob));

            foreach (var updateKnob in toUpdateKnobs)
                updateKnob.OnConnectionUpdate();
        }

        public IEnumerable<string> AvailableNodes()
        {
            return _availableNodes.Keys;
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
        #endregion
#endif
    }
}
