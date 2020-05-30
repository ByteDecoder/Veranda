using System.Collections;
using System.Collections.Generic;

namespace RedOwl.Sleipnir.Engine
{
    public interface IGraphFlow
    {
        IEnumerator Execute(IGraph graph, IFlowRootNode node);
        
        T Get<T>(DataPort<T> port);
        void Set(IDataPort port);
        void Complete(string id);
    }

    public class GraphFlow : IGraphFlow
    {
        protected internal Dictionary<string, object> Data;
        protected internal Dictionary<string, bool> NodeState;

        public IEnumerator Execute(IGraph graph, IFlowRootNode rootNode)
        {
            // Prepare Data
            int count = graph.NodeCount;
            Data = new Dictionary<string, object>(count);
            NodeState = new Dictionary<string, bool>(count);
            foreach (var node in graph.Nodes)
            {
                NodeState[node.Id] = false;
            }
            
            // Begin Execution of OutPorts
            // foreach (var port in rootNode.FlowOutPorts)
            // {
            //     yield return port.Run(this);
            // }
            // Decide which connections to follow and in what order
            // foreach (var connection in node.GetFlowConnections(id))
            // {
            //     yield return connection.Run(node.Graph, flow);
            // }
            yield break;
        }

        public T Get<T>(DataPort<T> port)
        {
            if (Data.TryGetValue(port.Id, out object value))
            {
                return (T) value;
            }

            return default;
        }

        public void Set(IDataPort port)
        {
            Data[port.Id] = port.Data;
        }

        public void Complete(string id)
        {
            NodeState[id] = true;
        }
    }
}