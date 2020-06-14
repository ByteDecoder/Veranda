using System;
using RedOwl.Sleipnir.Engine;

namespace RedOwl.Sleipnir.Graphs.StateMachine
{
    [Serializable]
    public class StateMachineGraph : Graph<StateNode>
    {
        public new T Add<T>(T node) where T : INode
        {
            base.Add(node);
            if (node is StateNode value) Link<UpdateNode>(value.tick);
            return node;
        }
    }
}