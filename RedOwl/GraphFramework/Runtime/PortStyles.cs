using System;

namespace RedOwl.GraphFramework
{
    public enum PortStyles
    {
        Multiple = 0,
        Single = 1
    }

    public static class PortStyleExtensions
    {
        public static bool IsSingle(this PortStyles self)
        {
            return self == PortStyles.Single;
        }
    }
}
