using System;

namespace Sleipnir
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ContextFunctionAttribute : Attribute
    {
        public string Name;
    }
}