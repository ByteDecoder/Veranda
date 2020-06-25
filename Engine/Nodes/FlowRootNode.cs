using System;
using UnityEngine;

namespace RedOwl.Veranda
{
    public interface IFlowRootNode : INode { }
    
    [Serializable]
    public abstract class FlowRootNode : Node, IFlowRootNode, IFlowOutNode
    {
        [SerializeField, HideInInspector]
        protected FlowOut flowOut;

        public FlowOut FlowOut => flowOut;

        protected override void Setup()
        {
            base.Setup();
            flowOut.SetCallback(OnExit);
        }

        // API
        public virtual void OnExit() {}
    }
}