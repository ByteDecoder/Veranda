using System;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine
{
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
}