using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine
{
    public interface ISubGraphNode
    {
        IEnumerable<FlowPort> GetDynamicFlowPorts();
        IEnumerable<DataPort> GetDynamicDataPorts();
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
            Graph.Initialize();
        }

        public IEnumerable<FlowPort> GetDynamicFlowPorts()
        {
            foreach (var node in Graph.Nodes)
            {
                switch (node)
                {
                    case EnterNode enterNode:
                        Debug.Log($"Creating Graph Enter Node Symmetrical Port: '{node}'");
                        yield return enterNode.FlowOut.CreateSymmetrical();
                        break;
                    case ExitNode exitNode:
                        Debug.Log($"Creating Graph Exit Node Symmetrical Port: '{node}'");
                        yield return exitNode.FlowIn.CreateSymmetrical();
                        break;
                }
            }
        }

        public IEnumerable<DataPort> GetDynamicDataPorts()
        {
            foreach (var node in Graph.Nodes)
            {
                switch (node)
                {
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