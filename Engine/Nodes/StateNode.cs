using System;
using System.Collections;
using System.Collections.Generic;
using RedOwl.Veranda;
using UnityEngine;

namespace RedOwl.Veranda
{
    public interface IStateBehaviour
    {
        bool OnEnter();
        bool OnUpdate();
        void OnExit();
    }
    
    [Serializable]
    public abstract class StateBehaviour : IStateBehaviour
    {
        /// <summary>
        /// Called when the state node is entered - return true to exit the state node immediately
        /// </summary>
        public virtual bool OnEnter() { return false; }
        
        /// <summary>
        /// Called when the state node's tick is entered - return true to exit the state node immediately
        /// </summary>
        public virtual bool OnUpdate() { return false; }
        
        /// <summary>
        /// Called when the state node is exiting
        /// </summary>
        public virtual void OnExit() {}
    }
    
    [Serializable]
    [NodeSettings(Width = 100, Height = 100)]
    public class StateNode : FlowNode
    {
        [SerializeReference] public List<IStateBehaviour> behaviours = new List<IStateBehaviour>();

        private bool _isActive;

        protected override IEnumerator Succession(IGraphFlow flow)
        {
            while (_isActive)
            {
                foreach (var behaviour in behaviours)
                {
                    if (behaviour.OnUpdate())
                    {
                        _isActive = false;
                        break;
                    }
                }
                yield return null;
            }

            yield return FlowOut;
        }

        protected override void OnEnter()
        {
            foreach (var behaviour in behaviours)
            {
                if (behaviour.OnEnter()) return;
            }
            _isActive = true;
        }

        protected override void OnExit()
        {
            foreach (var behaviour in behaviours)
            {
                behaviour.OnExit();
            }
            _isActive = false;
        }
    }
}