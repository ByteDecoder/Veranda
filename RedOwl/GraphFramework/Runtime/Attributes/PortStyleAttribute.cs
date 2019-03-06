using System;

namespace RedOwl.GraphFramework
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class PortStyleAttribute : Attribute
    {
        public PortStyles style;

        public PortStyleAttribute(PortStyles style = PortStyles.Single)
        {
            this.style = style;
        }
    }
}