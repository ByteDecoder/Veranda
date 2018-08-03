#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace Sleipnir
{
    public interface INode
    {
        object Value { get; set; }
        IList<IKnob> Knobs { get; }
        Vector2 Position { get; set; }
        float Width { get; }
    }
}
#endif