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
                return;
            
            var editor = (GraphEditor) GUIHelper.CurrentWindow;
            var propertyRect = EditorGUILayout.GetControlRect(false, 0);
            
            if (Attribute.Direction.IsInput())
            {
                var editorNode = editor.CurrentlyDrawedNode;
                var relativeRect = new Rect(new Rect(new Vector2(0, propertyRect.y), new Vector2(12, 12)));
                var slot = new Slot(editorNode.Content, GetSlotPath());
                var rect = new Rect(editorNode.ContentRect.position + relativeRect.position, relativeRect.size);

                GUIHelper.PushColor(Color.cyan);
                if (GUI.Button(relativeRect, ""))
                    editor.OnSlotClick(slot, Attribute.Direction);
                GUIHelper.PopColor();

                editor.Slots.Add(new Tuple<Slot, Rect>(slot, rect));
            }

            if (Attribute.Direction.IsOutput())
            {
                var editorNode = editor.CurrentlyDrawedNode;
                var relativeRect = new Rect(new Vector2(editorNode.ContentRect.width - 12, propertyRect.y), new Vector2(12, 12));
                var slot = new Slot(editorNode.Content, GetSlotPath());
                var rect = new Rect(editorNode.ContentRect.position + relativeRect.position, relativeRect.size);

                GUIHelper.PushColor(Color.cyan);
                if (GUI.Button(relativeRect, ""))
                    editor.OnSlotClick(slot, Attribute.Direction);
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