using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine
{
    public interface IEnterNode
    {
        bool ActivateOnStart { get; }
    }
    
    [Serializable]
    [HideReferenceObjectPicker, InlineProperty]
    public class EnterNode : Node, IFlowOutNode, IEnterNode
    {
        [SerializeField]
        private bool activateOnStart;

        public bool ActivateOnStart => activateOnStart;

        [FlowOut(nameof(OnExit))]
        protected FlowPort flowOut;

        public FlowPort FlowOut => flowOut;
        
        public EnterNode()
        {
            flowOut = new FlowPort();
        }

        public virtual void OnExit() {}
    }
}