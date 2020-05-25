using System;
using Sirenix.OdinInspector;

namespace RedOwl.Sleipnir.Engine
{
    public interface IFlowInNode : INode
    {
        FlowPort FlowIn { get; }
    }

    public interface IFlowOutNode : INode
    {
        FlowPort FlowOut { get; }
    }

    public interface IFlowNode : IFlowInNode, IFlowOutNode
    {
        bool Active { get; }

        void OnEnter();
        void OnUpdate();
        void OnExit();
    }

    [Serializable, HideReferenceObjectPicker, InlineProperty]
    public abstract class FlowNode : Node, IFlowNode
    {
        #region IFlowNode
        [FlowIn(nameof(OnEnter))]
        protected FlowPort flowIn;
        public FlowPort FlowIn => flowIn;

        [FlowOut(nameof(OnExit))]
        protected FlowPort flowOut;
        public FlowPort FlowOut => flowOut;

        private bool active;
        public bool Active => active;
        #endregion

        protected FlowNode()
        {
            flowIn = new FlowPort();
            flowOut = new FlowPort();
            active = false;
        }


        #region API
        public virtual void OnEnter() {}
        public virtual void OnUpdate() {}
        public virtual void OnExit() {}
        #endregion

    }
}