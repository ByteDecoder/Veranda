using System;
using Sirenix.Serialization;
using UnityEngine;

namespace Sleipnir.Graph.Odin
{
    [Serializable]
    public class OdinNode<TContent> : Node<TContent>
    {
        [HideInInspector, OdinSerialize]
        private TContent _content;

        [HideInInspector, OdinSerialize]
        private Node _editorNode = new Node();
        
        public override TContent Content
        {
            get { return _content; }
            set { _content = value; }
        }
        
        public override Node EditorNode
        {
            get { return _editorNode; }
            set { _editorNode = value; }
        }
    }
}