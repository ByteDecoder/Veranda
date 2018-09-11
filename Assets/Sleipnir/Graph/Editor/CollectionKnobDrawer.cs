using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sleipnir.Editor;
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
            if (!Property.Parent.Parent.ParentType.InheritsFrom(typeof(EditorNode)))
            {
                CallNextDrawer(label);
                return;
            }

            var rect = EditorGUILayout.GetControlRect(false, 0);
            if (Event.current.type == EventType.Repaint)
            {
                var yPosition = rect.y + 42;
                var i = 0;
                foreach (var unused in Property.Children)
                {
                    var knobs = ((GraphNode) Property.Parent.Parent.ValueEntry.WeakSmartValue)
                        .GetKnobs(Property.Name, i);

                    if (knobs == null)
                        break;

                    foreach (var knob in knobs)
                        knob.RelativeYPosition = yPosition + Node.HeaderHeight;
                    yPosition += 25;
                    i++;
                }
            }
            CallNextDrawer(label);
        }
    }
}