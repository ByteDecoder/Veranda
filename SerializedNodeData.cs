using System;
using UnityEngine;

namespace Sleipnir
{
    [Serializable]
    public class SerializedNodeData
    {
#if UNITY_EDITOR
        public const float MinNodeWidth = 30f;
        
        public float LabelWidth;
        public bool IsLabelSliderShown;

        // Dummies are null objects inserted to in-graph node list.
        // They allow nodes to keep their content properly extended 
        // after a node is deleted.
        public int NumberOfPrecedingDummies = 0;

        [SerializeField]
        private Rect _gridRect; // Height is autmatically calculated.

        public Rect GridRect
        {
            get
            {
                return _gridRect.width < MinNodeWidth
                    ? new Rect(
                        _gridRect.position,
                        new Vector2(MinNodeWidth, _gridRect.height
                        ))
                    : _gridRect;
            }
            set { _gridRect = value; }
        }
#endif
    }
}