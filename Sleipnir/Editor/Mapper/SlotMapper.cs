using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Sleipnir.Editor
{
    [DrawerPriority(121, 0, 0)]
    public class SlotMapper: OdinAttributeDrawer<SlotAttribute>
    {
        public static Dictionary<OdinSlot, Slot[]> CurrentNodeSlots;
        public static object NodeValue;

        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (GUIHelper.CurrentWindow.GetType() != typeof(GraphEditor)
                || !NestMapper.NestProperty.IsParentOf(Property)) // Nested not marked with [Nested]
            {
                CallNextDrawer(label);
                return;
            }
            
            var path = NestMapper.CurrentPath.IsNullOrWhitespace()
                ? Property.Name
                : NestMapper.CurrentPath + "." + Property.Name;

            var labelOnly = false;
            var propertyRect = EditorGUILayout.GetControlRect(false, 0);
            if (CurrentNodeSlots.Any(s => s.Key.DeepReflectionPath == path))
            {
                var slot = CurrentNodeSlots
                    .First(s => s.Key.DeepReflectionPath == path);
                foreach (var s in slot.Value)
                {
                    if (!labelOnly)
                        labelOnly = ((GraphEditor)GUIHelper.CurrentWindow)
                            .Connections
                            .Any(c => c.InputSlot == s);
                    if (Event.current.type == EventType.Repaint)
                        s.RelativeYPosition = propertyRect.y + 50;
                }
            }

            if (labelOnly)
                EditorGUILayout.LabelField(Property.NiceName);
            else
                CallNextDrawer(label);
        }
    }
}