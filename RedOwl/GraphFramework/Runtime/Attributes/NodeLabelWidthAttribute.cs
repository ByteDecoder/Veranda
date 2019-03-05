using System;

namespace RedOwl.GraphFramework
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
	public class NodeLabelWidthAttribute : Attribute
    {
        public float width;

        public NodeLabelWidthAttribute(float width)
        {
            this.width = width;
        }
    }
}