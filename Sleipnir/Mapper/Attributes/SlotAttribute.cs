using System;

namespace Sleipnir
{
    [Flags]
    public enum SlotDirection
    {
        Input = 1 << 0,
        Output = 1 << 1,

        InOut = Input | Output
    }

    public static class SlotDirectionExtensions
    {
        public static bool IsInput(this SlotDirection self)
        {
            return self.HasFlag(SlotDirection.Input);
        }

        public static bool IsOutput(this SlotDirection self)
        {
            return self.HasFlag(SlotDirection.Output);
        }
    }

    public class SlotAttribute : Attribute
    {
        public SlotDirection Direction;

        public SlotAttribute(SlotDirection direction)
        {
            Direction = direction;
        }
    }
}