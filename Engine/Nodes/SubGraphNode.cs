using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine
{
    [Serializable, HideReferenceObjectPicker, InlineProperty]
    public class SubGraphNode : BaseNode, INode, IFlowInNode, IFlowOutNode
    {
        public GraphReference Reference;
        [SerializeReference]
        public IGraph Data;

        [FlowIn(nameof(OnEnter))]
        protected FlowPort flowIn;
        public FlowPort FlowIn => flowIn;

        [FlowOut(nameof(OnExit))]
        protected FlowPort flowOut;
        public FlowPort FlowOut => flowOut;

        public void Initialize()
        {
            // Build / Reconcile Ports
            // Graph Nodes that should become ports are built here
            Debug.Log($"SubGraphNode Initialized!");
        }


        #region API
        public virtual void OnEnter() {}
        public virtual void OnExit() {}
        #endregion
    }
}