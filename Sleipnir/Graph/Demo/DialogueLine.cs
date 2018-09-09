using System;
using System.Collections.Generic;
using Sleipnir.Graph.Attributes;
using UnityEngine;

namespace Sleipnir.Graph.Demo
{
    [Serializable]
    [NodeWidth(300)]
    [TitleColor(1, 1, 1)]
    [HeaderColor(0.3f, 0.3f, 0.7f)]
    [Title("Hyyype!")]
    [LabelSlider]
    [LabelWidth(100)]
    [Attributes.Knob(KnobType.Both)]
    public class DialogueLine
    {
        public Character Character;
        
        public string Text;
        
        public AudioClip AudioClip;
        
        public int NextLine = -1;

        [ContextFunction]
        public void SayNodeHello()
        {
            Debug.Log("Hello!");
        }

        [Attributes.Knob(KnobType.Output, Description = "Out")]
        [MultiKnob(KnobType.Input, R = 0.4f, G = 0.5f, B = 0.8f, Description = "In")]
        public List<float> List = new List<float>();
    }
}