using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine
{
    public interface IFlowRootNode : INode
    {
        //IEnumerator Run(IFlowPort port, GraphFlow graphFlow);
        //IEnumerator Pull(IDataConnection port, GraphFlow graphFlow);
        //IEnumerator Pull(IDataPort port, GraphFlow graphFlow);    
    }
    
    [Serializable]
    public class FlowRootNode : Node, IFlowRootNode
    {
        [SerializeField]
        protected FlowOut flowOut;

        public FlowOut FlowOut => flowOut;

        protected override void Setup()
        {
            base.Setup();
            flowOut.SetCallback(OnExit);
        }

        // API
        public virtual void OnExit() {}
    }
}