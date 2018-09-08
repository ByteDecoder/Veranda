using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Sleipnir.Graph.Editor
{
    [DrawerPriority(DrawerPriorityLevel.AttributePriority)]
    public class NodeInputAttributeDrawer : OdinAttributeDrawer<Attributes.Knob>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (Property.ValueEntry.ParentType.IsSubclassOf(typeof(BaseNode)))
            {
                CallNextDrawer(label);
                return;
            }

            var knobs = ((BaseNode)Property.Parent.Parent.ValueEntry.WeakSmartValue).GetKnobs(Property.Name);
            var rect = EditorGUILayout.GetControlRect(false, 0);
            var yPosition = rect.y + EditorGUIUtility.singleLineHeight;
            foreach (var knob in knobs)
                knob.RelativeYPosition = yPosition + Node.HeaderHeight;
            CallNextDrawer(label);
        }
    }
}