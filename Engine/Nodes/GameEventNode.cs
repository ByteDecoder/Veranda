using System;
using RedOwl.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RedOwl.Veranda
{
    [Serializable]
    [NodeSettings(Width = 100, Height = 100)]
    public class GameEventNode : Node, IFlowNode, IFlowRootNode
    {
        [LabelText("Event")]
        public GameEvent evt;
        
        #region IFlowNode
        [SerializeField, HideInInspector]
        protected FlowIn flowIn = new FlowIn();
        public FlowIn FlowIn => flowIn;

        [SerializeField, HideInInspector]
        protected FlowOut flowOut = new FlowOut();
        public FlowOut FlowOut => flowOut;
        #endregion

        protected override void Setup()
        {
            if (evt != null) evt.On += StartFlow;
            flowIn.SetCallback(Trigger);
        }

        private void Trigger()
        {
            evt.Raise();
        }

        private void StartFlow()
        {
            CoroutineManager.StartRoutine(Graph.Execute(this));
        }
    }
}