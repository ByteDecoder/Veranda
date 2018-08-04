#if UNITY_EDITOR
using System;

namespace Sleipnir.Wrapper.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class Connection : Attribute
    {
        public float WindowWidth = 64;
        public bool ExpandedByDefault = false;
    }
}
#endif