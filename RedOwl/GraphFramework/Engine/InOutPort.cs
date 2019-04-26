using System;

namespace RedOwl.GraphFramework
{
    [PortDirection(PortDirections.InOut)]
    public class InOutPort<T> : Port<T>
    {
        public InOutPort() : base(PortDirections.InOut) {}
        public InOutPort(T defaultValue) : base(defaultValue, PortDirections.InOut) {}
    }
}
