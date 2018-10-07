using System;
using UnityEngine;

namespace Sleipnir
{
    [AttributeUsage(AttributeTargets.Method)]
    public class EvaluateAttribute : Attribute
    {
        public int Order = 0;

        public EvaluateAttribute(int order = 0)
        {
            Order = order;
        }
    }
}