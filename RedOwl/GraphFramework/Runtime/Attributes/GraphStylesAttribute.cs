using System;
using UnityEngine;

namespace RedOwl.GraphFramework
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class GraphStylesAttribute : Attribute
    {
        public string path;

        public GraphStylesAttribute(string path)
        {
	        this.path = path;
        }
    }
}