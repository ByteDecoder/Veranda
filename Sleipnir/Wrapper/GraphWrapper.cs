using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Serialization;
using UnityEngine;

namespace Sleipnir.Wrapper
{
    public class GraphWrapper<T> : IGraph where T : IWrappedGraph
    {
        [OdinSerialize, HideInInspector]
        public T Content;

#if UNITY_EDITOR
        public GraphWrapper(T content)
        {
            Content = content;
        }

        [OdinSerialize, HideInInspector]
        private float _zoom;
        public float Zoom
        {
            get { return _zoom; }
            set { _zoom = value; } 
        }

        [OdinSerialize, HideInInspector]
        private Vector2 _position;
        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        [OdinSerialize, HideInInspector] private Dictionary<Tuple<object, object>, Connection> _connections
            = new Dictionary<Tuple<object, object>, Connection>();
        [OdinSerialize, HideInInspector] private Dictionary<object, Node> _nodes
            = new Dictionary<object, Node>();

        public IEnumerable<INode> NodesInDrawingOrder()
        {
            return _nodes.Values.OrderBy(o => o.DrawingOrder).Select(o => o);
        }

        public IEnumerable<string> AvailableNodes()
        {
            return Content.AvailableNodes();
        }

        public INode AddNode(string nodeId)
        {
            var toAdd = Content.AddNode(nodeId);
            if (toAdd == null)
                return null;
            var newNode = new Node(toAdd);
            _nodes.Add(toAdd, newNode);
            return newNode;
        }

        public bool RemoveNode(INode node)
        {
            _nodes.Remove(node.Value);
            Content.RemoveNode(node.Value);
            return true;
        }

        public void MoveNodeToFront(INode node)
        {
            var i = 1;
            foreach (var nodeInOrder in _nodes.OrderBy(o => o.Value.DrawingOrder))
                if (nodeInOrder.Value == node)
                    nodeInOrder.Value.DrawingOrder = 0;
                else
                {
                    nodeInOrder.Value.DrawingOrder = i;
                    i++;
                }   
        }

        public IEnumerable<IConnection> ConnectionsInDrawingOrder()
        {
            return _connections.OrderBy(o => o.Value.DrawingOrder).Select(o => o.Value);
        }

        public void MoveConnectionToFront(IConnection connection)
        {
            var i = 1;
            foreach (var connectionInOrder in _connections.OrderBy(o => o.Value.DrawingOrder))
                if (connectionInOrder.Value == connection)
                    connectionInOrder.Value.DrawingOrder = 0;
                else
                {
                    connectionInOrder.Value.DrawingOrder = i;
                    i++;
                }
        }

        public IConnection AddConnection(IKnob outputKnob, IKnob inputKnob)
        {
            var knobs = _nodes.SelectMany(o => o.Value.GetKnobs).ToArray();
            var input = knobs.First(o => o == inputKnob);
            var output = knobs.First(o => o == outputKnob);
            if (input == null || output == null)
                return null;

            object connectionContent;
            var connectionTuple = new Tuple<object, object>(output.Content, input.Content);
            if (!Content.AddConnection(connectionTuple, out connectionContent))
                return null;

            var newConnectioin = connectionContent == null
                ? new Connection(output, input)
                : new Connection(output, input, connectionContent);

            _connections.Add(connectionTuple, newConnectioin);
            return newConnectioin;
        }

        public bool RemoveConnection(IConnection connection)
        {
            var toRemove = _connections.FirstOrDefault(x => x.Value == connection).Key;
            _connections.Remove(toRemove);
            Content.RemoveConnection(toRemove);
            return true;
        }
#endif
    }
}