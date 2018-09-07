using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Sleipnir.Demo.ExternalAdjacencyList
{
    // Example of a graph represented trough adjacency list with editor data stored through Unity 
    // serialization. This is an example how to work with classes that you don't have direct acces to.
    public class ExternalAdjacencyListDemo : MonoBehaviour
    {
        public FloatAdjacencyList FloatGraph = new FloatAdjacencyList();
        public IntListAdjacencyList IntListGraph = new IntListAdjacencyList();
    }

    [Serializable]
    public class FloatAdjacencyList : AdjacencyList<float> { }
    
    [Serializable]
    public class IntListAdjacencyList : AdjacencyList<IntList> { }

    [Serializable]
    public class IntList
    {
        public List<int> List = new List<int>();
    }

    [Serializable]
    public class AdjacencyList<T> : IGraph
    {
        [ReadOnly]
        public List<T> Nodes = new List<T>();
        [ReadOnly]
        public List<IntList> Connections = new List<IntList>();

#if UNITY_EDITOR
        #region Sleipnir data
        [SerializeField, ReadOnly]
        private List<Node> _editorNodes = new List<Node>();

        IList<Node> IGraph.Nodes
        {
            get
            {
                for (var i = 0; i < _editorNodes.Count; i++)
                {
                    var index = i;
                    _editorNodes[i].ValueGetter = () => Nodes[index];
                    _editorNodes[i].ValueSetter = value => Nodes[index] = (T)value;
                }
                return _editorNodes;
            }
        }

        [SerializeField]
        private float _zoom;

        public float Scale
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
            return new[] { "Node" };
        }

        public void AddNode(string nodeId, Vector2 position)
        {
            Nodes.Add(default(T));
            Connections.Add(new IntList());
            var node = new Node
            {
                Position = position,
                NodeWidth = 200,
                HeaderColor = new Color(0.1f, 0.4f, 0.6f),
                TitleColor = new Color(1, 1, 1),
                Knobs = new List<Knob>
                {
                    new Knob(50, KnobType.Input)
                    {
                        Color = new Color(0.1f, 0.4f, 0.6f), Description = "In"
                    },
                    new Knob(50, KnobType.Output)
                    {
                        Color = new Color(0.1f, 0.4f, 0.6f), Description = "Out"
                    }
                }
            };
            _editorNodes.Add(node);
        }

        public bool RemoveNode(Node node)
        {
            var index = _editorNodes.IndexOf(node);
            
            foreach (var connection in Connections)
            {
                connection.List.RemoveAll(e => e == index);
                for (var i = connection.List.Count - 1; i >= 0; i--)
                    if (connection.List[i] > index)
                        connection.List[i]--;
            }
            Nodes.RemoveAt(index);
            Connections.RemoveAt(index);
            _editorNodes.RemoveAt(index);
            return true;
        }

        IEnumerable<Connection> IGraph.Connections()
        {
            var connections = new List<Connection>();
            for (var i = 0; i < Connections.Count; i++)
            {
                var nestedList = Connections[i].List;
                foreach (var nestedIndex in nestedList)
                    connections.Add(new Connection(_editorNodes[i].Knobs[1], 
                                                   _editorNodes[nestedIndex].Knobs[0]));
            }

            return connections;
        }

        public void AddConnection(Knob outputKnob, Knob inputKnob)
        {
            var outputIndex = _editorNodes.FindIndex(o => o.Knobs.Contains(outputKnob));
            var inputIndex = _editorNodes.FindIndex(o => o.Knobs.Contains(inputKnob));
            Connections[outputIndex].List.Add(inputIndex);
        }

        public void RemoveConnection(Connection connection)
        {
            var outputIndex = _editorNodes.FindIndex(o => o.Knobs.Contains(connection.OutputKnob));
            var inputIndex = _editorNodes.FindIndex(o => o.Knobs.Contains(connection.InputKnob));

            Connections[outputIndex].List.Remove(inputIndex);
        }
        #endregion
#endif
    }
}