using Sirenix.OdinInspector.Editor;
using Sleipnir.Graph.Attributes;
using UnityEditor;
using UnityEngine;

namespace Sleipnir.Graph.Editor
{
    [DrawerPriority(DrawerPriorityLevel.AttributePriority)]
    public class CollectionKnobDrawer : OdinAttributeDrawer<CollectionKnob>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var rect = EditorGUILayout.GetControlRect(false, 0);
            var yPosition = rect.y + 42;
            var i = 0;

            if (Event.current.type == EventType.Repaint)
                foreach (var unused in Property.Children)
                {
                    var knobs = ((GraphNode)Property.Parent.Parent.ValueEntry.WeakSmartValue)
                        .GetKnobs(Property.Name, i);

                    if (knobs == null)
                        break;

                    foreach (var knob in knobs)
                        knob.RelativeYPosition = yPosition + Node.HeaderHeight;
                    yPosition += 25;
                    i++;
                }

            CallNextDrawer(label);
        }
    }
}