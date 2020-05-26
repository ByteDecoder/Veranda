using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine
{
    public interface INode
    {
        string Id { get; }
        Rect Rect { get; }
        bool IsRoot { get; }
        void Initialize(IGraph graph);
        
        IFlowPort GetFlowPort(string id);
        IDataPort GetDataPort(string id);
        IEnumerable<IFlowConnection> GetFlowConnections(string id);
        IEnumerable<IDataConnection> GetDataConnections(string id);

        void Link(FlowPort outPort, FlowPort inPort);
        void Link<TValue>(DataPort<TValue> outPort, DataPort<TValue> inPort);
        
        IEnumerator StartFlow(Flow flow);
        IEnumerator Run(IFlowPort port, Flow flow);
        IEnumerator Pull(IDataConnection port, Flow flow);
        IEnumerator Pull(IDataPort port, Flow flow);
    }
    
    [Serializable, HideReferenceObjectPicker, InlineProperty]
    public abstract class Node : INode
    {
        [HideLabel, PropertyOrder(-1000)]
        public string name;
        
        [SerializeField]
        [HideInInspector]
        //[DisplayAsString, HideLabel, Title("@Name")]
        private string id;
        
        public string Id => id;
        
        [SerializeField, HideInInspector]//, HideLabel, InlineProperty]
        private Rect rect;
        
        public Rect Rect => rect;
        
        private IGraph _graph;
        public bool IsRoot { get; protected set; }
        
        [SerializeReference, HideInInspector]
        private List<IFlowConnection> _flowConnections;
        [SerializeReference, HideInInspector]
        private List<IDataConnection> _dataConnections;

        private Dictionary<string, IFlowPort> _flowInPorts;
        private Dictionary<string, IFlowPort> _flowOutPorts;
        private Dictionary<string, IDataPort> _dataInPorts;
        private Dictionary<string, IDataPort> _dataOutPorts;

        protected Node()
        {
            name = GetType().Name;
            id = Sleipnir.GenerateId();
            rect = Sleipnir.GenerateRect(this);
            var nodeType = GetType();
            foreach (var attr in Sleipnir.Ports.GetDataPorts(nodeType))
            {
                var portInstance = (DataPort)Activator.CreateInstance(attr.Field.FieldType);
                portInstance.Initialize(this, attr);
                attr.Field.SetValue(this, portInstance);
            }
            foreach (var attr in Sleipnir.Ports.GetFlowPorts(nodeType))
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
            var flowPorts = new List<FlowPort>(6);
            foreach (var attr in Sleipnir.Ports.GetFlowPorts(nodeType))
            {
                //Debug.Log($"Initializing FlowPort: {name}.{attr.Field.Name}");
                var port = (FlowPort) attr.Field.GetValue(this);
                port.Initialize(this, attr);
                flowPorts.Add(port);
            }
            
            if (this is ISubGraphNode subGraphNode) flowPorts.AddRange(subGraphNode.GetDynamicFlowPorts());
            BuildFlowPortLookupTable(flowPorts);
        }
        
        private void BuildFlowPortLookupTable(List<FlowPort> flowPorts)
        {
            int count = flowPorts.Count;
            _flowInPorts = new Dictionary<string, IFlowPort>(count);
            _flowOutPorts = new Dictionary<string, IFlowPort>(count);
            foreach (var port in flowPorts)
            {
                if (port.Io.IsInput())
                    _flowInPorts.Add(port.Id, port);
                if (port.Io.IsOutput())
                    _flowInPorts.Add(port.Id, port);
            }
        }

        private void BuildDataPortList(Type nodeType)
        {
            var dataPorts = new List<DataPort>(20);
            foreach (var attr in Sleipnir.Ports.GetDataPorts(nodeType))
            {
                //Debug.Log($"Initializing DataPort: {name}.{attr.Field.Name}");
                var port = (DataPort) attr.Field.GetValue(this);
                port.Initialize(this, attr);
                dataPorts.Add(port);
            }

            if (this is ISubGraphNode subGraphNode) dataPorts.AddRange(subGraphNode.GetDynamicDataPorts());
            BuildDataPortLookupTable(dataPorts);
        }

        private void BuildDataPortLookupTable(List<DataPort> dataPorts)
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
        }

        public void Link<TValue>(DataPort<TValue> outPort, DataPort<TValue> inPort)
        {
            _dataConnections.Add(new DataConnection(inPort, outPort));
        }

        public IEnumerator StartFlow(Flow flow)
        {
            foreach (var port in _flowOutPorts.Values)
            {
                yield return port.Run(flow);
            }
        }

        public IEnumerator Run(IFlowPort port, Flow flow)
        {
            // Ask DataIn ports to suck in data?
            yield return port.Run(flow);
        }
        
        public IEnumerator Pull(IDataConnection connection, Flow flow)
        {
            // TODO: do we need to check if the upstream nodes already had its data pushed into the flow?
            var nextNode = _graph.GetNode(connection.TargetNode);
            yield return nextNode.Pull(nextNode.GetDataPort(connection.TargetPort), flow);
        }

        public IEnumerator Pull(IDataPort port, Flow flow)
        {
            foreach (var dataInPort in _dataInPorts.Values)
            {
                yield return dataInPort.Pull(flow);
            }
            SetData(flow);
        }

        private void SetData(Flow flow)
        {
            foreach (var port in _dataOutPorts.Values)
            {
                flow.Set(port);
            }
        }

        public override string ToString()
        {
            return $"{name}[{Id}]";
        }

        #region API
        protected virtual void Setup() {}
        #endregion
    }
}