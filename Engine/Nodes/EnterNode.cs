using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RedOwl.Veranda
{
    [Serializable]
    [HideReferenceObjectPicker, InlineProperty]
    public class EnterNode : Node, IFlowOutNode
    {
        [SerializeField, HideInInspector]
        protected FlowOut flowOut;

        public FlowOut FlowOut => flowOut;

        protected override void Setup()
        {
            base.Setup();
            flowOut.SetCallback(OnExit);
        }

        public virtual void OnExit() {}
    }
}