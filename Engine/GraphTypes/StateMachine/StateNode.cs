using System;
using System.Collections;
using System.Collections.Generic;
using RedOwl.Sleipnir.Engine;
using UnityEngine;

namespace RedOwl.Sleipnir.Graphs.StateMachine
{
    [Serializable]
    [NodeSettings(Width = 100, Height = 100)]
    public class StateNode : Node, IFlowNode
    {
        public FlowIn tick;
        
        #region IFlowNode
        [SerializeField]
        protected FlowIn flowIn;
        public FlowIn FlowIn => flowIn;

        [SerializeField]
        protected FlowOut flowOut;
        public FlowOut FlowOut => flowOut;
        #endregion

        [SerializeReference] public List<IStateBehaviour> behaviours;

        private bool _isActive;

        protected override void Setup()
        {
            flowIn.SetCallback(OnEnter);
            flowIn.Succession(Succession);
            flowOut.SetCallback(OnExit);
            tick.SetCallback(OnUpdate);
        }

        private IEnumerator Succession(IGraphFlow flow)
        {
            while (_isActive)
            {
                yield return new WaitForEndOfFrame();
            }

            yield return FlowOut;
        }

        private void OnEnter()
        {
            foreach (var behaviour in behaviours)
            {
                if (behaviour.OnEnter()) return;
            }
            _isActive = true;
        }
        
        private void OnUpdate()
        {
            if (!_isActive) return;
            foreach (var behaviour in behaviours)
            {
                if (behaviour.OnUpdate())
                {
                    _isActive = false;
                    return;
                }
            }
        }

        private void OnExit()
        {
            foreach (var behaviour in behaviours)
            {
                behaviour.OnExit();
            }

            _isActive = false;
        }
    }
}