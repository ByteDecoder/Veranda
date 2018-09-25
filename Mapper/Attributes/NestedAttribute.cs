using System;

namespace Sleipnir.Mapper
{
    [AttributeUsage(AttributeTargets.Field)]
    public class NestedAttribute : Attribute
    {
        public bool DisplayWhenHidden = false;
    }
}