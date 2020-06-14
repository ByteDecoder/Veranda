using System;
using System.Collections;
using System.Reflection;
using RedOwl.Core;
using Sirenix.OdinInspector;

namespace RedOwl.Sleipnir.Engine
{
    public abstract class FlowAttribute : Attribute
    {
        public PortIO Io;
        public FieldInfo Field;
    }
    
    [IncludeMyAttributes, PropertyOrder(-100)]
    [AttributeUsage(AttributeTargets.Field)]
    public class FlowInAttribute : FlowAttribute {}
    
    [IncludeMyAttributes, PropertyOrder(100)]
    [AttributeUsage(AttributeTargets.Field)]
    public class FlowOutAttribute : FlowAttribute {}
    
    public interface IFlowPort : IPort
    {
        event Action OnExecute;
        void Initialize(INode node, FlowAttribute attr);
        void Trigger(IGraphFlow flow);
        bool HasSuccession { get;  }
        Func<IGraphFlow, IFlowPort> SerialSuccession { get; }
        bool IsAsync { get; }
        Func<IGraphFlow, IEnumerator> AsyncSuccession { get; }
    }
    
    [Serializable, InlineProperty, HideReferenceObjectPicker]
    public class FlowPort : IFlowPort
    {
        public INode Node { get; private set; }
        public PortIO Io { get; private set; }
        public string Name { get; private set; }
        [ShowInInspector, HideLabel]
        public string Id { get; private set; }

        public event Action OnExecute;
        public bool HasSuccession { get; private set; }
        public Func<IGraphFlow, IFlowPort> SerialSuccession { get; private set; }
        public bool IsAsync { get; private set; }
        public Func<IGraphFlow, IEnumerator> AsyncSuccession { get; private set; }

        public FlowPort() {}
        
        internal FlowPort(IFlowPort symmetrical)
        {
            Node = symmetrical.Node;
            Io = symmetrical.Io.IsOutput() ? PortIO.In : PortIO.Out;
            Name = symmetrical.Name;
            Id = RedOwlHash.GetHashId($"{Node.Id}.{Name}.{Io}");
            SerialSuccession = (flow) => symmetrical;
        }

        public IFlowPort CreateSymmetrical() => new FlowPort(this);

        public void Initialize(INode node, FlowAttribute attr)
        {
            Node = node;
            Io = attr.Io;
            Name = attr.Field.Name;
            Id = RedOwlHash.GetHashId($"{Node.Id}.{Name}.{Io}");
            
        }

        public void Trigger(IGraphFlow flow)
        {
            OnExecute?.Invoke();
        }

        public void SetCallback(Action callback)
        {
            OnExecute += callback;
        }

        public void Succession(Func<IGraphFlow, IFlowPort> flow)
        {
            HasSuccession = true;
            IsAsync = false;
            SerialSuccession = flow;
        }

        public void Succession(Func<IGraphFlow, IEnumerator> flow)
        {
            HasSuccession = true;
            IsAsync = true;
            AsyncSuccession = flow;
        }

        public override string ToString()
        {
            return $"{Name}.{Io}";
        }
    }

    // TODO: Replace attribute usage with these
    // public class FlowInPort : FlowPort, IFlowInPort {}
    //
    // public class FlowOutPort : FlowPort, IFlowOutPort {}
    //
    // public class FlowInOutPort : FlowPort, IFlowInPort, IFlowOutPort {}
    
}