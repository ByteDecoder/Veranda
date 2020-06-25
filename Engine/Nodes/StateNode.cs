using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedOwl.Veranda
{
    public class ExitState : CustomYieldInstruction
    {
        public override bool keepWaiting => false;
    }
    
    public interface IStateBehaviour
    {
        void Setup();
        IEnumerator OnEnter();
        IEnumerator OnUpdate();
        IEnumerator OnExit();
    }
    
    [Serializable]
    public abstract class StateBehaviour : IStateBehaviour
    {
        public virtual void Setup() {}
        public virtual IEnumerator OnEnter() { yield break; }
        
        public virtual IEnumerator OnUpdate() { yield break; }
        public virtual IEnumerator OnExit() { yield break; }
    }
    
    [Serializable]
    [NodeSettings(Width = 100, Height = 100)]
    public class StateNode : FlowNode
    {
        [SerializeReference] public List<IStateBehaviour> behaviours;

        private bool _isActive;
        private bool _noBehaviours;

        public StateNode()
        {
            behaviours = new List<IStateBehaviour>();
        }

        public StateNode(params IStateBehaviour[] initialBehaviours)
        {
            behaviours = new List<IStateBehaviour>(initialBehaviours);
        }

        protected override void Setup()
        {
            base.Setup();
            _noBehaviours = behaviours.Count == 0;
            foreach (var behaviour in behaviours)
            {
                behaviour.Setup();
            }
        }

        protected override IEnumerator Succession(IGraphFlow flow)
        {
            _isActive = true;
            foreach (var behaviour in behaviours)
            {
                yield return HandleTarget(behaviour.OnEnter());
            }

            while (_isActive)
            {
                if (_noBehaviours) yield return new ExitState();
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