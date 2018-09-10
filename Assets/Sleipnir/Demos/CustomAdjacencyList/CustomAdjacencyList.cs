using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sleipnir.Demo.CustomAdjacencyList
{
    [Serializable]
    public class CustomAdjacencyList<TNode, TNodeContent, TConnection, TConnectionContent> 
        : IGraph, IEnumerable<TNode>
        where TNode : Node<TNodeContent, TConnection, TConnectionContent>, new()
        where TConnection : Connection<TConnectionContent>
        where TNodeContent : new()
        where TConnectionContent : new()
    {
        [SerializeField]
        private List<TNode> _nodes = new List<TNode>();

#if UNITY_EDITOR
        public Color BaseColor = new Color(0.1f, 0.4f, 0.6f);
#endif

        public TNode this[int i] => _nodes[i];

        public IEnumerator<TNode> GetEnumerator()
        {
            return _nodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

#if UNITY_EDITOR
        #region Sleipnir data

        IList<Node> IGraph.Nodes
        {
            get
            {
                for (var i = 0; i < _nodes.Count; i++)
                {
                    var index = i;
                    var node = _nodes[i];
                    var editorNode = _nodes[i].EditorNode;
                    if (editorNode.ValueGetter == null)
                        editorNode.ValueGetter = () => node;
                    if (editorNode.ValueSetter == null)
                        editorNode.ValueSetter = value => _nodes[index] = (TNode) value;
                    editorNode.HeaderTitle = node.Content.ToString();
                }

                return _nodes.Select(o => o.EditorNode).ToList();
            }
        }

        [SerializeField]
        private float _scale;

        public float Scale
        {
            get { return _scale; }
            set { _scale = value; }
        }

        [SerializeField]
        private Vector2 _position;

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }
        
        public IEnumerable<string> AvailableNodes()
        {
            return new [] { "Node" };
        }
        
        public void AddNode(string nodeId, Vector2 position)
        {
            var editorNode = new Node
            {
                Position = position,
                NodeWidth = 250,
                LabelWidth = 0,
                HasLabelSlider = true,
                HeaderColor = BaseColor,
                TitleColor = new Color(1, 1, 1),
                Knobs = new List<Knob>
                {
                    new Knob(50, KnobType.Input)
                    {
                        Color = BaseColor
                    }
                }
            };
            var newNode = new TNode { EditorNode = editorNode };
            _nodes.Add(newNode);
        }

        public bool RemoveNode(Node editorNode)
        {
            var index = _nodes.IndexOf(_nodes.First(o => ReferenceEquals(o.EditorNode, editorNode)));

            foreach (var node in _nodes)
                foreach (var connection in node.Connections)
                {
                    if (connection.TargetIndex == index)
                        connection.TargetIndex = -1;
                    if (connection.TargetIndex > index)
                        connection.TargetIndex --;
                }

            _nodes.RemoveAt(index);
            return true;
        }

        public IEnumerable<Connection> Connections()
        {
            var connections = new List<Connection>();

            foreach (var node in _nodes)
                for (var i = 0; i < node.Connections.Count; i++)
                    if (node.Connections[i].TargetIndex != -1)
                    {
                        connections.Add(new Connection(
                            node.OutputKnob(i),
                            _nodes[node.Connections[i].TargetIndex].InputKnob()));
                    }

            return connections;
        }

        public void AddConnection(Connection connection)
        {
            var indexOfInputNode = _nodes
                .FindIndex(o => ReferenceEquals(o.InputKnob(), connection.InputKnob));

            var outputNode = _nodes
                .First(o => o.EditorNode.Knobs.Any(k => ReferenceEquals(k, connection.OutputKnob)));

            var outKnobIndex = outputNode.OutputKnobIndex(connection.OutputKnob);

            outputNode.Connections[outKnobIndex].TargetIndex = indexOfInputNode;
        }

        public void RemoveConnection(Connection connection)
        {
            var outputNode = _nodes
                .First(o => o.EditorNode.Knobs.Any(k => ReferenceEquals(k, connection.OutputKnob)));

            var outKnobIndex = outputNode.OutputKnobIndex(connection.OutputKnob);
            outputNode.Connections[outKnobIndex].TargetIndex = -1;
        }

        #endregion
#endif
    }
}