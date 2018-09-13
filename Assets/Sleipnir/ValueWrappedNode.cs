using System;

namespace Sleipnir
{
    public class ValueWrappedNode
    {
        public Node Node;
        public Func<object> Getter;
        public Action<object> Setter;
    }
}