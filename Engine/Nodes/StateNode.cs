using System;
using System.Collections;
using System.Collections.Generic;
using RedOwl.Core;
using UnityEngine;

namespace RedOwl.Veranda
{
    public class ExitState : CustomYieldInstruction
    {
        public override bool keepWaiting => false;
    }
    
    public interface IStateBehaviour
    {
        IEnumerator OnEnter();
        IEnumerator OnUpdate();
        IEnumerator OnExit();
    }
    
    [Serializable]
    public abstract class StateBehaviour : IStateBehaviour
    {
        public virtual IEnumerator OnEnter() { yield break; }
        
        public virtual IEnumerator OnUpdate() { yield break; }
        public virtual IEnumerator OnExit() { yield break; }
    }
    
    [Serializable]
    [NodeSettings(Width = 100, Height = 100)]
    public class StateNode : FlowNode
    {
        [SerializeReference] public List<IStateBehaviour> behaviours = new List<IStateBehaviour>();

        private bool _isActive;

        protected override IEnumerator Succession(IGraphFlow flow)
        {
            _isActive = true;
            foreach (var behaviour in behaviours)
            {
                yield return HandleTarget(behaviour.OnEnter());
            }

            while (_isActive)
            {
                foreach (var behaviour in behaviours)
                {
                    yield return HandleTarget(behaviour.OnUpdate());
                }
            }

            foreach (var behaviour in behaviours)
            {
                yield return HandleTarget(behaviour.OnExit());
            }
            yield return FlowOut;
        }

        private IEnumerator HandleTarget(IEnumerator target)
        {
            while (target.MoveNext())
            {
                if (target.Current is ExitState)
                {
                    _isActive = false;
                    yield break;
                }
            
                yield return target.Current;
            }
        }
    }
}