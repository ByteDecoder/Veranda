using System;
using UnityEngine;

namespace RedOwl.GraphFramework
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
	public class NodeColorAttribute : Attribute
    {
        public Color color;

        public NodeColorAttribute(float r, float g, float b, float a = 1f)
        {
	        this.color = new Color(r, g, b, a);
        }
        
	    public NodeColorAttribute(byte r, byte g, byte b, byte a = 255)
	    {
		    this.color = new Color32(r, g, b, a);
	    }
    }
}