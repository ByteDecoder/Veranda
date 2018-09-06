using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEngine;

namespace Sleipnir.Editor
{
    public class EditorNodeDrawer : OdinValueDrawer<EditorNode>
    {
        private const float SliderHorizontalSpacing = 8f;
        private const float SliderVerticalOffset = 4f;
        private const float SliderMaxOffset = 12f;

        protected override void DrawPropertyLayout(GUIContent label)
        {
            var value = ValueEntry.SmartValue;
            var content = value.Content;

            var headerRect = value.Editor.GridToGuiDrawRect(content.HeaderRect);
            var sliderRect = value.Editor.GridToGuiDrawRect(content.SliderRect);

            var topBox = new Rect(headerRect.x, headerRect.y,
                headerRect.width, headerRect.height + sliderRect.height);

            // Draw top box
            GUIHelper.PushColor(content.HeaderColor);
            GUI.Box(topBox, "");
            if (content.HasLabelSlider && content.IsLabelSliderShown)
            {
                // Offset provide better visuals
                var offsettedSliderRect = new Rect(
                        sliderRect.x + SliderHorizontalSpacing,
                        sliderRect.y - SliderVerticalOffset,
                        sliderRect.width - 2 * SliderHorizontalSpacing,
                        sliderRect.height);

                content.LabelWidth = GUI.HorizontalSlider(offsettedSliderRect,
                    content.LabelWidth, 0, content.NodeWidth - SliderMaxOffset);
            }
            GUIHelper.PopColor();

            var titleGUIStyle = new GUIStyle(GUI.skin.GetStyle("Label"))
                    {
                        alignment = TextAnchor.MiddleCenter,
                        normal = { textColor = content.TitleColor}
                    };

            GUI.Label(headerRect, content.HeaderTitle, titleGUIStyle);

            // Draw content
            GUILayout.BeginArea(value.Editor.GridToGuiDrawRect(content.ContentRect));
            var contentBoxRect = SirenixEditorGUI.BeginBox();
            // When label width equals 0 it is drawn with default inspector width.
            // Maximum value makes it nicer.
            var labelWidth = Mathf.Clamp(ValueEntry.SmartValue.Content.LabelWidth, Mathf.Epsilon,
                ValueEntry.SmartValue.Content.NodeWidth - SliderMaxOffset);
            GUIHelper.PushLabelWidth(labelWidth);
            CallNextDrawer(label);
            GUIHelper.PopLabelWidth();
            SirenixEditorGUI.EndBox();

            if (Event.current.type == EventType.Repaint)
                content.Height = contentBoxRect.height;

            GUILayout.EndArea();

            foreach (var knob in content.Knobs)
                value.Editor.Draw(knob, value);
        }
    }

    [DrawerPriority(DrawerPriorityLevel.WrapperPriority)]
    public class GraphNodeListDrawer<TListType>
        : OdinValueDrawer<TListType> where TListType : IEnumerable<EditorNode>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            foreach (var propertyChild in ValueEntry.Property.Children)
                if (((EditorNode)propertyChild.ValueEntry.WeakSmartValue).Content != null)
                    propertyChild.Draw();
        }
    }
}