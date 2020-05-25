using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine
{
    [Serializable, HideReferenceObjectPicker, InlineProperty]
    public class SubGraphNode : Node
    {
        public GraphReference Reference;
        [SerializeReference, HideInInspector]
        public IGraph Data;
        
        public IGraph Graph => Reference == null ? Data : Reference.graph;
        
        protected override void Setup()
        {
            Graph.Initialize();

            Debug.Log($"Create SubGraph '{Graph.Name}' Symmetrical Flow Ports");
            _flowPorts.Add(Graph.EndNode.FlowIn.CreateSymmetrical());
            _flowPorts.Add(Graph.StartNode.FlowOut.CreateSymmetrical());

            foreach (var node in Graph.Nodes)
            {
                switch (node)
                {
                    case IGraphInput graphInput:
                        Debug.Log($"Creating Graph Input Node Symmetrical Port: '{node}'");
                        _dataPorts.Add(graphInput.Data.CreateSymmetrical());
                        break;
                    case IGraphOutput graphOutput:
                        Debug.Log($"Creating Graph Output Node Symmetrical Port: '{node}'");
                        _dataPorts.Add(graphOutput.Data.CreateSymmetrical());
                        break;
                }
            }
        }
    }
}