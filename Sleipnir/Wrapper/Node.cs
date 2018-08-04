#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.Serialization;
using Sirenix.Utilities;
using UnityEngine;

namespace Sleipnir.Wrapper
{
    [Serializable]
    public class Node : INode
    {
        [HideInInspector] public int DrawingOrder;
        [OdinSerialize, HideInInspector] private object _value;
        [OdinSerialize, HideInInspector] private Vector2 _position;
        [OdinSerialize, HideInInspector] private readonly Knob[] _knobs;
        [OdinSerialize, HideInInspector] private readonly float _width;

        public Knob[] GetKnobs => _knobs;
        public IReadOnlyList<IKnob> Knobs => _knobs;
        public float Width => _width;

        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public Node(object value)
        {
            DrawingOrder = 0;
            _value = value;

            var type = value.GetType();
            var nodeAttribute = value.GetType().GetCustomAttribute<Attributes.Node>();
            nodeAttribute = nodeAttribute ?? new Attributes.Node();
            _width = nodeAttribute.Width;

            var knobs = value.GetType().GetCustomAttributes<Attributes.Knob>()
                .Select(knob => new Knob(knob.Description, knob.Color, knob.Type, value)).ToList();
            knobs.AddRange(type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .SelectMany(fieldInfo => fieldInfo.GetCustomAttributes(typeof(Attributes.Knob), false)
                .Cast<Attributes.Knob>(),
                    (fieldInfo, attribute) => new Knob(attribute.Description, attribute.Color, attribute.Type,
                        fieldInfo.GetValue(this))));

            _knobs = knobs.ToArray();
        }
        
    }
}
#endif