using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine
{
    [Serializable]
    [HideReferenceObjectPicker, InlineProperty]
    public class ExitNode : Node, IFlowInNode
    {
        [FlowIn(nameof(OnEnter))]
        protected FlowPort flowIn;

        public FlowPort FlowIn => flowIn;
        
        public ExitNode()
        {
            flowIn = new FlowPort();
        }

        public virtual void OnEnter() {}
    }
}