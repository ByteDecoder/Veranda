using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine
{
    public interface IFlowInNode : INode
    {
        FlowIn FlowIn { get; }
    }

    public interface IFlowOutNode : INode
    {
        FlowOut FlowOut { get; }
    }
    
    public interface IFlowNode : IFlowInNode, IFlowOutNode {}

    [Serializable, HideReferenceObjectPicker, InlineProperty]
    public abstract class FlowNode : Node, IFlowNode
    {
        #region IFlowNode
        [SerializeField]
        protected FlowIn flowIn;
        public FlowIn FlowIn => flowIn;

        [SerializeField]
        protected FlowOut flowOut;
        public FlowOut FlowOut => flowOut;
        #endregion

        protected override void Setup()
        {
            flowIn.SetCallback(OnEnter);
            flowIn.Succession((flow) => flowOut);
            flowOut.SetCallback(OnExit);
        }

        #region API
        public virtual void OnEnter() {}
        public virtual void OnExit() {}
        #endregion

    }
}