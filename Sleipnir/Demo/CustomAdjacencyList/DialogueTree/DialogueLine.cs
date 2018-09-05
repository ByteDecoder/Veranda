using System;
using UnityEngine;

namespace Sleipnir.Demo.CustomAdjacencyList.DialogueTree
{
    [Serializable]
    public class DialogueLine
    {
        public Character Character;
        [Multiline]
        public string Text;
        public AudioClip AudioClip;

        public override string ToString()
        {
            return Character == null ? "Dialogue Node" : Character.ToString();
        }
    }
}