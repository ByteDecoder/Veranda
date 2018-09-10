using System;
using UnityEngine;

namespace Sleipnir.Graph.Unity
{
    [Serializable]
    public class UnityNode<TContent> : GraphNode<TContent>
    {
        [HideInInspector, SerializeField]
        private TContent _content;
        
        [HideInInspector, SerializeField]
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