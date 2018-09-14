using System;

namespace Sleipnir
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class NodeWidthAttribute : Attribute
    {
        public float Width;

        public NodeWidthAttribute(float width)
        {
            Width = width;
        }
    }
}