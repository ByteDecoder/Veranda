using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Sirenix.OdinInspector;

namespace Sleipnir
{
    [Serializable]
    public class OdinGraph<T> : SerializedScriptableObject, IGraph where T : INode, new()
    {
        [SerializeField]
        private List<T> _nodes = new List<T>();

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
                        Setter = value => _nodes[index] = (T) value
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
        
        private IEnumerable<Tuple<Type, string>> _INodeTypes;

        private IEnumerable<Tuple<Type, string>> GetNodeTypes()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            return assemblies.SelectMany(assembly => assembly.GetTypes(), (assembly, t) => new {assembly, t})
                .Where(t1 => typeof(T).IsAssignableFrom(t1.t) && !t1.t.IsAbstract)
                .Select(t1 => new Tuple<Type, string>(t1.t, null));
        }

        public IEnumerable<Tuple<Type, string>> NodeTypes
        {
            get
            {
                _INodeTypes = _INodeTypes ?? GetNodeTypes();
                return _INodeTypes;
            }
        }

        public Node AddNode<TNode>(string key = null)
        {
            _nodes.Add(Activator.CreateInstance<T>());
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
        
        public new void SetDirty()
        {
            #if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            #endif
        }
    }
}