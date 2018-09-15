using System;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEngine;

namespace Sleipnir.Editor
{
    [DrawerPriority(DrawerPriorityLevel.WrapperPriority)]
    public class SlotAttributeDrawer<T> : OdinAttributeDrawer<T> where T : SlotAttribute
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

                GUIHelper.PushColor(new Color(1f, 0.35f, 0.0f));
                if (GUI.Button(relativeRect, ""))
                    // There is no such thing as SlotDirection.InOut form editor's perspective
                    editor.OnSlotClick(slot, SlotDirection.Input);
                
                GUIHelper.PopColor();
                editor.Slots.Add(new Tuple<Slot, Rect, SlotDirection>(slot, rect, SlotDirection.Input));
            }

            if (Attribute.Direction.IsOutput())
            {
                var editorNode = editor.CurrentlyDrawedNode;
                var relativeRect = new Rect(new Vector2(editorNode.ContentRect.width + 28, propertyRect.y), new Vector2(12, 12));
                var slot = new Slot(editorNode.Content, GetSlotPath());
                var rect = new Rect(new Vector2(editorNode.ContentRect.xMax + 8, editorNode.ContentRect.position.y + propertyRect.y), 
                    relativeRect.size);

                GUIHelper.PushColor(new Color(1f, 0.35f, 0.0f));
                if (GUI.Button(relativeRect, ""))
                    // There is no such thing as SlotDirection.InOut form editor's perspective
                    editor.OnSlotClick(slot, SlotDirection.Output);
                GUIHelper.PopColor();
                editor.Slots.Add(new Tuple<Slot, Rect, SlotDirection>(slot, rect, SlotDirection.Output));
            }

            CallNextDrawer(label);
        }

        private string GetSlotPath()
        {
            if (Property.Parent == null) // Unity object
                return Property.PrefabModificationPath;

            var editor = (GraphEditor)GUIHelper.CurrentWindow;
            var value = editor.CurrentlyDrawedNode.Value;

            var editorNodeProperty = Property;

            // null in case of group
            while (editorNodeProperty.ValueEntry == null || 
                !ReferenceEquals(editorNodeProperty.ValueEntry.WeakSmartValue, value))
                editorNodeProperty = editorNodeProperty.Parent;
            
            // +1 is for the dot
            return Property.UnityPropertyPath.Remove(0, editorNodeProperty.UnityPropertyPath.Length +1);
        }
    }
}