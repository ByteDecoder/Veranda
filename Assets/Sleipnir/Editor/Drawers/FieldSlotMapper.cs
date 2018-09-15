using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Sleipnir.Editor
{
    /* This class is responsible for mapping fields with slot attributes into SlotEditorData.
     * Attribute drawer enables easy acces to property rect. It doesn't do any actual drawing
     * in order to escape node's content GUIArea. Slots are drawn inside EditorNodeDrawer. */
    [DrawerPriority(DrawerPriorityLevel.WrapperPriority)]
    public class FieldSlotMapper<T> : OdinAttributeDrawer<T> where T : SlotAttribute
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
                var rect = new Rect(editorNode.ContentRect.position + new Vector2(-20, propertyRect.y),
                    new Vector2(12, 12));
                var slot = new Slot(editorNode.Content, GetSlotPath());
                
                var editorSlot = new EditorSlot(slot, rect, SlotDirection.Input);
                editor.Slots.Add(editorSlot);
                editorNode.Slots.Add(editorSlot);
            }

            if (Attribute.Direction.IsOutput())
            {
                var editorNode = editor.CurrentlyDrawedNode;
                var slot = new Slot(editorNode.Content, GetSlotPath());
                var rect = new Rect(editorNode.ContentRect.position + new Vector2(8 + editorNode.ContentRect.width, propertyRect.y),
                    new Vector2(12, 12));
                var editorSlot = new EditorSlot(slot, rect, SlotDirection.Output);
                editor.Slots.Add(editorSlot);
                editorNode.Slots.Add(editorSlot);
            }

            CallNextDrawer(label);
        }

        private string GetSlotPath()
        {
            /* This doesn't work for nested inline editors right now - if your node have a field 
             * that is displayed through inline editor and this field contains slots you won't get 
             * a proper path through them. */

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
            return Property.PrefabModificationPath.Remove(0, editorNodeProperty.PrefabModificationPath.Length +1);
        }
    }
}

