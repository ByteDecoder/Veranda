using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine
{
    [Serializable]
    [HideReferenceObjectPicker, InlineProperty]
    public class StartNode : Node, IFlowOutNode
    {
        [SerializeField]
        [FlowOut]
        protected FlowPort flowOut;

        public FlowPort FlowOut => flowOut;

        protected override void Setup()
        {
            base.Setup();
            flowOut.SetCallback(OnExit);
        }

        public virtual void OnExit() {}
    }
}