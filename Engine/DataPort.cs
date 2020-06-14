using System;
using System.Collections;
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
        object Data { get; set; }
        void Initialize(INode node, DataAttribute attr);
    }
    
    [Serializable]
    public class DataPort : IDataPort
    {
        public INode Node { get; private set; }
        public PortIO Io { get; private set; }
        public string Name { get; private set; }
        
        public string Id { get; private set; }

        public object Data { get; set; }

        public DataPort() {}
        
        internal DataPort(IDataPort symmetrical)
        {
            Node = symmetrical.Node;
            Io = symmetrical.Io.IsOutput() ? PortIO.In : PortIO.Out;
            Name = symmetrical.Name;
            Id = RedOwlHash.GetHashId($"{Node.Id}.{Name}.{Io}");
        }

        public DataPort CreateSymmetrical() => new DataPort(this);
        
        public void Initialize(INode node, DataAttribute attr)
        {
            Node = node;
            Io = attr.Io;
            Name = attr.Field.Name;
            Id = RedOwlHash.GetHashId($"{Node.Id}.{Name}.{Io}");
        }

        public override string ToString()
        {
            return $"{Name}.{Io}";
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
            get => _value = (T)Data;
            set => Data = _value = value;
        }

        public void OnBeforeSerialize()
        {
            Data = _value;
        }

        public void OnAfterDeserialize()
        {
            Data = _value;
        }
    }
    
    // TODO: Replace attribute usage with these
    // public class DataInPort<T> : DataPort<T>, IDataInPort {}
    //
    // public class DataOutPort<T> : DataPort<T>, IDataOutPort {}
    //
    // public class DataInOutPort<T> : DataPort<T>, IDataInPort, IDataOutPort {}
}