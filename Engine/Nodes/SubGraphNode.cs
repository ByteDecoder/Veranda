using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RedOwl.Sleipnir
{
    public interface ISubGraphNode
    {
        IEnumerable<IFlowPort> GetDynamicFlowPorts();
        IEnumerable<IDataPort> GetDynamicDataPorts();
    }
    
    [Serializable, HideReferenceObjectPicker, InlineProperty]
    public class SubGraphNode : Node, ISubGraphNode
    {
        public GraphReference reference;
        [SerializeReference, HideInInspector]
        public IGraph data;
        
        public IGraph Graph => reference == null ? data : reference.graph;
        
        protected override void Setup()
        {
            Graph.Ensure<EnterNode>();
            Graph.Ensure<ExitNode>();
            Graph.Initialize();
        }

        public IEnumerable<IFlowPort> GetDynamicFlowPorts()
        {
            foreach (var node in Graph.Nodes)
            {
                FlowPort symmetrical;
                switch (node)
                {
                    case EnterNode enterNode:
                        Debug.Log($"Creating Graph Enter Node Symmetrical Port: '{node}'");
                        symmetrical = enterNode.FlowOut.CreateSymmetrical();
                        symmetrical.Succession(flow => enterNode.FlowOut);
                        yield return symmetrical;
                        break;
                    case ExitNode exitNode:
                        Debug.Log($"Creating Graph Exit Node Symmetrical Port: '{node}'");
                        symmetrical = exitNode.FlowIn.CreateSymmetrical();
                        exitNode.FlowIn.Succession(flow => symmetrical);
                        yield return symmetrical;
                        break;
                }
            }
        }

        public IEnumerable<IDataPort> GetDynamicDataPorts()
        {
            foreach (var node in Graph.Nodes)
            {
                switch (node)
                {
                    // TODO: Make connection for subgraph data ports? How is data traversed
                    case IGraphInput graphInput:
                        Debug.Log($"Creating Graph Input Node Symmetrical Port: '{node}'");
                        yield return graphInput.Data.CreateSymmetrical();
                        break;
                    case IGraphOutput graphOutput:
                        Debug.Log($"Creating Graph Output Node Symmetrical Port: '{node}'");
                        yield return graphOutput.Data.CreateSymmetrical();
                        break;
                }
            }
        }
    }
}