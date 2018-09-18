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

        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (GUIHelper.CurrentWindow.GetType() != typeof(GraphEditor))
            {
                CallNextDrawer(label);
                return;
            }

            var value = ValueEntry.SmartValue;
            var content = value.Content.Node;
            var editor = (GraphEditor)GUIHelper.CurrentWindow;
            editor.CurrentlyDrawedNode = value;
            value.Slots = new List<EditorSlot>();

            var headerRect = editor.GridToGuiDrawRect(value.HeaderRect);
            var sliderRect = editor.GridToGuiDrawRect(value.SliderRect);
            var topBox = new Rect(headerRect.x, headerRect.y,
                headerRect.width, headerRect.height + sliderRect.height);
            
            // Draw top box
            GUIHelper.PushColor(value.HeaderColor);
            GUI.Box(topBox, "");
            if (value.HasLabelSlider && content.IsLabelSliderShown)
            {
                // Offset provide better visuals
                var offsettedSliderRect = new Rect(
                        sliderRect.x + SliderHorizontalSpacing,
                        sliderRect.y - SliderVerticalOffset,
                        sliderRect.width - 2 * SliderHorizontalSpacing,
                        sliderRect.height);

                content.LabelWidth = GUI.HorizontalSlider(offsettedSliderRect,
                    value.LabelWidth, 0, content.NodeRect.width);
            }
            GUIHelper.PopColor();

            var titleGUIStyle = new GUIStyle(GUI.skin.GetStyle("Label"))
            {
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = value.TitleColor }
            };

            GUI.Label(headerRect, value.Title, titleGUIStyle);

            // Draw content
            var contentRect = value.ContentRect;
            GUILayout.BeginArea(editor.GridToGuiDrawRect(
                new Rect(contentRect.x, contentRect.y, contentRect.width, contentRect.height)));
            var contentBoxRect = SirenixEditorGUI.BeginBox();

            // TODO labeling stuff.
            // When label width equals 0 it is drawn with default inspector width.
            var labelWidth = value.LabelWidth;
            GUIHelper.PushLabelWidth(labelWidth);
            GUIHelper.PushHierarchyMode(false);
            CallNextDrawer(label);
            GUIHelper.PopHierarchyMode();
            GUIHelper.PopLabelWidth();

            if (Event.current.type == EventType.Repaint)
                content.NodeRect.height = contentBoxRect.height + topBox.height;

            SirenixEditorGUI.EndBox();
            GUILayout.EndArea();

            foreach (var editorSlot in value.Slots)
                DrawSlot(editorSlot);
        }

        public static void DrawSlot(EditorSlot slot)
        {
            var editor = (GraphEditor)GUIHelper.CurrentWindow;

            GUIHelper.PushColor(new Color(0.95f, 0.38f, 0.24f));
            if (GUI.Button(editor.GridToGuiDrawRect(slot.Rect), ""))
                editor.OnSlotClick(slot.Content, slot.Direction);
            GUIHelper.PopColor();
        }
    }

    [DrawerPriority(DrawerPriorityLevel.WrapperPriority)]
    public class EditorNodeListDrawer<TListType>
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
