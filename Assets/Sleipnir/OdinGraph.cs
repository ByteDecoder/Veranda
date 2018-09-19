using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Sleipnir
{
    public class OdinGraph<T> : SerializedScriptableObject, IGraph where T : INode, new()
    {
        [LabelText("Evaluation Async Wait Time"), Range(0.001f, 10.0f)]
        public float AsyncWaitTime = 0.1f;
        
        [OdinSerialize]
        private List<T> _nodes = new List<T>();

        [SerializeField]
        private List<Node> _editorNodes = new List<Node>();

        IList<ValueWrappedNode> Nodes
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
        IList<ValueWrappedNode> IGraph.Nodes
        {
            get { return Nodes; }
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
            _nodes.Add((T)Activator.CreateInstance(typeof(TNode)));
            var node = new Node();
            _editorNodes.Add(node);
            return node;
        }

        public void RemoveNode(Node node)
        {
            var index = _editorNodes.IndexOf(node);
            _nodes.RemoveAt(index);
            _editorNodes.RemoveAt(index);
        }

        private List<Connection> _connections = new List<Connection>();

        public IEnumerable<Connection> Connections()
        {
            return _connections;
        }

        public void AddConnection(Connection connection)
        {
            _connections.Add(connection);
        }

        public void RemoveConnection(Connection connection)
        {
            _connections.Remove(connection);
        }
        
        public new void SetDirty()
        {
            #if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            #endif
        }
        
        [Button]
        public void Test()
        {
            Evaluate();
        }
        
        public void Evaluate()
        {
            int maxIterations = _nodes.Count + 1;
            for (int i = 0; i < maxIterations; i++) {
                if (Evaluate(i)) break;
            }
            ResetEvaluations();
        }

        public IEnumerable AsyncEvaluate()
        {
            int maxIterations = _nodes.Count + 1;
            for (int i = 0; i < maxIterations; i++) {
                if (Evaluate(i)) break;
                yield return new WaitForSeconds(AsyncWaitTime);
            }
            ResetEvaluations();
        }

        private bool Evaluate(int iteration)
        {
            bool allNodesHaveEvaluated = true;
            foreach (var item in Nodes)
            {
                if (item.Node.HasEvaluated) continue;
                allNodesHaveEvaluated = false;
                CanEvaluate(iteration, item);
                if (item.Node.CanEvaluate) EvaluateNode(item);
            }
            return allNodesHaveEvaluated;
        }
        
        private void CanEvaluate(int iteration, ValueWrappedNode node)
        {
            bool canEvaluate = true;
            if (iteration == 0)
            {
                // When this is the first iteration
                // Only allow evaluation of a node if the graph no input connections for it
                foreach (var conn in _connections)
                {
                    if (conn.Input.Node == node ) canEvaluate = false;
                }
            } else {
                // When this is not the first iteration
                // Only allow evaluation of a node if all of the connections where 
                // this node is an input if the upstream output node has already evaluated
                foreach (var conn in _connections)
                {
                    if (conn.Input.Node == node )
                    {
                        if (conn.Output.Node.Node.HasEvaluated == false) canEvaluate = false;
                    }
                }
            }
            if (canEvaluate) node.Node.CanEvaluate = true;
        }
        
        private void EvaluateNode(ValueWrappedNode node)
        {
            foreach (var conn in _connections)
            {
                if (conn.Output.Node == node) conn.Shlep();
            }
            // Put in logic here for evaluating EvaluateAttribute marked methods based on their Order
            node.Node.HasEvaluated = true;
        }
        
        private void ResetEvaluations()
        {
            foreach (var item in Nodes)
            {
                item.Node.HasEvaluated = false;
            }
        }
    }
}