using System;

namespace RedOwl.Sleipnir.Graphs.StateMachine
{
    [Serializable]
    public abstract class StateBehaviour : IStateBehaviour
    {
        /// <summary>
        /// Called when the state node is entered - return true to exit the state node
        /// </summary>
        public virtual bool OnEnter() { return false; }
        
        /// <summary>
        /// Called when the state node's tick is entered - return true to exit the state node
        /// </summary>
        public virtual bool OnUpdate() { return false; }
        
        /// <summary>
        /// Called when the state node is exiting
        /// </summary>
        public virtual void OnExit() {}
    }
}