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
    public class NestMapper : OdinAttributeDrawer<NestedAttribute>
    {
        public static Stack<Nest> Nests = new Stack<Nest>();
        public static string CurrentPath;
        public static InspectorProperty NestProperty;

        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (GUIHelper.CurrentWindow.GetType() != typeof(GraphEditor))
            {
                CallNextDrawer(label);
                return;
            }
            
            var name = Property.Name.StartsWith("$") // List elements
                ? "[" + Property.Name.TrimStart('$') + "]"
                : Property.Name;

            var oldPath = CurrentPath;
            var oldProperty = NestProperty;

            NestProperty = Property;
            Nests.Push(Nests.Peek().Nests.First(n => n.FieldName == name));
            CurrentPath = oldPath.IsNullOrWhitespace()
                ? name
                : oldPath + "." + name;
            
            var propertyRect = EditorGUILayout.GetControlRect(false, 0);
            if (Event.current.type == EventType.Repaint)
                Nests.Peek().YPosition = propertyRect.y + 50;
            
            var path = CurrentPath;
            var slots = SlotMapper.CurrentNodeSlots
                .Where(s => s.Key.DeepReflectionPath.StartsWith(path));

            foreach (var slot in slots.SelectMany(s => s.Value))
            {
                if (Event.current.type == EventType.Repaint)
                    slot.RelativeYPosition = Nests.Peek().YPosition;
            }

            CallNextDrawer(label);
            Nests.Pop();
            CurrentPath = oldPath;
            NestProperty = oldProperty;
        }
    }
}