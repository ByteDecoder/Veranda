using UnityEngine;

namespace RedOwl.GraphFramework
{
    public abstract class GraphOutput<T> : Node, IGraphPort
    {
        public string Label;

        public InputPort<T> Data = new InputPort<T>();
    }
}
