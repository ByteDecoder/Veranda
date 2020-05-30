using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine
{
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
    public abstract class FlowNode : Node, IFlowNode
    {
        #region IFlowNode
        [SerializeField]
        [FlowIn]
        protected FlowPort flowIn;
        public FlowPort FlowIn => flowIn;

        [SerializeField]
        [FlowOut]
        protected FlowPort flowOut;
        public FlowPort FlowOut => flowOut;
        #endregion

        protected override void Setup()
        {
            flowIn.SetCallback(OnEnter);
            flowOut.SetCallback(OnExit);
        }

        #region API
        public virtual void OnEnter() {}
        public virtual void OnUpdate() {}
        public virtual void OnExit() {}
        #endregion

    }
}