using System;

namespace Sleipnir.Graph.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Struct)]
    public class Knob : Attribute
    {
        public KnobType Type;

        public Knob(KnobType type)
        {
            Type = type;
        }
    }
}
