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

        public IEnumerable<Tuple<Type, string>> NodeTypes
        {
            get
            {
                yield return new Tuple<Type, string>(typeof(DemoNode), null);
            }
        }

        public Node AddNode<T>(string key = null)
        {
            System.Object obj = Activator.CreateInstance<T>();
            _nodes.Add((DemoNode)obj);
            var node = new Node();
            _editorNodes.Add(node);
            return node;
        }

        public void RemoveNode(Node node)
        {
            int index = _editorNodes.IndexOf(node);
            _nodes.RemoveAt(index);
            _editorNodes.RemoveAt(index);
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
        
        public void SetDirty() {}
    }
}