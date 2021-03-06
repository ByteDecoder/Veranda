using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RedOwl.Veranda
{
    [Serializable]
    [HideReferenceObjectPicker, InlineProperty]
    public class ExitNode : Node, IFlowInNode
    {
        [SerializeField, HideInInspector]
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