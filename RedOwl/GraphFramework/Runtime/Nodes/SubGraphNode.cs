using UnityEngine;
using RedOwl.GraphFramework;

namespace RedOwl.GraphFramework
{
    public enum SubGraphStyles
    {
        Asset = 0,
        Embed,
    }

    public class SubGraphNode : Node
    {
        public SubGraphStyles style;

        public Graph asset;
    }
}
