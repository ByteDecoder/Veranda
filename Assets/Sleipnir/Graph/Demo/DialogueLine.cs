using System;
using System.Collections.Generic;
using Sleipnir.Graph.Attributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Sleipnir.Graph.Demo
{
    [Serializable]
    [NodeWidth(300)]
    [TitleColor(1, 1, 1)]
    [HeaderColor(0.3f, 0.3f, 0.7f)]
    [Title("Hyyype!")]
    [LabelSlider(true)]
    [LabelWidth(100)]
    [HeaderKnob(KnobType.Output)]
    public class DialogueLine
    {
        [FieldKnob(KnobType.Both)]
        public Character Character;
        public string Text;
        public AudioClip AudioClip;
        public KillerLine Line;
        
        [CollectionKnob(KnobType.Input, 
            R = 0.4f, G = 0.5f, B = 0.8f, 
            Description = "In")]
        public List<float> List = new List<float>();

        [OnGraphLoad]
        public void Load()
        {
            Debug.Log("Hello!");
        }
        
        [OnDraw]
        public void Draw(Sleipnir.Node node)
        {
            var r = node.HeaderColor.r + 0.001f;
            node.HeaderColor = new Color(r, r, r);
            node.TitleColor = new Color(1 - r, 1 - r, 1 - r);
        }

        [OnKnobsUpdate]
        public void OnConnectionsUpdate(Sleipnir.Node node)
        {
            foreach (var knob in node.Knobs)
                knob.Color = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
        }
        
        [OnChanged]
        public void OnValueChanged(Sleipnir.Node node)
        {
            node.HeaderColor = new Color(0, 0, 0);
        }

        [ContextFunction]
        public void SayNodeHello()
        {
            Debug.Log("Hello!");
        }
    }
}