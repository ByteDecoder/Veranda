using System;
using Sleipnir.Graph.Attributes;
using UnityEngine;

namespace Sleipnir.Graph.Demo
{
    [Serializable,
        NodeWidth(300),
        TitleColor(1, 1, 1),
        HeaderColor(0.3f, 0.3f, 0.7f),
        Title("Hyyype!"),
        LabelSlider,
        Attributes.Knob(KnobType.Both)]
    public class DialogueLine
    {
        [Attributes.Knob(KnobType.Output)]
        public Character Character;

        [Multiline, Attributes.Knob(KnobType.Both)]
        public string Text;

        [Attributes.Knob(KnobType.Input)]
        public AudioClip AudioClip;
        
        [Attributes.Knob(KnobType.Input)]
        public int NextLine = -1;

        [ContextFunction]
        public void SayNodeHello()
        {
            Debug.Log("Hello!");
        }
    }
}