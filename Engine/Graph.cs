using System;
using System.Collections.Generic;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine
{
    public interface IGraph
    {
        string Name { get;  }
        EnterNode EnterNode { get; }
        ExitNode ExitNode { get; }
        
        IEnumerable<INode> Nodes { get; }

        IEnumerable<Type> PossibleNodes { get; }
        void Initialize();
        void Start(MonoBehaviour behaviour);
        
        INode GetNode(string id);

        TNode Add<TNode>() where TNode : INode, new();
        TNode Add<TNode>(TNode node) where TNode : INode;
        void Link(IFlowOutNode outNode, IFlowInNode inNode);
        void Link(FlowPort outPort, FlowPort inPort);
        void Link<TValue>(DataPort<TValue> outPort, DataPort<TValue> inPort);
        void CreateFlow(params IFlowNode[] flow);
    }
    
    [Serializable]
    public abstract class Graph<T> : IGraph where T : INode
    {
        [SerializeReference] private List<INode> _nodes;
        private Dictionary<string, INode> _nodeTable;

        #region IGraph
        public string Name => GetType().Name;
        public EnterNode EnterNode => (EnterNode)_nodes[0];
        public ExitNode ExitNode => (ExitNode)_nodes[1];
        public IEnumerable<INode> Nodes => _nodes;
        public IEnumerable<Type> PossibleNodes => Sleipnir.Nodes.Find<T>();
        #endregion
        
        protected Graph()
        {
            _nodes = new List<INode>
            {
                new EnterNode(),
                new ExitNode()
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
        
        public void Start(MonoBehaviour behaviour)
        {
            foreach (INode node in _nodes)
            {
                if (!node.IsRoot) continue;
                behaviour.StartCoroutine(node.StartFlow(new Flow()));
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

        public void Link(IFlowOutNode outNode, IFlowInNode inNode)
        {
            Link(outNode.FlowOut, inNode.FlowIn);
        }
        
        public void Link(FlowPort outPort, FlowPort inPort)
        {
            outPort.Node.Link(outPort, inPort);
        }
        
        public void Link<TValue>(DataPort<TValue> outPort, DataPort<TValue> inPort)
        {
            inPort.Node.Link(outPort, inPort);
        }

        public void CreateFlow(params IFlowNode[] flow)
        {
            int count = flow.Length;
            Link(EnterNode, flow[0]);
            for (int i = 1; i < count; i++)
            {
                Link(flow[i - 1], flow[i]);
            }
            Link(flow[count - 1], ExitNode);
        }


    }
}