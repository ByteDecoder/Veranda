using System;

namespace RedOwl.GraphFramework
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class PortDirectionAttribute : Attribute
    {
        public PortDirections direction;

        public PortDirectionAttribute(PortDirections direction = PortDirections.None)
        {
            this.direction = direction;
        }
    }
}