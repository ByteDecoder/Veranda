using System;

namespace RedOwl.GraphFramework
{
    public interface IPort
    {
        string name { get; set; }
        Guid id { get; }
        PortStyles style { get; }
        PortDirections direction { get; }
        Type type { get; }

        object Get();
        void Set(object value);
        bool CanConnectPort(IPort port);
        string ToString();
    }
}
