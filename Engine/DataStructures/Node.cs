using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine.DataStructures
{
    [Serializable]
    public class Vertex<T>
    {
        [SerializeField]
        private T _data;
        [SerializeReference]
        private VertexList<T> _neighbors = null;

        public Vertex() {}
        public Vertex(T data) : this(data, null) {}
        public Vertex(T data, VertexList<T> neighbors)
        {
            this._data = data;
            this._neighbors = neighbors;
        }

        public T Value
        {
            get => _data;
            set => _data = value;
        }

        protected VertexList<T> Neighbors
        {
            get => _neighbors;
            set => _neighbors = value;
        }
    }

    [Serializable]
    public class VertexList<T> : Collection<Vertex<T>>
    {
        public VertexList() : base() { }

        public VertexList(int initialSize)
        {
            // Add the specified number of items
            for (int i = 0; i < initialSize; i++)
                Items.Add(default);
        }

        public Vertex<T> FindByValue(T value)
        {
            // search the list for the value
            foreach (var node in Items)
                if (node.Value.Equals(value))
                    return node;

            // if we reached here, we didn't find a matching node
            return null;
        }
    }
    
    [Serializable]
    public abstract class Node<TNode, TValue> where TNode : Node<TNode, TValue>
    {
        [SerializeField]
        private TValue _value;
        [SerializeReference]
        private List<TNode> _neighbors;

        public Node()
        {
            _value = default;
            _neighbors = new List<TNode>();
        }

        public Node(TValue value, List<TNode> neighbors = null)
        {
            _value = value;
            _neighbors = neighbors ?? new List<TNode>();
        }

        public TValue Value
        {
            get => _value;
            set => _value = value;
        }

        public List<TNode> Neighbors
        {
            get => _neighbors;
            set => _neighbors = value;
        }
    }
    
    /*[Serializable]
    public class GraphNode : Node<GraphNode, int>
    {

        public GraphNode() : base() { }
        public GraphNode(int value, List<GraphNode> neighbors = null) : base(value, neighbors) { }
    }*/
    
    public class GraphNode<T> : Vertex<T>
    {
        private List<int> costs;

        public GraphNode() : base() { }
        public GraphNode(T value) : base(value) { }
        public GraphNode(T value, VertexList<T> neighbors) : base(value, neighbors) { }

        new public VertexList<T> Neighbors
        {
            get
            {
                if (base.Neighbors == null)
                    base.Neighbors = new VertexList<T>();

                return base.Neighbors;
            }            
        }

        public List<int> Costs
        {
            get
            {
                if (costs == null)
                    costs = new List<int>();

                return costs;
            }
        }
    }
}
