using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine
{
    internal interface IPort
    {
        string Name { get; set; }
        string Id { get; set; }
        INode Node { get; set; }
    }

    [Serializable, HideInInspector]
    public class FlowPort : IPort
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public INode Node { get; set; }
        
        public string Method { get; set; }
        
        public void Invoke() {}

        public override string ToString()
        {
            return $"{Node.Name}[{Node.Id}].{Name}[{Id}]";
        }
    }

    [Serializable, InlineProperty, HideReferenceObjectPicker]
    public class DataPort<T> : IPort
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public INode Node { get; set; }
        [ShowInInspector, HideLabel] public T Value;

        public override string ToString()
        {
            return $"{Node.Name}[{Node.Id}].{Name}[{Id}]";
        }
    }
}