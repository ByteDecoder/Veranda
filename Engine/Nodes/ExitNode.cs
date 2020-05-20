using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine
{
    [Serializable, HideReferenceObjectPicker, InlineProperty]
    public class ExitNode : BaseNode, INode, IFlowInNode
    {
        [FlowIn(nameof(OnEnter))]
        protected FlowPort flowIn;
        public FlowPort FlowIn => flowIn;

        public void Initialize()
        {
            
        }
        
        public virtual void OnEnter() {}
    }
}