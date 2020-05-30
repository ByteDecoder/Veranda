using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using RedOwl.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine
{
    public abstract class DataAttribute : Attribute
    {
        public PortIO Io;
        public FieldInfo Field;
    }
    
    [IncludeMyAttributes, PropertyOrder(-10)]
    [AttributeUsage(AttributeTargets.Field)]
    public class DataInAttribute : DataAttribute {}
    
    [IncludeMyAttributes, PropertyOrder(10)]
    [AttributeUsage(AttributeTargets.Field)]
    public class DataOutAttribute : DataAttribute {}
    
    [AttributeUsage(AttributeTargets.Field)]
    public class DataInOutAttribute : DataAttribute {}
    
    public interface IDataPort : IPort
    {
        object Data { get; }
        bool IsExit { get; }
        IEnumerator Pull(GraphFlow graphFlow);
    }
    
    [Serializable]
    public class DataPort : IDataPort
    {
        public INode Node { get; private set; }

        public string Id { get; private set; }

        public PortIO Io { get; private set; }

        public bool IsExit => Io.IsOutput();

        [SerializeField, HideInInspector]
        protected object data;
        public object Data => data;
        
        public DataPort() {}
        
        internal DataPort(DataPort symmetrical)
        {
            Node = symmetrical.Node;
            Io = symmetrical.IsExit ? PortIO.In : PortIO.Out;
            Id = RedOwlHash.GetHashId($"{symmetrical.Id}.symmetrical");
        }

        public DataPort CreateSymmetrical() => new DataPort(this);
        
        internal void Initialize(INode node, DataAttribute attr)
        {
            Node = node;
            Id = RedOwlHash.GetHashId($"{node.Id}.{attr.Field.Name}.{attr.Io}");
            Io = attr.Io;
        }

        public IEnumerator Pull(GraphFlow graphFlow)
        {
            throw new NotImplementedException();
            // foreach (var connection in Node.GetDataConnections(Id))
            // {
            //     yield return Node.Pull(connection, graphFlow);
            // }
        }

        public override string ToString()
        {
            return $"{Id}.{Io}";
        }
    }
    
    [Serializable, InlineProperty, HideReferenceObjectPicker]
    public class DataPort<T> : DataPort, ISerializationCallbackReceiver
    {
        [SerializeField, HideInInspector]
        private T _value;

        [ShowInInspector, HideLabel]
        public T Value
        {
            get => _value;
            set
            {
                data = value;
                _value = value;
            }
        }

        public void OnBeforeSerialize()
        {
            data = _value;
        }

        public void OnAfterDeserialize()
        {
            data = _value;
        }
    }
}