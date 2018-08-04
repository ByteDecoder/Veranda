#if UNITY_EDITOR
using System;
using UnityEngine;

namespace Sleipnir.Wrapper.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field, AllowMultiple = true)]
    public class Knob : Attribute
    {
        public KnobType Type { get; }
        public float Red = 1;
        public float Green = 1;
        public float Blue = 1;
        public float Alpha = 1;
        public Color Color => new Color(Red, Green, Blue, Alpha);
        public string Description;

        public Knob(KnobType type)
        {
            Type = type;
        }
    }
}
#endif