using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Sleipnir.Demo
{
    public class NonOdinAdjacencyList : MonoBehaviour
    {
        public FloatAdjacencyGraph FloatAdjacencyGraph = new FloatAdjacencyGraph();
        public IntListAdjacencyGraph IntListAdjacencyGraph = new IntListAdjacencyGraph();
    }

    [Serializable]
    public class FloatAdjacencyGraph : NonOdinAdjacencyGraph<float> { }
    
    [Serializable]
    public class IntListAdjacencyGraph : NonOdinAdjacencyGraph<IntList> { }

    [Serializable]
    public class IntList
    {
        public List<int> List = new List<int>();
    }

    [Serializable]
    public class NonOdinAdjacencyGraph<T> : IGraph
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

        public void RemoveNode(Node node)
        {
            var index = _editorNodes.IndexOf(node);

            // Dummies are null objects created inside the graph editor. They enable removing
            // objects without messing with displaying - namely they keep order of objects that
            // are extended in the editor. Whenever you use Sleipnir to display data that can be 
            // extended remember to update number of dummies. I want to give users ability to 
            // cancel removing process here and because of that I can't take care of this by myself.
            if (index + 1 < _editorNodes.Count)
                _editorNodes[index + 1].NumberOfPrecedingDummies++;

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