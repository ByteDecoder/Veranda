using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEngine;

namespace Sleipnir.Editor
{
    public class NodeDrawer : OdinValueDrawer<Node>
    { 
        private const float SliderHorizontalSpacing = 8f;
        private const float SliderVerticalOffset = 4f;

        protected override void DrawPropertyLayout(GUIContent label)
        {
            var editor = GUIHelper.CurrentWindow as GraphEditor;
            if (editor == null)
            {
                CallNextDrawer(label);
                return;
            }

            var node = ValueEntry.SmartValue;
            var serializedNodeData = node.SerializedNodeData;

            var headerGUIRect = editor.GridToGUIDrawRect(node.HeaderRect());
            var sliderGUIRect = editor.GridToGUIDrawRect(node.SliderRect());
            var topBox = editor.GridToGUIDrawRect(node.TopRect());
            
            // Draw top box
            GUIHelper.PushColor(node.HeaderColor);
            GUI.Box(topBox, "");
            if (node.HasLabelSlider && serializedNodeData.IsLabelSliderShown)
            {
                // Offset provide better visuals
                var offsettedSliderRect = new Rect(
                    sliderGUIRect.x + SliderHorizontalSpacing,
                    sliderGUIRect.y - SliderVerticalOffset,
                    sliderGUIRect.width - 2 * SliderHorizontalSpacing,
                    sliderGUIRect.height
                    );

                serializedNodeData.LabelWidth = GUI.HorizontalSlider(
                    offsettedSliderRect,
                    serializedNodeData.LabelWidth, 
                    0,
                    serializedNodeData.GridRect.width
                    );
            }
            GUIHelper.PopColor();

            var titleGUIStyle = new GUIStyle(GUI.skin.GetStyle("Label"))
            {
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = node.TitleColor }
            };

            GUI.Label(headerGUIRect, node.HeaderTitle, titleGUIStyle);

            // Draw content
            var contentRect = node.ContentRect();
            GUILayout.BeginArea(editor.GridToGUIDrawRect(
                new Rect(contentRect.x, contentRect.y, contentRect.width, contentRect.height)));
            var contentBoxRect = SirenixEditorGUI.BeginBox();

            // 0 is inspector default.
            var labelWidth = Mathf.Max(serializedNodeData.LabelWidth, Mathf.Epsilon);
            GUIHelper.PushLabelWidth(labelWidth);
            GUIHelper.PushHierarchyMode(false);
            CallNextDrawer(label);
            GUIHelper.PopHierarchyMode();
            GUIHelper.PopLabelWidth();

            if (Event.current.type == EventType.Repaint)
                node.SetNodeContentHeight(contentBoxRect.height);

            SirenixEditorGUI.EndBox();
            GUILayout.EndArea();

            foreach (var slot in node.Slots)
            {
                GUIHelper.PushGUIEnabled(slot.Interactable);
                GUIHelper.PushColor(slot.Color);
                if (GUI.Button(editor.GridToGUIDrawRect(slot.GridRect), ""))
                    editor.OnSlotButtonClick(slot);
                GUIHelper.PopColor();
                GUIHelper.PopGUIEnabled();
            }
        }
    }

    [DrawerPriority(DrawerPriorityLevel.WrapperPriority)]
    public class EditorNodeListDrawer<TListType>
        : OdinValueDrawer<TListType> where TListType : IEnumerable<Node>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            foreach (var propertyChild in ValueEntry.Property.Children)
                if ((Node)propertyChild.ValueEntry.WeakSmartValue != null)
                    propertyChild.Draw();
        }
    }
}
