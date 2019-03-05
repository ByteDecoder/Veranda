using System;

namespace RedOwl.GraphFramework
{
    public class OutputPort<T> : Port<T>
    {
        public OutputPort() : base(PortDirections.Output) {}
        public OutputPort(T defaultValue) : base(defaultValue, PortDirections.Output) {}
        public OutputPort(T defaultValue, PortStyles style) : base(defaultValue, PortDirections.Output, style) {}
    }
}
