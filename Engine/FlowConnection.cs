using System;
using UnityEngine;

namespace RedOwl.Veranda
{
    public interface IFlowConnection
    {
        string Port { get; }
        string TargetNode { get; }
        string TargetPort { get; }
    }
    
    [Serializable]
    public class FlowConnection : IFlowConnection
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

        public FlowConnection(IPort start, IPort target)
        {
            port = start.Id;
            targetNode = target.Node.Id;
            targetPort = target.Id;
        }
    }
}