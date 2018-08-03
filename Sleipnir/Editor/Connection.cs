using System;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Sleipnir.Editor
{
    public struct Connection
    {
        private const float LineWidth = 5f;
        private static readonly Vector2 ButtonSize = new Vector2(20, 20);
        private const float Padding = 5f;

        [NonSerialized] public readonly IConnection Content;
        [NonSerialized] public readonly Knob OutputKnob;
        [NonSerialized] public readonly Knob InputKnob;
        
        private readonly GraphEditor _editor;
        [NonSerialized] public Boxed<float> Height;

        [ShowInInspector, OnValueChanged("UpdateValue"), HideReferenceObjectPicker, HideLabel]
        private object _value;
        // ReSharper disable once UnusedMember.Local
        private void UpdateValue() => Content.Value = _value;

        public Connection(IConnection content, GraphEditor editor, Knob outputKnob, Knob inputKnob)
        {
            Content = content;
            OutputKnob = outputKnob;
            InputKnob = inputKnob;
            
            _value = content.Value;
            Height = new Boxed<float> { Value = 0 };

            _editor = editor;
        }
        
        private Vector2 Foothold()
        {
            return (OutputKnob.Rect.center + InputKnob.Rect.center)/2;
        }
        
        private Color Color() => OutputKnob.Color == InputKnob.Color
            ? OutputKnob.Color
            : UnityEngine.Color.Lerp(InputKnob.Color, OutputKnob.Color, 0.5f);

        private Rect Rect => new Rect(
            Foothold() - new Vector2(Content.ValueWindowWidth/2, - ButtonSize.x / 2 - Padding),
            new Vector2(Content.ValueWindowWidth, Height.Value));
        
        private Rect DeleteButtonRect() => new Rect(Foothold() - ButtonSize / 2, ButtonSize);

        private Rect ValueButtonRect() => new Rect(
            Foothold() + new Vector2(0.5f * ButtonSize.x + Padding, -ButtonSize.y / 2), ButtonSize);

        public void BeginDraw()
        {
            GUIHelper.PushColor(Color());
            if (GUI.Button(_editor.GridToGuiDrawRect(DeleteButtonRect()), "X"))
                _editor.RemoveConnection(this);

            if (Content.DoesHaveValue && GUI.Button(_editor.GridToGuiDrawRect(ValueButtonRect()), "i"))
            {
                Content.IsExpanded = !Content.IsExpanded;
                _editor.MoveConnectionToFront(this);
            }

            GUIHelper.PopColor();
            GUILayout.BeginArea(_editor.GridToGuiDrawRect(Rect));
        }

        public void EndDraw(float height)
        {
            GUILayout.EndArea();
            if (Event.current.type == EventType.Repaint)
                Height.Value = height;
        }

        public static void DrawLine(Vector2 startGridPosition, Vector2 endGridPosition, Color color)
        {
            var tangentMultiplier = (endGridPosition - startGridPosition).magnitude;

            Handles.DrawBezier(
                startGridPosition,
                endGridPosition,
                startGridPosition + Vector2.right * tangentMultiplier,
                endGridPosition + Vector2.left * tangentMultiplier,
                color,
                null,
                LineWidth
            );
        }

        public void DrawLine()
        {
            var start = _editor.GridToGuiPositionNoClip(OutputKnob.Rect.center);
            var end = _editor.GridToGuiPositionNoClip(InputKnob.Rect.center);
            DrawLine(start, end, Color());
        }
    }
}