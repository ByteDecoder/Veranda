using System;

namespace RedOwl.GraphFramework
{
    public class InOutPort<T> : Port<T>
    {
        public InOutPort() : base(PortDirections.InOut) {}
        public InOutPort(T defaultValue) : base(defaultValue, PortDirections.InOut) {}
        public InOutPort(T defaultValue, PortStyles style) : base(defaultValue, PortDirections.InOut, style) {}
    }
}
