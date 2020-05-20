using System;
using RedOwl.Core;

namespace RedOwl.Sleipnir.Engine
{
    public class GraphCache : TypeCache<IGraph>
    {
        protected override bool ShouldCache(Type type)
        {
            if (typeof(GraphReference).IsAssignableFrom(type)) return false;
            return true;
        }
    }
}