using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Sleipnir
{
    public class Test : SerializedMonoBehaviour
    {
        [OdinSerialize] public TestClass test;
    }

    public class TestClass : IGraph
    {
        [Serializable]
        [HideInInspector]
        public class TestConnection : IConnection
        {
            [OdinSerialize]
            private IKnob _outputKnob;
            public IKnob OutputKnob => _outputKnob;

            [OdinSerialize]
            private IKnob _inputKnob;
            public IKnob InputKnob => _inputKnob;

            [OdinSerialize]
            private bool _doesHaveValue;
            public bool DoesHaveValue => _doesHaveValue;

            public float ValueWindowWidth => 200;

            [OdinSerialize]
            private bool _isExpanded;
            public bool IsExpanded
            {
                get { return _isExpanded; }
                set { _isExpanded = value; }
            }

            [OdinSerialize]
            private object _value;

            public object Value
            {
                get { return _value; }
                set { _value = value; }
            }

            public TestConnection(IKnob outputKnob, IKnob inputKnob)
            {
                _outputKnob = outputKnob;
                _inputKnob = inputKnob;
                _doesHaveValue = true;
                IsExpanded = false;
                Value = new List<float>();
            }
        }

        [Serializable]
        [HideInInspector]
        public class TestKnob : IKnob
        {
            [OdinSerialize]
            private readonly string _description;
            [OdinSerialize]
            private readonly KnobType _type;
            [OdinSerialize]
            private readonly Color _color;

            public string Description => _description;
            public Color Color => _color;
            public KnobType Type => _type;

            public TestKnob(KnobType type, string description, Color color)
            {
                _type = type;
                _color = color;
                _description = description;
            }
        }

        [Serializable]
        public class TestNode : INode
        {
            [OdinSerialize]
            private object _value;

            [OdinSerialize]
            private readonly IReadOnlyList<IKnob> _knobs = new List<IKnob>
            {
                new TestKnob(KnobType.Output, "1st out", Color.cyan),
                new TestKnob(KnobType.Output, "2nd out", Color.blue),
                new TestKnob(KnobType.Output, "I can't count that far", Color.green),
                new TestKnob(KnobType.Output, "I hope 5", Color.red),
                new TestKnob(KnobType.Input, "", Color.cyan),
                new TestKnob(KnobType.Input, "Foo", Color.cyan),
                new TestKnob(KnobType.Input, "Boo", Color.green)
            };

            [OdinSerialize]
            private Vector2 _position1;

            [OdinSerialize] private readonly float _width = 400;

            public object Value
            {
                get { return _value; }
                set { _value = value; }
            }
            
            public IReadOnlyList<IKnob> Knobs => _knobs;

            public Vector2 Position
            {
                get { return _position1; }
                set { _position1 = value; }
            }
            
            public float Width => _width;
        }

        [SerializeField] private float _zoom;
        public float Zoom { get { return _zoom; } set { _zoom = value; }}

        [SerializeField] private Vector2 _position;
        public Vector2 Position { get { return _position; } set { _position = value; } }

        [OdinSerialize]
        public List<TestConnection> Connections = new List<TestConnection>();
        [OdinSerialize]
        public List<TestNode> Nodes = new List<TestNode>();

        public IEnumerable<INode> NodesInDrawingOrder()
        {
            return Nodes;
        }

        public IEnumerable<string> AvailableNodes()
        {
            return new[] {"Test/Test"};
        }

        public INode AddNode(string nodeId)
        {
            var newNode = new TestNode();
            Nodes.Add(newNode);
            return newNode;
        }

        public bool RemoveNode(INode node)
        {
            var toRemove = Nodes.First(o => ReferenceEquals(o, node));
            if (toRemove == null)
                return false;

            Nodes.Remove(toRemove);
            return true;
        }

        public void MoveNodeToFront(INode node)
        {
            var toMove = Nodes.First(o => ReferenceEquals(o, node));
            Nodes.Remove(toMove);
            Nodes.Add(toMove);
        }

        public IEnumerable<IConnection> ConnectionsInDrawingOrder()
        {
            return Connections;
        }

        public void MoveConnectionToFront(IConnection connection)
        {
            var toMove = Connections.First(o => ReferenceEquals(o, connection));
            Connections.Remove(toMove);
            Connections.Add(toMove);
        }

        public IConnection AddConnection(IKnob outputKnob, IKnob inputKnob)
        {
            var newConnection = new TestConnection(outputKnob, inputKnob);
            Connections.Add(newConnection);
            return newConnection;
        }

        public bool RemoveConnection(IConnection connection)
        {
            var toRemove = Connections.First(o => ReferenceEquals(o, connection));
            if (toRemove == null)
                return false;

            Connections.Remove(toRemove);
            return true;
        }
    }
}