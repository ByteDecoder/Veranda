using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Sleipnir.Demo.CustomAdjacencyList
{
    [Serializable]
    public class Node<TContent, TConnection, TConnectionContent> 
        where TConnection : Connection<TConnectionContent>
        where TContent : new()
    {
        [HideLabel, HideReferenceObjectPicker]
        public TContent Content = new TContent();
        [HideReferenceObjectPicker, OnValueChanged("UpdateKnobs", true)]
        public List<TConnection> Connections = new List<TConnection>();
        
#if UNITY_EDITOR
        #region Sleipnir data
        [SerializeField, HideInInspector]
        private Node _editorNode;

        public Node EditorNode
        {
            get { return _editorNode; }
            set { _editorNode = value; }
        }

        public Knob OutputKnob(int index) => _editorNode.Knobs[index + 1];
        public Knob InputKnob() => _editorNode.Knobs[0];
        public int OutputKnobIndex(Knob knob) => _editorNode.Knobs.IndexOf(knob) - 1;

        private void UpdateKnobs()
        {
            _editorNode.Knobs = new List<Knob>(Connections.Count)
            {
                new Knob(50, KnobType.Input)
                {
                    Color = new Color(0.1f, 0.4f, 0.6f),
                }
            };

            var knobYPosition = 50f;
            foreach (var connection in Connections)
            { 
                _editorNode.Knobs.Add(
                    new Knob(knobYPosition, KnobType.Output)
                    {
                        Color = new Color(0.1f, 0.4f, 0.6f),
                        
                        Description = connection == null || connection.Content == null 
                        ? null
                        : connection.Content.ToString()
                    });
                knobYPosition += 24;
            }
        }
        #endregion
#endif
    }
}