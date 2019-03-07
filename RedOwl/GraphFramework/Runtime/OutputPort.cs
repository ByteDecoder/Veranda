using System;

namespace RedOwl.GraphFramework
{
    [PortDirection(PortDirections.Output)]
    public class OutputPort<T> : Port<T>
    {
        public OutputPort() : base(PortDirections.Output) {}
        public OutputPort(T defaultValue) : base(defaultValue, PortDirections.Output) {}
    }
}
