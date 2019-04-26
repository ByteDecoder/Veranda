using UnityEngine;

namespace RedOwl.GraphFramework
{
    public abstract class GraphPortNode : Node
    {
        public string label = "Data";
    }

    public abstract class GraphInput<T> : GraphPortNode
    {
        public OutputPort<T> Data = new OutputPort<T>();
    }

    public abstract class GraphOutput<T> : GraphPortNode
    {
        public InputPort<T> Data = new InputPort<T>();
    }
}
