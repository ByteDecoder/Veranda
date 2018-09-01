using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Sleipnir.Demo
{
    public class NonOdinAdjacencyListExample : MonoBehaviour
    {
        public NonOdinFloatAdjacencyGraph FloatAdjacencyGraph = new NonOdinFloatAdjacencyGraph();
    }

    [Serializable]
    public class IntList
    {
        public List<int> List = new List<int>();
    }

    [Serializable]
    public class NonOdinFloatAdjacencyGraph : IGraph
    {
        [ReadOnly]
        public List<float> Nodes = new List<float>();
        public List<IntList> Connections = new List<IntList>();

        #if UNITY_EDITOR

        #region Sleipnir data

        [SerializeField, ReadOnly]
        private List<Node> _editorNodes = new List<Node>();

        List<Node> IGraph.Nodes
        {
            get
            {
                for (var i = 0; i < _editorNodes.Count; i++)
                {
                    var index = i;
                    _editorNodes[i].ValueGetter = () => Nodes[index];
                    _editorNodes[i].ValueSetter = (value) => Nodes[index] = (float)value;
                }
                return _editorNodes;
            }
        }

        [SerializeField]
        private float _zoom;

        public float Zoom
        {
            get { return _zoom; }
            set { _zoom = value; }
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
            return new[] { "Float" };
        }

        public Node CreateNode(string nodeId, Vector2 position)
        {
            Nodes.Add(0);
            Connections.Add(new IntList());
            var node = new Node
            {
                Position = position,
                NodeWidth = 200,
                LabelWidth = 0,
                Knobs = new List<Knob>
                {
                    new Knob(50, KnobType.Input),
                    new Knob(50, KnobType.Output),
                }
            };
            return node;
        }

        public bool RemoveNode(Node node)
        {
            var index = _editorNodes.IndexOf(node);
            Nodes.RemoveAt(index);
            Connections.RemoveAt(index);
            foreach (var connection in Connections)
                for (var i = 0; i < connection.List.Count; i++)
                    if (connection.List[i] > index)
                        connection.List[i]--;
            return true;
        }

        IEnumerable<Connection> IGraph.Connections()
        {
            var connections = new List<Connection>();
            for (var i = 0; i < Connections.Count; i++)
            {
                var nestedList = Connections[i].List;
                foreach (var nestedIndex in nestedList)
                    connections.Add(new Connection(_editorNodes[i].Knobs[1], _editorNodes[nestedIndex].Knobs[0]));
            }

            return connections;
        }

        public void AddConnection(Knob outputKnob, Knob inputKnob)
        {
            var outputNode = _editorNodes.First(o => o.Knobs.Contains(outputKnob));
            var inputNode = _editorNodes.First(o => o.Knobs.Contains(inputKnob));

            var outputIndex = _editorNodes.IndexOf(outputNode);
            var inputIndex = _editorNodes.IndexOf(inputNode);
            Connections[outputIndex].List.Add(inputIndex);
        }

        public void RemoveConnection(Connection connection)
        {
            var outputNode = _editorNodes.First(o => o.Knobs.Contains(connection.OutputKnob));
            var inputNode = _editorNodes.First(o => o.Knobs.Contains(connection.InputKnob));

            var outputIndex = _editorNodes.IndexOf(outputNode);
            var inputIndex = _editorNodes.IndexOf(inputNode);

            Connections[outputIndex].List.Remove(inputIndex);
        }

        #endregion

        #endif
    }
}