using System;

namespace Sleipnir.Graph.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ContextFunction : Attribute
    {
        public string Name;
    } 
}