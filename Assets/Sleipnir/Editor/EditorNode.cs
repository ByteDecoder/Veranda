using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Sleipnir.Editor
{
    public class EditorNode
    {
        public const float HeaderHeight = 32f;
        public const float SliderHeight = 12f;
        public const float TopBoxHeight = SliderHeight + HeaderHeight;

        private const float MinNodeWidth = 80f;
        private const float ResizePrecision = 5f;
        
        [HideInInspector]
        public readonly ValueWrappedNode Content;
        [HideInInspector]
        public Color HeaderColor = Color.cyan;
        [HideInInspector]
        public Color TitleColor = Color.white;
        [HideInInspector]
        public string Title = "Node";
        [HideInInspector]
        public bool HasLabelSlider = true;
        
        [ShowInInspector, HideLabel, HideReferenceObjectPicker]
        public object Value
        {
            get
            {
                if (Content.Getter != null)
                    return Content.Getter();

                Debug.LogError("Node value getter is not set.");
                return null;
            }
            set
            {
                if (Content.Setter != null)
                {
                    Content.Setter(value);
                    return;
                }

                Debug.LogError("Node value setter is not set.");
            }
        }

        public EditorNode(ValueWrappedNode content)
        {
            Content = content;
            var attributes = content.Getter().GetType().GetCustomAttributes(true).Cast<Attribute>().ToArray();

            if (Content.Node.NodeRect.width == 0)
                ProcessOptionalAttribute<NodeWidthAttribute>(attributes,
                    attribute =>
                    {
                        var oldRect = Content.Node.NodeRect;
                        Content.Node.NodeRect = new Rect(oldRect.x, oldRect.y, attribute.DefaultWidth, oldRect.height);
                    });

            ProcessOptionalAttribute<HeaderTitleAttribute>(attributes,
                attribute => Title = attribute.Text);

        }

        private static void ProcessOptionalAttribute<T>
            (IEnumerable<Attribute> attributes, Action<T> onFound) where T : Attribute
        {
            var attribute = (T)attributes.FirstOrDefault(a => a.GetType() == typeof(T));
            if (attribute != null)
                onFound(attribute);
        }

        private Vector2 Position => Content.Node.NodeRect.position;

        public void Move(Vector2 delta)
        {
            Content.Node.NodeRect.position += delta;
        }

        private float NodeWidth => Mathf.Max(Content.Node.NodeRect.width, MinNodeWidth);

        public float LabelWidth => Mathf.Max(float.Epsilon, Content.Node.LabelWidth);

        public void Resize(NodeResizeSide side, float delta)
        {
            var node = Content.Node;

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
            node.NodeRect.width += delta;
            node.LabelWidth = oldRatio * NodeWidth;
        }
        
        public Rect HeaderRect => HasLabelSlider && Content.Node.IsLabelSliderShown
            ? new Rect(Position, new Vector2(NodeWidth, HeaderHeight))
            : new Rect(Position, new Vector2(NodeWidth, TopBoxHeight));

        public Rect SliderRect => HasLabelSlider && Content.Node.IsLabelSliderShown
            ? new Rect(Position.x, Position.y + HeaderHeight, NodeWidth, SliderHeight)
            : Rect.zero;

        public Rect TopRect => new Rect(HeaderRect.position, new Vector2(NodeWidth, TopBoxHeight));

        public Rect ContentRect => new Rect(Position.x,
            Position.y + TopBoxHeight,
            NodeWidth,
            Content.Node.NodeRect.height - TopBoxHeight);

        public Rect RightResizeZone => new Rect(HeaderRect.xMax - ResizePrecision,
            HeaderRect.y,
            ResizePrecision * 2,
            TopBoxHeight);

        public Rect LeftResizeZone => new Rect(HeaderRect.position.x - ResizePrecision,
            HeaderRect.y,
            ResizePrecision * 2,
            TopBoxHeight);
    }
}