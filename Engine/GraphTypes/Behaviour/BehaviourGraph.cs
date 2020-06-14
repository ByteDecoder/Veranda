using System.Collections.Generic;
using RedOwl.Sleipnir.Engine;

namespace RedOwl.Sleipnir.Graphs.Behaviour
{
    public class BehaviourGraph : Graph<BehaviourNode, BehaviourGraphFlow> { }

    
    public class BehaviourGraphFlow : GraphFlow {}

    // Needs ability to know which nodes in the flow are "left most"
    // Needs control of the flow class to properly implement "left most" flow execution
}