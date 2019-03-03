using System;
using UnityEngine;

namespace RedOwl.GraphFramework
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
	public class NodeTitleAttribute : Attribute
    {
        public string title;

        public NodeTitleAttribute(string title)
        {
            this.title = title;
        }
    }
}