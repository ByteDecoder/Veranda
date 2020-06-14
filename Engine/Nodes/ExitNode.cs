using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine
{
    [Serializable]
    [HideReferenceObjectPicker, InlineProperty]
    public class ExitNode : Node, IFlowInNode
    {
        [SerializeField]
        protected FlowIn flowIn;

        public FlowIn FlowIn => flowIn;

        protected override void Setup()
        {
            base.Setup();
            flowIn.SetCallback(OnEnter);
        }

        public virtual void OnEnter() {}
    }
}