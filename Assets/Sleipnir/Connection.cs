using System;
using UnityEngine;

namespace Sleipnir
{
    [Serializable]
    public class Connection
    {
#if UNITY_EDITOR
        public Knob OutputKnob { get; }
        public Knob InputKnob { get; }
        public Vector2 OutputControlPoint;
        public Vector2 InputControlPoint;

        public Connection(Knob outputKnob, Knob inputKnob)
        {
            OutputKnob = outputKnob;
            InputKnob = inputKnob;
            OutputControlPoint = Vector2.right;
            InputControlPoint = Vector2.left;
        }
#endif
    }
}