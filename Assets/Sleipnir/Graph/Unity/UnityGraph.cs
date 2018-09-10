using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sleipnir.Graph.Unity
{
    [Serializable]
    public abstract class UnityGraph<TNode, TNodeContent> : Graph<TNode, TNodeContent> 
        where TNode : UnityNode<TNodeContent>, new()
    {
        [HideInInspector, SerializeField]
        private List<TNode> _nodes = new List<TNode>();

        public override List<TNode> Nodes
        {
            get { return _nodes; }
            set { _nodes = value; }
        }
    }
}