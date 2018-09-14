using System;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Sleipnir.Editor
{
    [DrawerPriority(DrawerPriorityLevel.AttributePriority)]
    public class SlotAttributeDrawer : OdinAttributeDrawer<SlotAttribute>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (GUIHelper.CurrentWindow.GetType() != typeof(GraphEditor))
            {
                CallNextDrawer(label);
                return;
            }

            var editor = (GraphEditor) GUIHelper.CurrentWindow;
            var propertyRect = EditorGUILayout.GetControlRect(false, 0);
            
            if (Attribute.Direction.IsInput())
            {
                var editorNode = editor.CurrentlyDrawedNode;
                var relativeRect = new Rect(new Rect(new Vector2(0, propertyRect.y), new Vector2(12, 12)));
                var slot = new Slot(editorNode.Content, GetSlotPath());
                var rect = new Rect(editorNode.ContentRect.position + new Vector2(-20, propertyRect.y), relativeRect.size);

                GUIHelper.PushColor(Color.cyan);
                if (GUI.Button(relativeRect, ""))
                    // There is no such thing as SlotDirection.InOut form editor's perspective
                    editor.OnSlotClick(slot, SlotDirection.Input);
                
                GUIHelper.PopColor();
                editor.Slots.Add(new Tuple<Slot, Rect>(slot, rect));
            }

            if (Attribute.Direction.IsOutput())
            {
                var editorNode = editor.CurrentlyDrawedNode;
                var relativeRect = new Rect(new Vector2(editorNode.ContentRect.width + 28, propertyRect.y), new Vector2(12, 12));
                var slot = new Slot(editorNode.Content, GetSlotPath());
                var rect = new Rect(new Vector2(editorNode.ContentRect.xMax + 8, editorNode.ContentRect.position.y + propertyRect.y), relativeRect.size);

                GUIHelper.PushColor(Color.cyan);
                if (GUI.Button(relativeRect, ""))
                    // There is no such thing as SlotDirection.InOut form editor's perspective
                    editor.OnSlotClick(slot, SlotDirection.Output);
                GUIHelper.PopColor();
                editor.Slots.Add(new Tuple<Slot, Rect>(slot, rect));
            }

            CallNextDrawer(label);
        }

        private string GetSlotPath()
        {
            // TODO It needs to be relative to the node!
            return Property.UnityPropertyPath;
        }
    }
}