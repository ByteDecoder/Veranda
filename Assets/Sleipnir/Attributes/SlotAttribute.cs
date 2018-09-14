using System;

namespace Sleipnir
{
    public abstract class SlotAttribute : Attribute
    {
        public SlotDirection Direction;

        protected SlotAttribute(SlotDirection direction)
        {
            Direction = direction;
        }
    }
    
    [AttributeUsage(AttributeTargets.Field)]
    public class SlotInputAttribute : SlotAttribute
    {
        public SlotInputAttribute() : base(SlotDirection.Input) {}
    }
    
    [AttributeUsage(AttributeTargets.Field)]
    public class SlotOutputAttribute : SlotAttribute
    {
        public SlotOutputAttribute() : base(SlotDirection.Output) {}
    }
    
    [AttributeUsage(AttributeTargets.Field)]
    public class SlotInOutAttribute : SlotAttribute
    {
        public SlotInOutAttribute() : base(SlotDirection.InOut) {}
    }
}