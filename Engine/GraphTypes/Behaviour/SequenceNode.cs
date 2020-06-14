using System;
using System.Collections;
using RedOwl.Sleipnir.Engine;

namespace RedOwl.Sleipnir.Graphs.Behaviour
{
    [Serializable]
    public class SequenceNode : BehaviourNode
    {
        protected override IEnumerator Succession(IGraphFlow flow)
        {
            // TODO: Figure out how to make "left most" Succession work
            return base.Succession(flow);
        }
    }
}