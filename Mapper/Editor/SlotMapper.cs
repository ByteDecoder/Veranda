using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using Sleipnir.Editor;
using UnityEditor;
using UnityEngine;

namespace Sleipnir.Mapper.Editor
{
    [DrawerPriority(120, 0, 0)]
    public class SlotMapper: OdinAttributeDrawer<SlotAttribute>
    {
        public static Dictionary<OdinSlot, Slot[]> CurrentNodeSlots;
        public static object NodeValue;

        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (GUIHelper.CurrentWindow.GetType() != typeof(GraphEditor))
            {
                CallNextDrawer(label);
                return;
            }

            if (NestMapper.NestProperty != Property.Parent)
            {
                CallNextDrawer(label);
                return;
            }

            var propertyRect = EditorGUILayout.GetControlRect(false, 0);
            var path = NestMapper.CurrentPath.IsNullOrWhitespace()
                ? Property.Name
                : NestMapper.CurrentPath + "." + Property.Name;
            
            if (CurrentNodeSlots.All(s => s.Key.DeepReflectionPath != path))
            {
                CallNextDrawer(label);
                return;
            }

            var slot = CurrentNodeSlots
                .FirstOrDefault(s => s.Key.DeepReflectionPath == path);
            
            if (!slot.Equals(default(KeyValuePair<OdinSlot, Slot[]>)) && Event.current.type == EventType.Repaint)
                foreach (var s in slot.Value)
                {
                    s.Interactable = true;
                    s.RelativeYPosition = propertyRect.y + 50;
                }

            CallNextDrawer(label);
        }
    }
}