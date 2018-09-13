using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sleipnir.Demos
{
    [Serializable]
    public class DemoGraph : IGraph
    {
        [SerializeField]
        private List<DemoNode> _nodes = new List<DemoNode>();

        [SerializeField]
        private List<Node> _editorNodes = new List<Node>();

        IList<ValueWrappedNode> IGraph.Nodes
        {
            get
            {
                var result = new List<ValueWrappedNode>();
                for (var i = 0; i < _nodes.Count; i++)
                {
                    var index = i;
                    result.Add(new ValueWrappedNode
                    {
                        Node = _editorNodes[index],
                        Getter = () => _nodes[index],
                        Setter = value => _nodes[index] = (DemoNode) value
                    });
                }
                return result;
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
        private Vector2 _pan;

        public Vector2 Pan
        {
            get { return _pan; }
            set { _pan = value; }
        }

        public IEnumerable<string> AvailableNodes()
        {
            return new[] { "Node" };
        }

        public Node AddNode(string name)
        {
            _nodes.Add(new DemoNode());
            var node = new Node();
            _editorNodes.Add(node);
            return node;
        }

        public void RemoveNode(Node editorNode)
        {
        }

        public IEnumerable<Connection> Connections()
        { 
            return null;
        }

        public void AddConnection(Connection connection)
        {
        }

        public void RemoveConnection(Connection connection)
        {
        }
    }
}