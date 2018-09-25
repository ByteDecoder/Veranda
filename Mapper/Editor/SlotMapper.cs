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
    [DrawerPriority(121, 0, 0)]
    public class SlotMapper: OdinAttributeDrawer<SlotAttribute>
    {
        public static Dictionary<OdinSlot, Slot> CurrentNodeSlots;
        public static object NodeValue;

        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (GUIHelper.CurrentWindow.GetType() != typeof(GraphEditor))
            {
                CallNextDrawer(label);
                return;
            }
            var propertyRect = EditorGUILayout.GetControlRect(false, 0);
            var path = NestMapper.CurrentPath.IsNullOrWhitespace()
                ? Property.Name
                : NestMapper.CurrentPath + "." + Property.Name;

            var slot = CurrentNodeSlots
                .First(s => s.Key.DeepReflectionPath == path)
                .Value;

            if (Event.current.type == EventType.Repaint)
                slot.RelativeYPosition = propertyRect.y + 50;
            CallNextDrawer(label);
        }
    }
}