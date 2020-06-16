using System;
using System.Collections;
using System.Reflection;
using RedOwl.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RedOwl.Veranda
{
    public interface IDataPort : IPort
    {
        object Data { get; set; }
        void Initialize(INode node, PortInfo attr);
    }
    
    public interface IDataInPort : IDataPort {}
    public interface IDataOutPort : IDataPort {}

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
        
        public void Initialize(INode node, PortInfo info)
        {
            Node = node;
            Io = info.Io;
            Name = info.Field.Name;
            Id = RedOwlHash.GetHashId($"{Node.Id}.{Name}.{Io}");
        }

        public override string ToString()
        {
            return $"{Name}.{Io}";
        }
    }
    
    [Serializable, InlineProperty, HideReferenceObjectPicker]
    public abstract class DataPort<T> : DataPort, ISerializationCallbackReceiver
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
    
    [Serializable]
    public class DataIn<T> : DataPort<T>, IDataInPort {}
    [Serializable]
    public class DataOut<T> : DataPort<T>, IDataOutPort {}
    [Serializable]
    public class DataInOut<T> : DataPort<T>, IDataInPort, IDataOutPort {}
}