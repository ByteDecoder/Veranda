using System;
using System.Reflection;
using Sirenix.OdinInspector;

namespace RedOwl.Sleipnir.Engine
{
    public abstract class Port
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public PortIO Io { get; set; }
        public INode Node { get; set; }
    }
    
    [Serializable]
    public class FlowPort : Port
    {
        public string Method { get; set; }

        private MethodInfo MethodHandle => Sleipnir.Ports.GetFlowPortCallback(Node.GetType(), Method);
        
        public void Invoke() {}

        public override string ToString()
        {
            return $"{Node.Name}[{Node.Id}].{Name}[{Id}].{Method}";
        }

        internal FlowPort CreateSymmetrical()
        {
            return new FlowPort
            {
                Name = $"{Name}Symmetrical",
                Id = Id,
                Io = Io == PortIO.In ? PortIO.Out : PortIO.In,
                Node = Node,
                Method = Method,
            };
        }
    }

    public class DataPort : Port
    {
        public override string ToString()
        {
            return $"{Node.Name}[{Node.Id}].{Name}[{Id}]";
        }
        
        internal DataPort CreateSymmetrical()
        {
            return new DataPort
            {
                Name = $"{Name}Symmetrical",
                Id = Id,
                Io = Io == PortIO.In ? PortIO.Out : PortIO.In,
                Node = Node,
            };
        }
    }

    [Serializable, InlineProperty, HideReferenceObjectPicker]
    public class DataPort<T> : DataPort
    {
        [ShowInInspector, HideLabel] public T Value;
    }
}