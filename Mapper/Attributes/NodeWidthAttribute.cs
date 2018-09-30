using System;

namespace Sleipnir.Mapper
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