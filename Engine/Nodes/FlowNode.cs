using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RedOwl.Veranda
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
        [SerializeField, HideInInspector]
        protected FlowIn flowIn = new FlowIn();
        public FlowIn FlowIn => flowIn;

        [SerializeField, HideInInspector]
        protected FlowOut flowOut = new FlowOut();
        public FlowOut FlowOut => flowOut;
        #endregion

        protected override void Setup()
        {
            flowIn.SetCallback(OnEnter);
            flowIn.Succession(Succession);
            flowOut.SetCallback(OnExit);
        }
        
        protected virtual IEnumerator Succession(IGraphFlow flow)
        {
            yield return flowOut;
        }

        #region API
        protected virtual void OnEnter() {}
        protected virtual void OnExit() {}
        #endregion

    }
}