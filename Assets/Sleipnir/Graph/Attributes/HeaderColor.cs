using System;
using UnityEngine;

namespace Sleipnir.Graph.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class HeaderColor : Attribute
    {
        private readonly float _r;
        private readonly float _g;
        private readonly float _b;
        private readonly float _a;

        public HeaderColor(float r, float g, float b, float a = 1f)
        {
            _r = r;
            _g = g;
            _b = b;
            _a = a;
        }

        public Color Color => new Color(_r, _g, _b, _a);
    }
}
