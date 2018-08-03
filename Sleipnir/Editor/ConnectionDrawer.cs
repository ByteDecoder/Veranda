using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEngine;

namespace Sleipnir.Editor
{
    [OdinDrawer]
    public class ConnectionDrawer : OdinValueDrawer<Connection>
    {
        private const float LabelWidth = 0.3f;

        protected override void DrawPropertyLayout(IPropertyValueEntry<Connection> entry, GUIContent label)
        {
            entry.SmartValue.BeginDraw();
            var height = 0f;
            if (entry.SmartValue.Content.DoesHaveValue && entry.SmartValue.Content.IsExpanded)
            {
                var rect = SirenixEditorGUI.BeginBox();
                GUIHelper.PushLabelWidth(rect.width * LabelWidth);
                CallNextDrawer(entry, null);
                GUIHelper.PopLabelWidth();
                SirenixEditorGUI.EndBox();
                height = rect.height;
            }
            entry.SmartValue.EndDraw(height);
        }
    }
    
    [OdinDrawer]
    [DrawerPriority(DrawerPriorityLevel.WrapperPriority)]
    public class ConnectionListDrawer<TListType> : OdinValueDrawer<TListType>
        where TListType : IEnumerable<Connection>
    {
        protected override void DrawPropertyLayout(IPropertyValueEntry<TListType> entry, 
            GUIContent label)
        {
            for (var i = 0; i < entry.Property.Children.Count; i++)
                entry.Property.Children[i].Draw();
        }
    }
}