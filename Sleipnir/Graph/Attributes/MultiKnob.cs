using System;

namespace Sleipnir.Graph.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class MultiKnob : Attribute
    {
        public KnobType Type;

        public MultiKnob(KnobType type) 
        {
            Type = type;
        }
    }
}