using System;
using System.Collections;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine
{
    public interface IFlowConnection
    {
        string Port { get; }
        IEnumerator Run(IGraph graph, GraphFlow graphFlow);
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
        
        public IEnumerator Run(IGraph graph, GraphFlow graphFlow)
        {
            throw new NotImplementedException();
            //var nextNode = graph.GetNode(targetNode);
            //yield return nextNode.Run(nextNode.GetFlowPort(targetPort), graphFlow);
        }
    }
}