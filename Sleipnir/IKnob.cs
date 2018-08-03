#if UNITY_EDITOR
using UnityEngine;

namespace Sleipnir
{
    public interface IKnob
    {
        string Description { get; }
        Color Color { get; }
        KnobType Type { get; }
    }
}
#endif