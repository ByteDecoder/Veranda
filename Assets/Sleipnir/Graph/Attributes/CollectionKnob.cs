using System;

namespace Sleipnir.Graph.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class CollectionKnob : Knob
    {
        public CollectionKnob(KnobType type) : base (type) { }
    }
}