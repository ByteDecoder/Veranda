using System;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Sleipnir.Editor
{
    [DrawerPriority(DrawerPriorityLevel.AttributePriority)]
    public class KnobAttributeDrawer : OdinAttributeDrawer<KnobAttribute>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (GUIHelper.CurrentWindow.GetType() != typeof(GraphEditor))
                return;
            
            var editor = (GraphEditor) GUIHelper.CurrentWindow;
            var propertyRect = EditorGUILayout.GetControlRect(false, 0);
            
            if (Attribute.Direction == KnobDirection.Both || Attribute.Direction == KnobDirection.Input)
            {
                var editorNode = editor.CurrentlyDrawedNode;
                var relativeRect = new Rect(new Rect(new Vector2(0, propertyRect.y), new Vector2(12, 12)));
                var knob = new Knob(editorNode.Content, GetKnobPath());
                var rect = new Rect(editorNode.ContentRect.position + relativeRect.position, relativeRect.size);

                GUIHelper.PushColor(Color.cyan);
                if (GUI.Button(relativeRect, ""))
                    editor.OnKnobClick(knob, KnobType.Input);
                GUIHelper.PopColor();

                editor.Knobs.Add(new Tuple<Knob, Rect>(knob, rect));
            }

            if (Attribute.Direction == KnobDirection.Both || Attribute.Direction == KnobDirection.Output)
            {
                var editorNode = editor.CurrentlyDrawedNode;
                var relativeRect = new Rect(new Vector2(editorNode.ContentRect.width - 12, propertyRect.y), new Vector2(12, 12));
                var knob = new Knob(editorNode.Content, GetKnobPath());
                var rect = new Rect(editorNode.ContentRect.position + relativeRect.position, relativeRect.size);

                GUIHelper.PushColor(Color.cyan);
                if (GUI.Button(relativeRect, ""))
                    editor.OnKnobClick(knob, KnobType.Output);
                GUIHelper.PopColor();
                editor.Knobs.Add(new Tuple<Knob, Rect>(knob, rect));
            }

            CallNextDrawer(label);
        }

        private string GetKnobPath()
        {
            // TODO It needs to be relative to the node!
            return Property.UnityPropertyPath;
        }
    }
}