using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine
{
    /*
Flow Port Needs
- Be given to a "graph.Link" method
- Create Symmetrical Port
- Store and "ID" use for adjacency list
- IsDynamicallyCreated


Data Port Needs
- Serialize T Data
- Be given to a "graph.Link" method
- Create Symmetrical Port
- Store an "ID" used for adjacency list
- IsDynamicallyCreated
*/
    
    public interface IPort
    {
        string Id { get; }
        PortIO Io { get;  }
        INode Node { get; }
    }
    
    public interface IDataConnection
    {
        string Port { get; }
        
        string TargetNode { get; }
        string TargetPort { get; }
    }
    
    [Serializable]
    public class DataConnection : IDataConnection
    {
        [SerializeField]
        private string port;
        public string Port => port;
        [SerializeField]
        private string targetNode;
        public string TargetNode => targetNode;
        [SerializeField]
        private string targetPort;
        public string TargetPort => targetPort;
        
        public DataConnection(IPort start, IPort target)
        {
            port = start.Id;
            targetNode = target.Node.Id;
            targetPort = target.Id;
        }
    }

    public interface IFlowConnection
    {
        string Port { get; }
        IEnumerator Run(IGraph graph, Flow flow);
    }

    [Serializable]
    public class FlowConnection : IFlowConnection
    {
        [SerializeField]
        private string port;
        public string Port => port;
        [SerializeField]
        private string targetNode;
        [SerializeField]
        private string targetPort;

        public FlowConnection(IPort start, IPort target)
        {
            port = start.Id;
            targetNode = target.Node.Id;
            targetPort = target.Id;
        }

        public IEnumerator Run(IGraph graph, Flow flow)
        {
            var nextNode = graph.GetNode(targetNode);
            yield return nextNode.Run(nextNode.GetFlowPort(targetPort), flow);
        }
    }

    public class Flow
    {
        private Dictionary<string, object> _data;

        public Flow()
        {
            _data = new Dictionary<string, object>();
        }

        public T Get<T>(DataPort<T> port)
        {
            if (_data.TryGetValue(port.Id, out object value))
            {
                return (T) value;
            }

            return default;
        }

        internal void Set(IDataPort port)
        {
            _data[port.Id] = port.Data;
        }
    }
}