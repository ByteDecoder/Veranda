using System;

namespace Sleipnir
{
    [AttributeUsage(AttributeTargets.Field)]
    public class KnobAttribute : Attribute
    {
        public KnobDirection Direction;

        public KnobAttribute(KnobDirection direction)
        {
            Direction = direction;
        }
    }

    public enum KnobDirection
    {
        Input,
        Output,
        Both
    }
}