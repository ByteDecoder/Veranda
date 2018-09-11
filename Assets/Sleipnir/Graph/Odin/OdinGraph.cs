using System;
using System.Collections.Generic;
using Sirenix.Serialization;
using UnityEngine;

namespace Sleipnir.Graph.Odin
{
    [Serializable]
    public abstract class OdinGraph<TNodeContent> : Graph<OdinNode<TNodeContent>, TNodeContent>
        where TNodeContent : ScriptableObject
    {
        [HideInInspector, OdinSerialize]
        private List<OdinNode<TNodeContent>> _nodes = new List<OdinNode<TNodeContent>>();

        public override List<OdinNode<TNodeContent>> Nodes
        {
            get { return _nodes; }
            set { _nodes = value; }
        }
    }
}