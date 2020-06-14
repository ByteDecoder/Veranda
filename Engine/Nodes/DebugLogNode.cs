using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine
{
    [Serializable]
    [HideReferenceObjectPicker, InlineProperty]
    public class DebugLogNode : Node, IFlowInNode
    {
        [SerializeField]
        [FlowIn] 
        protected FlowPort flowIn;

        public FlowPort FlowIn => flowIn;

        [DataIn]
        public DataPort<string> Message;

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