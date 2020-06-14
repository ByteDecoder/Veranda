using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace RedOwl.Sleipnir.Engine
{
    public interface IGraph
    {
        string Name { get;  }

        int NodeCount { get; }
        IEnumerable<INode> Nodes { get; }

        IEnumerable<Type> PossibleNodes { get; }
        void Initialize();

        IEnumerator Execute(IFlowRootNode node);

        IEnumerable<TNode> GetNodes<TNode>() where TNode : INode;
        T GetFirstNode<T>() where T : INode;
        INode GetNode(string id);
        
        TNode Add<TNode>() where TNode : INode, new();
        TNode Add<TNode>(TNode node) where TNode : INode;
        void Link<T>(IFlowInNode inNode) where T : IFlowOutNode;
        void Link<T>(FlowIn inPort) where T : IFlowOutNode;
        void Link<T>(IFlowOutNode outNode) where T : IFlowInNode;
        void Link<T>(FlowOut outPort) where T : IFlowInNode;
        void Link(FlowPort outPort, FlowPort inPort);
        void Link<TValue>(DataPort<TValue> outPort, DataPort<TValue> inPort);
        void Link(IFlowOutNode outNode, IFlowInNode inNode);
        void Link(IFlowOutNode startNode, IFlowInNode endNode, params IFlowNode[] flow);
    }
    
    [Serializable]
    public abstract class Graph<TNode, TFlow> : IGraph where TNode : INode where TFlow : IGraphFlow, new()
    {
        [SerializeReference] 
        private List<INode> _nodes;
        private Dictionary<string, INode> _nodeTable;

        #region IGraph
        public string Name => GetType().Name;

        public int NodeCount => _nodes.Count;
        public IEnumerable<INode> Nodes => _nodes;
        public IEnumerable<Type> PossibleNodes => Sleipnir.Nodes.Find<TNode>();
        #endregion
        
        protected Graph()
        {
            var start = new StartNode();
            start.Initialize(this);
            var update = new UpdateNode();
            update.Initialize(this);
            _nodes = new List<INode> { start, update };
            EnsureNodeTable();
            _nodeTable.Add(start.Id, start);
            _nodeTable.Add(update.Id, update);
        }

        private void EnsureNodeTable()
        {
            if (_nodeTable != null) return;
            _nodeTable = new Dictionary<string, INode>(_nodes.Count);
        }

        public void Initialize()
        {
            EnsureNodeTable();
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
            // TODO: Don't execute of node isn't connect?
            //Debug.Log($"Beginning Flow on: '{Name}' at RootNode '{node}'");
            yield return new TFlow().Execute(this, node);
        }
        
        public IEnumerable<T> GetNodes<T>() where T : INode
        {
            foreach (var node in _nodes)
            {
                if (node is T value) yield return value;
            }
        }
        
        public T GetFirstNode<T>() where T : INode
        {
            foreach (var node in _nodes)
            {
                if (node is T value) return value;
            }
            throw new Exception($"There was no node of type '{typeof(T)}' found within this graph!");
        }

        public INode GetNode(string id)
        {
            return _nodeTable[id];
        }

        public T Add<T>() where T : INode, new() => Add(new T());
        public T Add<T>(T node) where T : INode
        {
            EnsureNodeTable();
            node.Initialize(this);
            _nodes.Add(node);
            _nodeTable.Add(node.Id, node);
            return node;
        }
        
        public void Link<T>(IFlowInNode inNode) where T : IFlowOutNode
        {
            foreach (var node in _nodes)
            {
                if (node is T value) Link(value.FlowOut, inNode.FlowIn);
            }
        }
        
        public void Link<T>(FlowIn inPort) where T : IFlowOutNode
        {
            foreach (var node in _nodes)
            {
                if (node is T value) Link(value.FlowOut, inPort);
            }
        }
        
        public void Link<T>(IFlowOutNode outNode) where T : IFlowInNode
        {
            foreach (var node in _nodes)
            {
                if (node is T value) Link(outNode.FlowOut, value.FlowIn);
            }
        }
        
        public void Link<T>(FlowOut outPort) where T : IFlowInNode
        {
            foreach (var node in _nodes)
            {
                if (node is T value) Link(outPort, value.FlowIn);
            }
        }

        public void Link(FlowPort outPort, FlowPort inPort)
        {
            outPort.Node.Link(outPort, inPort);
        }
        
        public void Link<TValue>(DataPort<TValue> outPort, DataPort<TValue> inPort)
        {
            inPort.Node.Link(outPort, inPort);
        }

        public void Link(IFlowOutNode outNode, IFlowInNode inNode)
        {
            outNode.Link(outNode.FlowOut, inNode.FlowIn);
        }

        public void Link(IFlowOutNode startNode, IFlowInNode endNode, params IFlowNode[] flow)
        {
            int count = flow.Length;
            startNode.Link(startNode.FlowOut, flow[0].FlowIn);
            for (int i = 1; i < count; i++)
            {
                flow[i - 1].Link(flow[i - 1].FlowOut, flow[i].FlowIn);
            }
            flow[count - 1].Link(flow[count - 1].FlowOut, endNode.FlowIn);
        }
    }
    
    [Serializable]
    public abstract class Graph<TNode> : Graph<TNode, GraphFlow> where TNode : INode {}
}