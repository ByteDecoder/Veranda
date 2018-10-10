using System;

namespace Sleipnir
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class LabelAttribute : Attribute
    {
        public float Width;
        public bool? IsShown;

        public LabelAttribute(float width)
        {
            Width = width;
        }

        public LabelAttribute(bool isShown)
        {
            IsShown = isShown;
        }

        public LabelAttribute(float width, bool isShown)
        {
            Width = width;
            IsShown = isShown;
        }
    }
}