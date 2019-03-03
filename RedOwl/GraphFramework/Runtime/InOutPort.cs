using System;

namespace RedOwl.GraphFramework
{
    public class InOutPort<T> : Port<T>
    {
        public InOutPort(T defaultValue) : base(defaultValue, PortDirections.Output) {}
        public InOutPort(T defaultValue, PortStyles style) : base(defaultValue, PortDirections.InOut, style) {}
    }
}
