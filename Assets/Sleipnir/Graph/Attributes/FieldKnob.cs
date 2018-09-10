using System;
using Sirenix.OdinInspector;

namespace Sleipnir.Graph.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    [DontApplyToListElements]
    public class FieldKnob : Knob
    { 
        public FieldKnob(KnobType type) : base(type) { }
    }
}
