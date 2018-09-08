using System;

namespace Sleipnir.Demo.CustomAdjacencyList.DialogueTree
{
    [Serializable]
    public class DialogueNode : Node<DialogueLine, DialogueConnection, Condition> { }
}