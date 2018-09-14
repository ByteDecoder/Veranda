using System;

namespace Sleipnir
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SlotAttribute : Attribute
    {
        public SlotDirection Direction;

        public SlotAttribute(SlotDirection direction)
        {
            Direction = direction;
        }
    }
    
    [Flags]
    public enum SlotDirection : int
    {
        None   = 0,
        Input  = 1 << 0,
        Output = 1 << 1,
        
        InOut = Input | Output,
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
}