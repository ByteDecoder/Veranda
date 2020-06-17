using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RedOwl.Veranda
{
    public interface INode
    {
        string Id { get; }
        Rect Rect { get; }
        IEnumerable<IFlowPort> FlowInPorts { get; }
        IEnumerable<IFlowPort> FlowOutPorts { get; }
        IEnumerable<IDataPort> DataInPorts { get; }
        IEnumerable<IDataPort> DataOutPorts { get; }
        void Initialize(IGraph graph);
        
        IFlowPort GetFlowPort(string id);
        IDataPort GetDataPort(string id);
        IEnumerable<IFlowConnection> GetFlowConnections(string id);
        IEnumerable<IDataConnection> GetDataConnections(string id);

        void Link(FlowPort outPort, FlowPort inPort);
        void Link<TValue>(DataPort<TValue> outPort, DataPort<TValue> inPort);
    }
    
    [Serializable, HideReferenceObjectPicker, InlineProperty]
    public abstract class Node : INode
    {
        [HideLabel, PropertyOrder(-1000)]
        public string name;
        
        [SerializeField]//, HideInInspector]
        private string id;
        
        public string Id => id;
        
        [SerializeField, HideInInspector]//, HideLabel, InlineProperty]
        private Rect rect;
        
        public Rect Rect => rect;
        
        private IGraph _graph;
        
        [SerializeReference, HideInInspector]
        private List<IFlowConnection> _flowConnections;
        [SerializeReference, HideInInspector]
        private List<IDataConnection> _dataConnections;

        private Dictionary<string, IFlowPort> _flowInPorts;
        private Dictionary<string, IFlowPort> _flowOutPorts;
        private Dictionary<string, IDataPort> _dataInPorts;
        private Dictionary<string, IDataPort> _dataOutPorts;

        public bool IsConnected
        {
            get
            {
                foreach (var connection in _flowConnections)
                {
                    foreach (var outPort in FlowOutPorts)
                    {
                        if (outPort.Id == connection.Port) return true;
                    }
                }
                return false;
            }
        }

        public IEnumerable<IFlowPort> FlowInPorts => _flowInPorts.Values;
        public IEnumerable<IFlowPort> FlowOutPorts => _flowOutPorts.Values;
        public IEnumerable<IDataPort> DataInPorts => _dataInPorts.Values;
        public IEnumerable<IDataPort> DataOutPorts => _dataOutPorts.Values;

        protected Node()
        {
            name = GetType().Name;
            id = VerandaUtils.GenerateId();
            rect = VerandaUtils.GenerateRect(this);
            var nodeType = GetType();
            foreach (var attr in VerandaUtils.Ports.GetDataPorts(nodeType))
            {
                var portInstance = (DataPort)Activator.CreateInstance(attr.Field.FieldType);
                portInstance.Initialize(this, attr);
                attr.Field.SetValue(this, portInstance);
            }
            foreach (var attr in VerandaUtils.Ports.GetFlowPorts(nodeType))
            {
                var portInstance = (FlowPort)Activator.CreateInstance(attr.Field.FieldType);
                portInstance.Initialize(this, attr);
                attr.Field.SetValue(this, portInstance);
            }
            _flowConnections = new List<IFlowConnection>();
            _dataConnections = new List<IDataConnection>();
        }
        
        public void Initialize(IGraph graph)
        {
            _graph = graph;
            Setup();
            var nodeType = GetType();
            BuildFlowPortList(nodeType);
            BuildDataPortList(nodeType);
        }

        private void BuildFlowPortList(Type nodeType)
        {
            var flowPorts = new List<IFlowPort>(6);
            foreach (var attr in VerandaUtils.Ports.GetFlowPorts(nodeType))
            {
                //Debug.Log($"Initializing FlowPort: {name}.{attr.Field.Name}");
                var port = (IFlowPort) attr.Field.GetValue(this);
                port.Initialize(this, attr);
                flowPorts.Add(port);
            }
            
            if (this is ISubGraphNode subGraphNode) flowPorts.AddRange(subGraphNode.GetDynamicFlowPorts());
            BuildFlowPortLookupTable(flowPorts);
        }
        
        private void BuildFlowPortLookupTable(IReadOnlyCollection<IFlowPort> flowPorts)
        {
            int count = flowPorts.Count;
            _flowInPorts = new Dictionary<string, IFlowPort>(count);
            _flowOutPorts = new Dictionary<string, IFlowPort>(count);
            foreach (var port in flowPorts)
            {
                if (port.Io.IsInput())
                    _flowInPorts.Add(port.Id, port);
                if (port.Io.IsOutput())
                    _flowOutPorts.Add(port.Id, port);
            }
        }

        private void BuildDataPortList(Type nodeType)
        {
            var dataPorts = new List<IDataPort>(20);
            foreach (var attr in VerandaUtils.Ports.GetDataPorts(nodeType))
            {
                //Debug.Log($"Initializing DataPort: {name}.{attr.Field.Name}");
                var port = (IDataPort) attr.Field.GetValue(this);
                port.Initialize(this, attr);
                dataPorts.Add(port);
            }

            if (this is ISubGraphNode subGraphNode) dataPorts.AddRange(subGraphNode.GetDynamicDataPorts());
            BuildDataPortLookupTable(dataPorts);
        }

        private void BuildDataPortLookupTable(IReadOnlyCollection<IDataPort> dataPorts)
        {
            int count = dataPorts.Count;
            _dataInPorts = new Dictionary<string, IDataPort>(count);
            _dataOutPorts = new Dictionary<string, IDataPort>(count);
            foreach (var port in dataPorts)
            {
                if (port.Io.IsInput())
                    _dataInPorts.Add(port.Id, port);
                if (port.Io.IsOutput())
                    _dataOutPorts.Add(port.Id, port);
            }
        }

        public IFlowPort GetFlowPort(string id)
        {
            return _flowInPorts[id];
        }
        
        public IDataPort GetDataPort(string id)
        {
            return _dataOutPorts[id];
        }

        public IEnumerable<IFlowConnection> GetFlowConnections(string id)
        {
            foreach (var connection in _flowConnections)
            {
                if (connection.Port != id) continue;
                yield return connection;
            }
        }

        public IEnumerable<IDataConnection> GetDataConnections(string id)
        {
            foreach (var connection in _dataConnections)
            {
                if (connection.Port != id) continue;
                yield return connection;
            }
        }
        
        public void Link(FlowPort outPort, FlowPort inPort)
        {
            _flowConnections.Add(new FlowConnection(outPort, inPort));
            _flowConnections.Sort((x, y) => _graph.GetNode(x.TargetNode).Rect.position.x.CompareTo(_graph.GetNode(y.TargetNode).Rect.position.x));
        }

        public void Link<TValue>(DataPort<TValue> outPort, DataPort<TValue> inPort)
        {
            _dataConnections.Add(new DataConnection(inPort, outPort));
        }

        public override string ToString()
        {
            return $"{name}";
        }

        #region API
        protected virtual void Setup() {}
        #endregion
    }
}