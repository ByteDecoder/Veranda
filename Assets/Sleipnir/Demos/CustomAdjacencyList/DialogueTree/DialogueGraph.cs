using System;

namespace Sleipnir.Demo.CustomAdjacencyList.DialogueTree
{
    // This generic abomination is unfortunately needed for serialization with Unity.
    [Serializable]
    public class DialogueGraph : CustomAdjacencyList<Node, DialogueLine, DialogueConnection, Condition> { }
}