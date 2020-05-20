using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine
{
    [Serializable, HideReferenceObjectPicker, InlineProperty]
    public class EnterNode : BaseNode, INode, IFlowOutNode
    {
        [FlowOut(nameof(OnExit))]
        protected FlowPort flowOut;
        public FlowPort FlowOut => flowOut;

        public void Initialize()
        {
            
        }
        
        public virtual void OnExit() {}
    }
}