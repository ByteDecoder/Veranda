using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using RedOwl.Core;
using Sirenix.OdinInspector;
using UnityEngine;

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
        Func<Flow, IFlowPort> FlowSerial { get; }
        Func<Flow, IEnumerator> FlowAsync { get; }
        IEnumerator Run(Flow flow);
    }
    
    [Serializable]
    public class FlowPort : IFlowPort
    {
        public INode Node { get; private set; }

        public string Id { get; private set; }

        public PortIO Io { get; private set; }

        public bool IsExit => Io.IsOutput();

        public Func<Flow, IFlowPort> FlowSerial { get; private set; }
        public bool IsAsync { get; private set; }
        public Func<Flow, IEnumerator> FlowAsync { get; private set; }

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
            
            //On = (Action)Delegate.CreateDelegate(typeof(Action), Node, Method);
            //BindSymmetrical();
            // TODO: Hookup Adjacent Delegates?
        }

        public void SetCallback(Action callback)
        {
            
        }

        public void SetFlow(Func<Flow, IFlowPort> flow)
        {
            IsAsync = false;
            FlowSerial = flow;
        }

        public void SetFlow(Func<Flow, IEnumerator> flow)
        {
            IsAsync = true;
            FlowAsync = flow;
        }

        // private void BindSymmetrical()
        // {
        //     if (_symmetrical == null) return;
        //     switch (Io)
        //     {
        //         case PortIO.In:
        //             _symmetrical.On += Invoke;
        //             break;
        //         case PortIO.Out:
        //             On += _symmetrical.Invoke;
        //             break;
        //     }
        // }
        //
        // public void Invoke()
        // {
        //     On?.Invoke();
        // }
        //

        public IEnumerator Run(Flow flow)
        {
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
            // foreach (var connection in node.GetFlowConnections(id))
            // {
            //     yield return connection.Run(node.Graph, flow);
            // }
        }
        
        public override string ToString()
        {
            return $"{Id}.{Io}";
        }
    }
}