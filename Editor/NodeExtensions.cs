using System;
using UnityEngine;

namespace Sleipnir.Editor
{
    public static class NodeExtensions
    {
        public const float HeaderHeight = 32f;
        public const float SliderHeight = 12f;
        public const float ResizeZoneWidth = 5f;
        
        internal static void Move(this Node node, Vector2 delta)
        {
            var rect = node.SerializedNodeData.GridRect;
            node.SerializedNodeData.GridRect = new Rect(rect.position + delta, rect.size);
        }

        internal static void Resize(this Node node, NodeResizeSide side, float delta)
        {
            const float minNodeWidth = SerializedNodeData.MinNodeWidth;
            var rectWidth = node.SerializedNodeData.GridRect.width;
            var oldRatio = node.SerializedNodeData.LabelWidth / rectWidth;
            switch (side)
            {
                case NodeResizeSide.Left:
                    if (rectWidth - delta < minNodeWidth)
                        delta = rectWidth - minNodeWidth;
                    node.Move(new Vector2(delta, 0));
                    delta = -delta;
                    break;

                case NodeResizeSide.Right:
                    if (node.SerializedNodeData.GridRect.width + delta < minNodeWidth)
                        delta = minNodeWidth - rectWidth;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(side), side, null);
            }
            node.SerializedNodeData.GridRect = new Rect(
                node.SerializedNodeData.GridRect.position,
                node.SerializedNodeData.GridRect.size + new Vector2(delta, 0)
                );
            node.SerializedNodeData.LabelWidth = oldRatio * rectWidth;
        }

        internal static void SetNodeContentHeight(this Node node, float contentHeight)
        {
            var rect = node.SerializedNodeData.GridRect;
            node.SerializedNodeData.GridRect = new Rect(
                rect.position, 
                new Vector2(rect.width, contentHeight + HeaderHeight + SliderHeight)
                );
        }

        internal static Rect HeaderRect(this Node node)
        {
            var rect = node.SerializedNodeData.GridRect;
            return node.HasLabelSlider && node.SerializedNodeData.IsLabelSliderShown
                ? new Rect(rect.position.x, rect.position.y, rect.width, HeaderHeight)
                : new Rect(rect.position.x, rect.position.y, rect.width, HeaderHeight + SliderHeight);
        }

        internal static Rect SliderRect(this Node node)
        {
            var rect = node.SerializedNodeData.GridRect;
            return node.HasLabelSlider && node.SerializedNodeData.IsLabelSliderShown
                ? new Rect(rect.position.x, rect.position.y + HeaderHeight, rect.width, SliderHeight)
                : new Rect(rect.position.x, rect.position.y + HeaderHeight + SliderHeight, rect.width, 0);
        }

        internal static Rect TopRect(this Node node)
        {
            var rect = node.SerializedNodeData.GridRect;
            return new Rect(rect.position, new Vector2(rect.width, HeaderHeight + SliderHeight));
        }

        internal static Rect ContentRect(this Node node)
        {
            var rect = node.SerializedNodeData.GridRect;
            return new Rect(rect.x, rect.y + HeaderHeight + SliderHeight,
                rect.width, rect.height - HeaderHeight - SliderHeight);
        }

        internal static Rect RightResizeZone(this Node serializedNodeData)
        {
            return new Rect(serializedNodeData.HeaderRect().xMax - ResizeZoneWidth, serializedNodeData.HeaderRect().y,
                ResizeZoneWidth * 2, HeaderHeight + SliderHeight);
        }

        internal static Rect LeftResizeZone(this Node serializedNodeData)
        {
            return new Rect(serializedNodeData.HeaderRect().position.x - ResizeZoneWidth,
                serializedNodeData.HeaderRect().y, ResizeZoneWidth * 2, HeaderHeight + SliderHeight);
        }
    }
}