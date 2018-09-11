using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sleipnir.Editor;
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

            if (property.ParentType.InheritsFrom(typeof(Object)))
            {
                //Debug.Log(Property.ParentType);
                //Debug.Log(Property);
            }
            else
            {

                while (property.ValueEntry.WeakSmartValue == null
                       || !property.ValueEntry.WeakSmartValue.GetType().InheritsFrom(typeof(GraphNode)))
                    property = property.Parent;

                if (!property.ParentType.InheritsFrom(typeof(EditorNode)))
                {
                    CallNextDrawer(label);
                    return;
                }

                var rect = EditorGUILayout.GetControlRect(false, 0);

                if (Event.current.type != EventType.Repaint)
                {
                    CallNextDrawer(label);
                    return;
                }

                var knobs = ((GraphNode) property.ValueEntry.WeakSmartValue)
                    .GetKnobs(Property.Name, -1);

                if (knobs == null)
                {
                    CallNextDrawer(label);
                    return;
                }

                var yPosition = rect.y + EditorGUIUtility.singleLineHeight;
                foreach (var knob in knobs)
                    knob.RelativeYPosition = yPosition + Node.HeaderHeight;
            }

            CallNextDrawer(label);
        }
    }
}