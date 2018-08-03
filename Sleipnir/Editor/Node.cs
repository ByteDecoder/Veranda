using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Sleipnir.Editor
{
    public struct Node
    {
        private static readonly Vector2 ButtonSize = new Vector2(20, 20);
        private const float ButtonPadding = 10f;

        private readonly GraphEditor _editor;
        [HideInInspector] public readonly INode Content;
        [HideInInspector] public readonly Knob[] Knobs;
        public Boxed<Rect> Rect { get; }

        [ShowInInspector, OnValueChanged("ValueUpdate"), HideReferenceObjectPicker, HideLabel]
        private object _value;
        // ReSharper disable once UnusedMember.Local
        private void ValueUpdate() => Content.Value = _value;
        
        public Node(GraphEditor editor, INode node)
        {
            _editor = editor;
            Content = node;
            Rect = new Boxed<Rect> { Value = new Rect(node.Position.x, node.Position.y, node.Width, 0)};
            Knobs = node.Knobs.Select(o => new Knob(editor, o)).ToArray();
            _value = node.Value;
        }

        public void Move(Vector2 delta)
        {
            Rect.Value = new Rect(Rect.Value.position + delta, Rect.Value.size);
            Content.Position = Rect.Value.position;
        }

        public void BeginDraw()
        {
            GUILayout.BeginArea(_editor.GridToGuiDrawRect(Rect.Value));
        }

        //y axis is inverted
        public void EndDraw(float height)
        {
            if(Event.current.type == EventType.Repaint)
                Rect.Value = new Rect(Rect.Value.x, Rect.Value.y, Rect.Value.width, height);
            GUILayout.EndArea();

            if (GUI.Button(_editor.GridToGuiDrawRect(new Rect(Rect.Value.position + 
                new Vector2(Rect.Value.width/2 - ButtonSize.x/2, - ButtonSize.y - ButtonPadding), ButtonSize)),
                ""))
                _editor.SelectNode(this);
            
            var inputs = Knobs.Where(o => o.Content.Type == KnobType.Input).ToArray();
            DrawKnobs(
                offsetX: Rect.Value.position.x - Knob.KnobSize.x - Knob.KnobPadding.x,
                offsetY: GetOffsetY(inputs.Length),
                knobs: inputs
            );
            var outputs = Knobs.Where(o => o.Content.Type == KnobType.Output).ToArray();
            DrawKnobs(
                offsetX: Rect.Value.xMax + Knob.KnobPadding.x,
                offsetY: GetOffsetY(outputs.Length),
                knobs: outputs
            );
        }

        private static void DrawKnobs(float offsetX, float offsetY, IEnumerable<Knob> knobs)
        {
            var y = offsetY;
            foreach (var knob in knobs)
            {
                knob.Draw(new Rect(offsetX, y, Knob.KnobSize.x, Knob.KnobSize.y));
                var knobHeight = Knob.KnobSize.y + Knob.KnobPadding.y;
                y += knobHeight;
            }
        }

        private float GetOffsetY(int numberOfKnobs)
        {
            return Rect.Value.center.y -
                   (Knob.KnobSize.y * numberOfKnobs + (numberOfKnobs - 1) * Knob.KnobPadding.y) / 2;
        }
    }
}