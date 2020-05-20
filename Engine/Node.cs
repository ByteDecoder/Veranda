using System;
using RedOwl.Core;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
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
    
    public interface IFlowInNode : INode
    {
        FlowPort FlowIn { get; }
    }

    public interface IFlowOutNode : INode
    {
        FlowPort FlowOut { get; }
    }
    
    public interface IFlowNode : IFlowInNode, IFlowOutNode {}

    [Serializable, HideReferenceObjectPicker, InlineProperty]
    public abstract class BaseNode : INode
    {
        public string Name => GetType().Name;
        
        [SerializeField, DisplayAsString, HideLabel, Title("@Name")]
        private string id;
        
        public string Id => id;
        
        [SerializeField, HideInInspector]//, HideLabel, InlineProperty]
        private Rect rect;
        
        public Rect Rect => rect;

        protected BaseNode()
        {
            id = Sleipnir.GenerateId();
            rect = Sleipnir.GenerateRect(this);
            var nodeType = GetType();
            foreach (var portPrototype in Sleipnir.Ports.GetDataPorts(nodeType))
            {
                var instance = (IPort)Activator.CreateInstance(portPrototype.Info.FieldType);
                instance.Name = portPrototype.Name;
                instance.Id = portPrototype.GetHashId(Id);
                instance.Node = this;
                portPrototype.Info.SetValue(this, instance);
            }
            foreach (var portPrototype in Sleipnir.Ports.GetFlowPorts(nodeType))
            {
                var instance = (IPort)Activator.CreateInstance(portPrototype.Info.FieldType);
                instance.Name = portPrototype.Name;
                instance.Id = portPrototype.GetHashId(Id);
                instance.Node = this;
                portPrototype.Info.SetValue(this, instance);
            }
        }
        
        public void Initialize()
        {
            Setup();
        }
        
        #region API
        protected virtual void Setup() {}
        #endregion
    }
    
    [Serializable, HideReferenceObjectPicker, InlineProperty]
    public abstract class Node : BaseNode, IFlowNode
    {
        #region IFlowNode
        [FlowIn(nameof(OnEnter))]
        protected FlowPort flowIn;
        public FlowPort FlowIn => flowIn;

        [FlowOut(nameof(OnExit))]
        protected FlowPort flowOut;
        public FlowPort FlowOut => flowOut;
        #endregion


        #region API
        public virtual void OnEnter() {}
        public virtual void OnExit() {}
        #endregion

    }
}