using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RedOwl.Sleipnir
{
    [Serializable]
    [HideReferenceObjectPicker, InlineProperty]
    public class DebugLogNode : Node, IFlowInNode
    {
        [SerializeField]
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