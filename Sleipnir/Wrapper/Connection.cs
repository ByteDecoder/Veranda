#if UNITY_EDITOR
using System;
using Sirenix.Serialization;
using Sirenix.Utilities;
using UnityEngine;

namespace Sleipnir.Wrapper
{
    [Serializable]
    public class Connection : IConnection
    {
        [HideInInspector] public int DrawingOrder;
        [OdinSerialize, HideInInspector] private readonly Knob _outputKnob;
        [OdinSerialize, HideInInspector] private readonly Knob _inputKnob;
        [OdinSerialize, HideInInspector] private readonly bool _doesHaveValue;
        [OdinSerialize, HideInInspector] private readonly float _valueWindowWidth;
        [OdinSerialize, HideInInspector] private bool _isExpanded;
        [OdinSerialize, HideInInspector] private object _value;

        public IKnob OutputKnob => _outputKnob;
        public IKnob InputKnob => _inputKnob;
        public bool DoesHaveValue => _doesHaveValue;
        public float ValueWindowWidth => _valueWindowWidth;

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { _isExpanded = value; }
        }

        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public Connection(Knob outputKnob, Knob inputKnob)
        {
            DrawingOrder = 0;
            _outputKnob = outputKnob;
            _inputKnob = inputKnob;
            _value = null;
            _doesHaveValue = false;
            _isExpanded = false;
            _valueWindowWidth = 0;
        }

        public Connection(Knob outputKnob, Knob inputKnob, object value)
        {
            DrawingOrder = 0;
            _outputKnob = outputKnob;
            _inputKnob = inputKnob;
            _value = value;
            _doesHaveValue = true;

            var attribute = value.GetType().GetCustomAttribute<Attributes.Connection>();
            attribute = attribute ?? new Attributes.Connection();

            _isExpanded = attribute.ExpandedByDefault;
            _valueWindowWidth = attribute.WindowWidth;
        }
    }
}
#endif