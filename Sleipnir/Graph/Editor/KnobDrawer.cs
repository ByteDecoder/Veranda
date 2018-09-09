using System.Collections;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sleipnir.Graph.Attributes;
using UnityEditor;
using UnityEngine;

namespace Sleipnir.Graph.Editor
{
    [DrawerPriority(DrawerPriorityLevel.AttributePriority)]
    public class KnobDrawer : OdinAttributeDrawer<Attributes.Knob>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (Property.ValueEntry.ParentType.IsSubclassOf(typeof(BaseNode)))
            {
                CallNextDrawer(label);
                return;
            }

            if(Property.ValueEntry.ParentType.IsCastableTo(typeof(IEnumerable)))
            {
                CallNextDrawer(label);
                return;
            }

            var knobs = ((BaseNode)Property.Parent.Parent.ValueEntry.WeakSmartValue)
                .GetKnobs(Property.Name, -1);
            var rect = EditorGUILayout.GetControlRect(false, 0);
            var yPosition = rect.y + EditorGUIUtility.singleLineHeight;
            if (Event.current.type == EventType.Repaint)
                foreach (var knob in knobs)
                    knob.RelativeYPosition = yPosition + Node.HeaderHeight;

            CallNextDrawer(label);
        }
    }

    [DrawerPriority(DrawerPriorityLevel.AttributePriority)]
    public class MultiKnobDrawer : OdinAttributeDrawer<MultiKnob>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var rect = EditorGUILayout.GetControlRect(false, 0);
            var yPosition = rect.y + 42;
            var i = 0;

            if (Event.current.type == EventType.Repaint)
                foreach (var propertyChild in Property.Children)
                    {
                        var knobs = ((BaseNode)Property.Parent.Parent.ValueEntry.WeakSmartValue)
                            .GetKnobs(Property.Name, i);
                        foreach (var knob in knobs)
                            knob.RelativeYPosition = yPosition + Node.HeaderHeight;
                        yPosition += 25;
                        i++;
                    }

            CallNextDrawer(label);
        }
    }
}