using System;

namespace RedOwl.GraphFramework
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
	public class NodeWidthAttribute : Attribute
    {
        public float width;

        public NodeWidthAttribute(float width)
        {
            this.width = width;
        }
    }
}