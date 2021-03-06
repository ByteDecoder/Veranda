using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RedOwl.Veranda
{
    [Serializable]
    [HideReferenceObjectPicker, InlineProperty]
    public class DebugLogNode : Node, IFlowInNode
    {
        [SerializeField, HideInInspector]
        protected FlowIn flowIn;

        public FlowIn FlowIn => flowIn;

        public DataIn<string> Message;

        protected override void Setup()
        {
            base.Setup();
            flowIn.SetCallback(OnEnter);
        }

        public virtual void OnEnter()
        {
            Debug.Log(Message.Value);
        }
    }
}