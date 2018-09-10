using System;

namespace Sleipnir.Graph.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class LabelWidth : Attribute
    {
        public float Width;

        public LabelWidth(float width)
        {
            Width = width;
        }
    }
}