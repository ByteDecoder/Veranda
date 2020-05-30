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
        bool IsExit { get; }
        event Action OnExecute;
        Func<IGraphFlow, IFlowPort> FlowSerial { get; }
        Func<IGraphFlow, IEnumerator> FlowAsync { get; }
        IEnumerator Run(IGraphFlow graphFlow);
    }
    
    [Serializable]
    public class FlowPort : IFlowPort
    {
        public INode Node { get; private set; }

        public string Id { get; private set; }

        public PortIO Io { get; private set; }

        public bool IsExit => Io.IsOutput();
        public event Action OnExecute;
        public Func<IGraphFlow, IFlowPort> FlowSerial { get; private set; }
        public bool IsAsync { get; private set; }
        public Func<IGraphFlow, IEnumerator> FlowAsync { get; private set; }

        public FlowPort() {}
        
        internal FlowPort(FlowPort symmetrical)
        {
            Node = symmetrical.Node;
            Io = symmetrical.IsExit ? PortIO.In : PortIO.Out;
            Id = RedOwlHash.GetHashId($"{Node.Id}.symmetrical");
            FlowSerial = (flow) => symmetrical;
        }

        public FlowPort CreateSymmetrical() => new FlowPort(this);

        internal void Initialize(INode node, FlowAttribute attr)
        {
            Node = node;
            Id = RedOwlHash.GetHashId($"{node.Id}.{attr.Field.Name}.{attr.Io}");
            Io = attr.Io;
        }

        public void SetCallback(Action callback)
        {
            OnExecute += callback;
        }

        public void SetFlow(Func<IGraphFlow, IFlowPort> flow)
        {
            IsAsync = false;
            FlowSerial = flow;
        }

        public void SetFlow(Func<IGraphFlow, IEnumerator> flow)
        {
            IsAsync = true;
            FlowAsync = flow;
        }

        public IEnumerator Run(IGraphFlow graphFlow)
        {
            OnExecute?.Invoke();
            throw new NotImplementedException();
            // if (!IsAsync)
            // {
            //     var nextFlowPort = port.FlowSerial(flow);
            //     SetData(flow);
            //     yield return nextFlowPort.Run(flow);
            // }
            // else
            // {
            //     while (port.FlowAsync.MoveNext())
            //     {
            //         if (port.FlowAsync.Current is IFlowPort nextFlowPort)
            //         {
            //             SetData(flow);
            //             yield return nextFlowPort.Run(flow);
            //         }
            //         yield return port.FlowAsync.Current;
            //     }
            // }
            //
            
        }
        
        public override string ToString()
        {
            return $"{Id}.{Io}";
        }
    }
}