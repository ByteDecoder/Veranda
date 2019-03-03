using System;

namespace RedOwl.GraphFramework
{
    public class InputPort<T> : Port<T>
    {
        public InputPort(T defaultValue) : base(defaultValue, PortDirections.Input) {}
        public InputPort(T defaultValue, PortStyles style) : base(defaultValue, PortDirections.Input, style){}
    }
}
