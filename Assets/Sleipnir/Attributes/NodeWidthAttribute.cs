using System;

namespace Sleipnir
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class NodeWidthAttribute : Attribute
    {
        public float DefaultWidth;
        public float MinWidth;

        public NodeWidthAttribute(float defaultWidth)
        {
            DefaultWidth = defaultWidth;
        }
    }
}