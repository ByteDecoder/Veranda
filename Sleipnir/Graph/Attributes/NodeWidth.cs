using System;

namespace Sleipnir.Graph.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class NodeWidth : Attribute
    {
        public float Width;

        public NodeWidth(float width)
        {
            Width = width;
        }
    }
}