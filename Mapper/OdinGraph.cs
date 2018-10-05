using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Sirenix.Serialization;

namespace Sleipnir.Mapper
{
    [Serializable]
    public class OdinGraph<T> : IGraph, IEnumerable<T>
    {
        [OdinSerialize]
        [HideReferenceObjectPicker]
        [ReadOnly]
        private List<OdinNode<T>> _nodes = new List<OdinNode<T>>();

        [OdinSerialize]
        [HideReferenceObjectPicker]
        [ReadOnly]
        private List<OdinConnection<T>> _connections = new List<OdinConnection<T>>();

        public T this[int key] => _nodes[key].Value;

        public IEnumerator<T> GetEnumerator()
        {
            return _nodes.Select(n => n.Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        public void Evaluate()
        {
            var maxIterations = _nodes.Count + 1;
            for (var i = 0; i < maxIterations; i++)
                if (Evaluate(i)) break;
            ResetEvaluations();
        }

        private bool Evaluate(int iteration)
        {
            var allNodesHaveEvaluated = true;
            foreach (var item in _nodes)
            {
                if (item.HasEvaluated)
                    continue;

                allNodesHaveEvaluated = false;
                CanEvaluate(iteration, item);
                if (item.CanEvaluate)
                    EvaluateNode(item);
            }
            return allNodesHaveEvaluated;
        }

        private void CanEvaluate(int iteration, OdinNode<T> node)
        {
            var canEvaluate = true;
            if (iteration == 0)
            {
                // When this is the first iteration
                // Only allow evaluation of a node if the graph no input connections for it
                foreach (var connection in _connections)
                    if (_nodes[connection.Input.NodeIndex] == node)
                        canEvaluate = false;
            }
            else
            {
                // When this is not the first iteration
                // Only allow evaluation of a node if all of the connections where 
                // this node is an input if the upstream output node has already evaluated
                foreach (var connection in _connections)
                    if (_nodes[connection.Input.NodeIndex] == node
                        && _nodes[connection.Output.NodeIndex].HasEvaluated == false)
                            canEvaluate = false;
            }

            if (canEvaluate)
                node.CanEvaluate = true;
        }

        private void EvaluateNode(OdinNode<T> node)
        {
            foreach (var connection in _connections)
                if (_nodes[connection.Output.NodeIndex] == node)
                    connection.Shlep(this);

            // Put in logic here for evaluating EvaluateAttribute marked methods based on their Order            
            node.Changed();
            node.HasEvaluated = true;
        }

        private void ResetEvaluations()
        {
            foreach (var item in _nodes)
                item.HasEvaluated = false;
        }

#if UNITY_EDITOR
        #region Sleipnir data
        [SerializeField]
        private float _zoom;

        [SerializeField]
        private Vector2 _pan;

        private IEnumerable<Type> _nodeTypes;

        IList<Node> IGraph.Nodes
        {
            get
            {
                foreach (var odinNode in _nodes)
                    odinNode.Draw();
                
                return _nodes.Select(n => n.Node).ToList();
            }
        }

        public void LoadDrawingData()
        {
            foreach (var odinNode in _nodes)
                odinNode.LoadDrawingData(_nodes, this);

            foreach (var odinConnection in _connections)
                odinConnection.LoadDrawingData(_nodes);
        }
        
        public float Zoom
        {
            get { return _zoom; }
            set { _zoom = value; }
        }

        public Vector2 Pan
        {
            get { return _pan; }
            set { _pan = value; }
        }

        public IEnumerable<string> AvailableNodes()
        {
            _nodeTypes = _nodeTypes ?? GetNodeTypes();
            return _nodeTypes.Select(t => t.Name);
        }

        private static IEnumerable<Type> GetNodeTypes()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            return assemblies.SelectMany(assembly => assembly.GetTypes())
                .Where(t => typeof(T).IsAssignableFrom(t) && !t.IsAbstract);
        }

        public void AddNode(string typeName)
        {
            var type = _nodeTypes.First(t => t.Name == typeName);
            var odinNode = new OdinNode<T>((T)Activator.CreateInstance(type), _nodes, this);
            _nodes.Add(odinNode);
        }

        public void RemoveNode(Node node)
        {
            var odinNode = _nodes.First(n => ReferenceEquals(n.Node, node));
            var index = _nodes.IndexOf(odinNode);
            _nodes.RemoveAt(index);

            var toRemove = new List<OdinConnection<T>>();

            foreach (var odinConnection in _connections)
            {
                var outputIndex = odinConnection.Output.NodeIndex;
                var inputIndex = odinConnection.Input.NodeIndex;

                if (outputIndex == index || inputIndex == index)
                    toRemove.Add(odinConnection);

                //if (outputIndex > index)
                //    odinConnection.Output.NodeIndex--;
                //if (inputIndex > index)
                //    odinConnection.Input.NodeIndex--;
            }

            foreach (var odinConnection in toRemove)
            {
                _connections.Remove(odinConnection);
            }

            foreach (var slot in _nodes.SelectMany(n => n.Slots.Keys))
                if (slot.NodeIndex > index)
                    slot.NodeIndex--;
            Evaluate();
        }

        public IEnumerable<Connection> Connections() =>
            _connections.Select(n => n.ConnectionDrawingData).ToList();

        public void AddConnection(Connection connection)
        {
            if(_connections.Any(c => connection.Equals(c.ConnectionDrawingData)))
                return;
            var odinConnection = new OdinConnection<T>(connection, _nodes);
            
            odinConnection.Shlep(this);

           _connections.Add(new OdinConnection<T>(connection, _nodes));
            Evaluate();
        }

        public void RemoveConnection(Connection connection)
        {
            var toRemove = _connections.First(c => connection.Equals(c.ConnectionDrawingData));
            _connections.Remove(toRemove);
            Evaluate();
        }
        #endregion
#endif
    }
}