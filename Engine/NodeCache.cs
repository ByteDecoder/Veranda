using System;
using System.Collections.Generic;
using RedOwl.Core;

namespace RedOwl.Sleipnir.Engine
{
    public class NodeCache : TypeCache<INode>
    {
        protected override bool ShouldCache(Type type)
        {
            if (typeof(GraphReference).IsAssignableFrom(type)) return false;
            return true;
        }

        public IEnumerable<Type> Find<T>() where T : INode => Find(typeof(T));
        public IEnumerable<Type> Find(Type match)
        {
            foreach (var type in All)
            {
                if (match.IsAssignableFrom(type))
                    yield return type;
            }
        }
    }
}