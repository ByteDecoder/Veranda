using System;
using System.Collections.Generic;
using Sirenix.Serialization;
using UnityEngine;

namespace Sleipnir
{
    [Serializable]
    public class Node
    {
        private const float MinNodeWidth = 30f;
        private const float HeaderHeight = 32f;
        private const float SliderHeight = 12f;
        private const float ResizePrecision = 5f;

        [OdinSerialize]
        public List<Tuple<string, Action>> ContextMenuFunctions;

        [OdinSerialize]
        public Func<object> ValueGetter;

        [OdinSerialize]
        public Action<object> ValueSetter;

        public Vector2 Position;
        public List<Knob> Knobs;
        public Color HeaderColor = new Color(0.1f, 0.4f, 0.4f);
        public string HeaderTitle = "Node";
        public float NodeWidth = 128f;
        public float LabelWidth = 42f;
        public bool IsLabelSliderShown;
        public bool HasLabelSlider;

        [NonSerialized]
        public float Height = float.PositiveInfinity;

        // Dummies are null objects inserted to in-graph node list.
        // They allow nodes to keep their content properly extended 
        // after a node is deleted.
        public int NumberOfPrecedingDummies = 0;
        
        public void Move(Vector2 delta)
        {
            Position += delta;
        }

        public void Resize(NodeResizeSide side, float delta)
        {
            var oldRatio = LabelWidth / NodeWidth;
            switch (side)
            {
                case NodeResizeSide.Left:
                    if (NodeWidth - delta < MinNodeWidth)
                        delta = NodeWidth - MinNodeWidth;
                    Move(new Vector2(delta, 0));
                    delta = -delta;
                    break;

                case NodeResizeSide.Right:
                    if (NodeWidth + delta < MinNodeWidth)
                        delta = MinNodeWidth - NodeWidth;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(side), side, null);
            }
            NodeWidth += delta;
            LabelWidth = oldRatio * NodeWidth;
        }

        public Rect HeaderRect => HasLabelSlider && IsLabelSliderShown
            ? new Rect(Position, new Vector2(NodeWidth, HeaderHeight))
            : new Rect(Position, new Vector2(NodeWidth, HeaderHeight + SliderHeight));

        public Rect SliderRect => HasLabelSlider && IsLabelSliderShown
            ? new Rect(Position.x, Position.y + HeaderHeight, NodeWidth, SliderHeight)
            : Rect.zero;

        public Rect ContentRect => new Rect(Position.x, Position.y + HeaderHeight + SliderHeight,
            NodeWidth, Height);

        public Rect RightResizeZone => new Rect(HeaderRect.xMax - ResizePrecision, HeaderRect.y,
            ResizePrecision * 2, HeaderHeight + SliderHeight);

        public Rect LeftResizeZone => new Rect(HeaderRect.position.x - ResizePrecision,
            HeaderRect.y, ResizePrecision * 2, HeaderHeight + SliderHeight);
    }
}