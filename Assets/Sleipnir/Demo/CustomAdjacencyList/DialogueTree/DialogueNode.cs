using System;

namespace Sleipnir.Demo.CustomAdjacencyList.DialogueTree
{
    [Serializable]
    public class Node : Node<DialogueLine, DialogueConnection, Condition> { }
}