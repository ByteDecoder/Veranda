using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEngine;

namespace Sleipnir.Editor
{
    [OdinDrawer]
    public class NodeDrawer : OdinValueDrawer<Node>
    {
        private const float LabelWidth = 0.3f;

        protected override void DrawPropertyLayout(IPropertyValueEntry<Node> entry, GUIContent label)
        {
            entry.SmartValue.BeginDraw();
            var rect = SirenixEditorGUI.BeginBox();
            GUIHelper.PushLabelWidth(entry.SmartValue.Rect.Value.width * LabelWidth);
            CallNextDrawer(entry, null);
            GUIHelper.PopLabelWidth();
            SirenixEditorGUI.EndBox();
            entry.SmartValue.EndDraw(rect.height);
        }
    }
    
    [OdinDrawer]
    [DrawerPriority(DrawerPriorityLevel.WrapperPriority)]
    public class GraphNodeListDrawer<TListType> : OdinValueDrawer<TListType> where TListType : IEnumerable<Node>
    {
        protected override void DrawPropertyLayout(IPropertyValueEntry<TListType> entry, GUIContent label)
        {
            for (var i = 0; i < entry.Property.Children.Count; i++)
                entry.Property.Children[i].Draw();
        }
    }
}