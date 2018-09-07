using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Sleipnir.Demo.CustomAdjacencyList.DialogueTree
{
    [ExecuteInEditMode] // This attribute is a very lazy workaround and a truly ugly hack.
    public class DialogueTreeDemo : MonoBehaviour
    {
        public DialogueGraph DialogueGraph = new DialogueGraph();

        // Please mind that right now removing the current node from graph won't reset this.
        [HideInInspector]
        public int CurrentNode = -1;

#if UNITY_EDITOR
        public Color CurrentNodeHeaderColor = Color.blue;

        private void Update()
        {
            var i = 0;
            foreach (var node in DialogueGraph)
            {
                var nodeIndex = i;
                node.EditorNode.ContextMenuFunctions = new List<Tuple<string, Action>>
                {
                    new Tuple<string, Action>("Set current node", () =>
                    {
                        CurrentNode = nodeIndex;
                        UpdateCurrentNode();
                    })
                };
                i++;
            }
        }
#endif

        public int GetNext()
        {
            var node = DialogueGraph[CurrentNode];
            foreach (var connection in node.Connections)
            {
                var content = connection.Content;
                if (content == null)
                    return connection.TargetIndex;

                if (connection.Content.Value)
                    return connection.TargetIndex;
            }
            return -1;
        }

        [Button]
        public void ProcessDialogue()
        {
            if (CurrentNode == -1)
                return;
            
            var nodeContent = DialogueGraph[CurrentNode].Content;
            Debug.Log($"{nodeContent.Character} : {nodeContent.Text}");
            if(nodeContent.AudioClip != null)
            Debug.Log($"Playing {nodeContent.AudioClip}");
            CurrentNode = GetNext();

#if UNITY_EDITOR
            UpdateCurrentNode();
#endif
        }

#if UNITY_EDITOR
        private void UpdateCurrentNode()
        {
            foreach (var node in DialogueGraph)
                node.EditorNode.HeaderColor = DialogueGraph.BaseColor;

            if (CurrentNode >- 1)
                DialogueGraph[CurrentNode].EditorNode.HeaderColor = CurrentNodeHeaderColor;
        }
#endif
    }
}