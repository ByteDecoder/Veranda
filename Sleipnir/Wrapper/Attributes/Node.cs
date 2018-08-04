#if UNITY_EDITOR
using System;

namespace Sleipnir.Wrapper.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]  
    public class Node : Attribute
    {
        public float Width = 128f;
    }
}
#endif