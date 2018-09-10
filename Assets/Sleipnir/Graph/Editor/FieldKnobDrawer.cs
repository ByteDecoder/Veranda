using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sleipnir.Graph.Attributes;
using UnityEditor;
using UnityEngine;

namespace Sleipnir.Graph.Editor
{
    [DrawerPriority(DrawerPriorityLevel.AttributePriority)]
    public class KnobDrawer : OdinAttributeDrawer<FieldKnob>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var property = Property;
            while (property.ValueEntry.WeakSmartValue == null
                || !property.ValueEntry.WeakSmartValue.GetType().InheritsFrom(typeof(GraphNode)))
                property = property.Parent;

            var knobs = ((GraphNode)property.ValueEntry.WeakSmartValue)
                .GetKnobs(Property.Name, -1);

            if (knobs == null)
            {
                CallNextDrawer(label);
                return;
            }

            var rect = EditorGUILayout.GetControlRect(false, 0);
            var yPosition = rect.y + EditorGUIUtility.singleLineHeight;
            if (Event.current.type == EventType.Repaint)
                foreach (var knob in knobs)
                    knob.RelativeYPosition = yPosition + Node.HeaderHeight;

            CallNextDrawer(label);
        }
    }
}