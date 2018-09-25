using System;

namespace Sleipnir.Mapper
{
    [Flags]
    public enum Direction
    {
        Input = 1 << 0,
        Output = 1 << 1,

        InOut = Input | Output
    }

    public static class SlotDirectionExtensions
    {
        public static bool IsInput(this Direction self)
        {
            return self.HasFlag(Direction.Input);
        }

        public static bool IsOutput(this Direction self)
        {
            return self.HasFlag(Direction.Output);
        }
    }

    public class SlotAttribute : Attribute
    {
        public bool DisplayWhenHidden = false;
        public Direction Direction;

        public SlotAttribute(Direction direction)
        {
            Direction = direction;
        }
    }
}