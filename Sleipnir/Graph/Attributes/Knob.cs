using System;
using UnityEngine;

namespace Sleipnir.Graph.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Struct)]
    public class Knob : Attribute
    {
        public KnobType Type;
        public string Description;
        public float R = 1;
        public float G = 1;
        public float B = 1;
        public float A = 1;

        public Color Color => new Color(R, G, B, A);

        public Knob(KnobType type)
        {
            Type = type;
        }
    }
}
