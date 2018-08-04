#if UNITY_EDITOR
using System;
using Sirenix.Serialization;
using UnityEngine;

namespace Sleipnir.Wrapper
{
    [Serializable]
    public class Knob : IKnob
    {
        [OdinSerialize, HideInInspector] public object Content;
        [OdinSerialize, HideInInspector] private readonly string _description;
        [OdinSerialize, HideInInspector] private readonly KnobType _type;
        [OdinSerialize, HideInInspector] private readonly Color _color;

        public string Description => _description;
        public Color Color => _color;
        public KnobType Type => _type;

        public Knob(string description, Color color, KnobType type, object content)
        {
            Content = content;
            _description = description;
            _color = color;
            _type = type;
        }
    }
}
#endif