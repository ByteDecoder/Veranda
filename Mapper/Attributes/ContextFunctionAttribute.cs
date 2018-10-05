using System;

namespace Sleipnir.Mapper
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ContextFunctionAttribute : Attribute
    {
        public string Name;
    }
}