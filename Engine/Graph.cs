using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine
{
    public interface IGraph
    {
        string Name { get;  }
        EnterNode EnterNode { get; }
        ExitNode ExitNode { get; }
        
        int NodeCount { get; }
        IEnumerable<INode> Nodes { get; }

        IEnumerable<Type> PossibleNodes { get; }
        void Initialize();

        IEnumerator Execute(IFlowRootNode node);

        IEnumerable<TNode> GetNodes<TNode>() where TNode : INode;
        INode GetNode(string id);
        
        TNode Add<TNode>() where TNode : INode, new();
        TNode Add<TNode>(TNode node) where TNode : INode;
        void Link(FlowPort outPort, FlowPort inPort);
        void Link<TValue>(DataPort<TValue> outPort, DataPort<TValue> inPort);
        void Link(params IFlowNode[] flow);
    }
    
    [Serializable]
    public abstract class Graph<TNode, TFlow> : IGraph where TNode : INode where TFlow : IGraphFlow, new()
    {
        [SerializeReference] private List<INode> _nodes;
        private Dictionary<string, INode> _nodeTable;

        #region IGraph
        public string Name => GetType().Name;
        public EnterNode EnterNode => (EnterNode)_nodes[0];
        public ExitNode ExitNode => (ExitNode)_nodes[1];

        public int NodeCount => _nodes.Count;
        public IEnumerable<INode> Nodes => _nodes;
        public IEnumerable<Type> PossibleNodes => Sleipnir.Nodes.Find<TNode>();
        #endregion
        
        protected Graph()
        {
            _nodes = new List<INode>
            {
                new StartNode(),
                new UpdateNode()
            };
        }

        public void Initialize()
        {
            _nodeTable = new Dictionary<string, INode>(_nodes.Count);
            foreach (var node in _nodes)
            {
                //Debug.Log($"Initializing Graph Node: '{node}'");
                node.Initialize(this);
                _nodeTable.Add(node.Id, node);
            }
            //Debug.Log($"GraphType '{GetType().Name}' Initialized!");
        }

        public IEnumerator Execute(IFlowRootNode node)
        {
            yield return new TFlow().Execute(this, node);
        }
        
        public IEnumerable<TNode> GetNodes<TNode>() where TNode : INode
        {
            foreach (var node in _nodes)
            {
                if (node is TNode value) yield return value;
            }
        }

        public INode GetNode(string id)
        {
            return _nodeTable[id];
        }

        public TNode Add<TNode>() where TNode : INode, new() => Add(new TNode());
        public TNode Add<TNode>(TNode node) where TNode : INode
        {
            _nodes.Add(node);
            return node;
        }

        public void Link(FlowPort outPort, FlowPort inPort)
        {
            outPort.Node.Link(outPort, inPort);
        }
        
        public void Link<TValue>(DataPort<TValue> outPort, DataPort<TValue> inPort)
        {
            inPort.Node.Link(outPort, inPort);
        }

        public void Link(params IFlowNode[] flow)
        {
            int count = flow.Length;
            Link(EnterNode.FlowOut, flow[0].FlowIn);
            for (int i = 1; i < count; i++)
            {
                Link(flow[i - 1].FlowOut, flow[i].FlowIn);
            }
            Link(flow[count - 1].FlowOut, ExitNode.FlowIn);
        }
    }
    
    [Serializable]
    public abstract class Graph<TNode> : Graph<TNode, GraphFlow> where TNode : INode {}
}