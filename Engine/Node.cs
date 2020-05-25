using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine
{
    public interface INode
    {
        string Name { get; }
        string Id { get; }
        Rect Rect { get; }
        void Initialize();
    }
    
    [Serializable, HideReferenceObjectPicker, InlineProperty]
    public abstract class Node : INode
    {
        [ShowInInspector, DisplayAsString, HideLabel, PropertyOrder(-1000)]
        public string Name => GetType().Name;
        
        [SerializeField]
        [HideInInspector]
        //[DisplayAsString, HideLabel, Title("@Name")]
        private string id;
        
        public string Id => id;
        
        [SerializeField, HideInInspector]//, HideLabel, InlineProperty]
        private Rect rect;
        
        public Rect Rect => rect;

        //[ShowInInspector]
        protected List<DataPort> _dataPorts;
        //[ShowInInspector]
        protected List<FlowPort> _flowPorts;

        protected Node()
        {
            id = Sleipnir.GenerateId();
            rect = Sleipnir.GenerateRect(this);
            var nodeType = GetType();
            foreach (var portPrototype in Sleipnir.Ports.GetDataPorts(nodeType))
            {
                var instance = (DataPort)Activator.CreateInstance(portPrototype.Field.FieldType);
                instance.Name = portPrototype.Name;
                instance.Id = portPrototype.GetHashId(Id);
                instance.Io = portPrototype.Io;
                instance.Node = this;
                portPrototype.Field.SetValue(this, instance);
            }
            foreach (var portPrototype in Sleipnir.Ports.GetFlowPorts(nodeType))
            {
                var instance = (FlowPort)Activator.CreateInstance(portPrototype.Field.FieldType);
                instance.Name = portPrototype.Name;
                instance.Id = portPrototype.GetHashId(Id);
                instance.Io = portPrototype.Io;
                instance.Node = this;
                instance.Method = portPrototype.Method.Name;
                portPrototype.Field.SetValue(this, instance);
            }
        }
        
        public void Initialize()
        {
            var nodeType = GetType();
            var dataPorts = Sleipnir.Ports.GetDataPorts(nodeType);
            var flowPorts = Sleipnir.Ports.GetFlowPorts(nodeType);
            _dataPorts = new List<DataPort>(dataPorts.Count);
            _flowPorts = new List<FlowPort>(flowPorts.Count);
            foreach (var portPrototype in dataPorts)
            {
                _dataPorts.Add((DataPort)portPrototype.Field.GetValue(this));
            }

            foreach (var portPrototype in flowPorts)
            {
                _flowPorts.Add((FlowPort)portPrototype.Field.GetValue(this));
            }
            Setup();
        }

        public override string ToString()
        {
            return $"{Name}[{Id}]";
        }

        #region API
        protected virtual void Setup() {}
        #endregion
    }
}