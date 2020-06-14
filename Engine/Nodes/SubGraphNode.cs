using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine
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
            // TODO: Ensure Graph has Enter & Exit nodes
            // TODO: Properly Create Succession between Enter & Exit node's ports and the dynamically created symmetrical ports
            Graph.Initialize();
        }

        public IEnumerable<IFlowPort> GetDynamicFlowPorts()
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

        public IEnumerable<IDataPort> GetDynamicDataPorts()
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