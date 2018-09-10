using System;

namespace Sleipnir.Graph.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class HeaderKnob : Knob
    {
        public HeaderKnob(KnobType type) : base(type) { }
    }
}