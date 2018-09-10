using System;
using UnityEngine;

namespace Sleipnir
{
    [Serializable]
    public class Knob
    {
#if UNITY_EDITOR
        public float RelativeYPosition;
        public string Description;
        public Color Color = new Color(0.1f, 0.4f, 0.4f);
        public KnobType Type;

        [NonSerialized]
        public Rect Rect;
        
        public Knob(float relativeYPosition, KnobType type)
        {
            RelativeYPosition = relativeYPosition;
            Type = type;
        }

        public Knob(float relativeYPosition, KnobType type, Color color, string description) 
            : this(relativeYPosition, type)
        {
            Color = color;
            Type = type;
        }
#endif
    }
}