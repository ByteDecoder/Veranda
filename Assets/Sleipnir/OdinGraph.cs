using System;
using System.Collections.Generic;
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
        
        private List<Type> _INodeTypes;
        private void LoadNodeTypes() {
            _INodeTypes = new List<Type>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach( var assembly in assemblies ) {
                var types = assembly.GetTypes();
                foreach( var t in types ) {
                    if( typeof( T ).IsAssignableFrom( t ) && !t.IsAbstract ) {
                        _INodeTypes.Add( t );
                    }
                }
            }
        }
        public IEnumerable<Type> NodeTypes {
            get {
                if (_INodeTypes == null) LoadNodeTypes();
                foreach (var item in _INodeTypes)
                {
                    yield return item;
                }
            }
        }

        public Node AddNode<TNode>()
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
        
        new public void SetDirty()
        {
            #if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            #endif
        }
    }
}