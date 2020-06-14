using System;
using System.Collections;
using RedOwl.Sleipnir.Engine;
using UnityEngine;

namespace RedOwl.Sleipnir.Graphs.Behaviour
{
    [Serializable]
    public abstract class BehaviourNode : FlowNode
    {
        #region IFlowNode
        [SerializeField]
        protected FlowIn flowIn;
        public FlowIn FlowIn => flowIn;

        [SerializeField]
        protected FlowOut flowOut;
        public FlowOut FlowOut => flowOut;
        #endregion

        protected override void Setup()
        {
            flowIn.SetCallback(OnEnter);
            flowIn.Succession(Succession);
            flowOut.SetCallback(OnExit);
        }

        protected virtual IEnumerator Succession(IGraphFlow flow)
        {
            yield return flowOut;
        }
    }
}