using UnityEngine;

namespace RedOwl.GraphFramework
{
    public abstract class GraphInput<T> : Node, IGraphPort
    {
        public string Label;

        public OutputPort<T> Data = new OutputPort<T>();
    }
}
