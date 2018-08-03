using System;
using System.Collections.Generic;
using Sirenix.Utilities.Editor;
using UnityEngine;

namespace Sleipnir.Editor
{
    public struct Knob
    {
        public const float LabelOffset = 3f;
        public static readonly Vector2 KnobSize = new Vector2(20, 20);
        public static readonly Vector2 KnobPadding = new Vector2(5, 5);
        public static readonly Dictionary<KnobType, GUIStyle> LabelGuiStyles = new Dictionary<KnobType, GUIStyle>
        {
            { KnobType.Input,
                new GUIStyle {
                    normal = { textColor = Color.white }}},

            { KnobType.Output,
                new GUIStyle {
                    normal = { textColor = Color.white }}}
        };
        
        [NonSerialized] public readonly IKnob Content;
        private readonly Boxed<Rect> _rect;
        private readonly GraphEditor _editor;
        public Rect Rect => _rect.Value;
        public Color Color => Content.Color;

        public Knob(GraphEditor editor, IKnob knob)
        {
            Content = knob;
            _editor = editor;
            _rect = new Boxed<Rect>();
        }

        public void Draw(Rect rect)
        {
            _rect.Value = rect;

            GUIHelper.PushColor(Color);
            if (GUI.Button(_editor.GridToGuiDrawRect(Rect), ""))
                _editor.OnKnobClick(this);
            GUIHelper.PopColor();

            var labetStyle = LabelGuiStyles[Content.Type];
            var labelContent = new GUIContent(Content.Description);
            var labelSize = labetStyle.CalcSize(labelContent);

            var labelPosition = Content.Type == KnobType.Input 
                ? Rect.position + new Vector2(-LabelOffset - labelSize.x, KnobSize.y / 2 + LabelOffset)
                : Rect.position + new Vector2(LabelOffset + KnobSize.x, 
                                                - KnobSize.y / 2 + labelSize.y / 2 - LabelOffset);

            var labelRect = new Rect(labelPosition, labelSize);

            GUI.Label(_editor.GridToGuiDrawRect(labelRect), labelContent, labetStyle);
        }
    }
}