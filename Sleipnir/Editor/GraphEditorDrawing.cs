using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Sleipnir.Editor
{
    public partial class GraphEditor
    {
        protected override void DrawEditor(int index)
        {
            if (_graph == null)
            {
                DrawGrid();
                return;
            }

            var matrix = GUI.matrix;
            ProcessInput();
            DrawGrid();
            BeginZoomed();

            foreach (var connection in _graph.Connections())
                DrawConnection(connection);

            DrawConnectionToMouse();
            base.DrawEditor(index);

            EndZoomed();
            GUI.matrix = matrix;
        }

        #region Grid
        private void DrawGrid()
        {
            var rect = new Rect(Vector2.zero, position.size);
            var center = rect.size / 2f;

            // Offset from origin in tile units
            var xOffset = -(center.x * Scale + Position.x)
                / GridTexture.Value.width;
            var yOffset = ((center.y - rect.size.y) * Scale + Position.y)
                / GridTexture.Value.height;

            var tileOffset = new Vector2(xOffset, yOffset);

            // Amount of tiles
            var tileAmountX = Mathf.Round(rect.size.x * Scale)
                / GridTexture.Value.width;
            var tileAmountY = Mathf.Round(rect.size.y * Scale)
                / GridTexture.Value.height;

            var tileAmount = new Vector2(tileAmountX, tileAmountY);

            // Draw tiled background
            GUI.DrawTextureWithTexCoords(rect, GridTexture.Value,
                new Rect(tileOffset, tileAmount));
            GUI.DrawTextureWithTexCoords(rect, CrossTexture.Value,
                new Rect(tileOffset + new Vector2(0.5f, 0.5f), tileAmount));
        }
        #endregion

        #region Connections
        private void DrawConnectionToMouse()
        {
            if (_selectedKnob == null)
                return;

            var knobPosition = GridToGuiPositionNoClip(_selectedKnob.Rect.center);
            var mousePosition = Event.current.mousePosition;

            if (_selectedKnob.Type == KnobType.Output)
                DrawConnection(knobPosition, mousePosition, _selectedKnob.Color);
            else
                DrawConnection(mousePosition, knobPosition, _selectedKnob.Color);
        }

        private static Color ConnectionColor(Knob outputKnob, Knob inputKnob)
        {
            return outputKnob.Color == inputKnob.Color
                ? outputKnob.Color
                : Color.Lerp(inputKnob.Color, outputKnob.Color, 0.5f);
        }

        private static void DrawConnection(Vector2 startGridPosition, Vector2 endGridPosition,
            Color color, float width = ConnectionLineWidth, Texture2D texture = null)
        {
            Handles.DrawBezier(
                startGridPosition,
                endGridPosition,
                startGridPosition + Vector2.right * ConnectionTangentMultiplier,
                endGridPosition + Vector2.left * ConnectionTangentMultiplier,
                color,
                texture,
                width
            );
        }

        private void DrawConnection(Connection connection)
        {
            var start = GridToGuiPositionNoClip(connection.OutputKnob.Rect.center);
            var end = GridToGuiPositionNoClip(connection.InputKnob.Rect.center);
            DrawConnection(start, end, ConnectionColor(connection.OutputKnob, connection.InputKnob));
        }
        #endregion

        #region Knobs
        // y axis is inverted
        public void Draw(Knob knob, EditorNode editorNode)
        {
            var rect = GetKnobRect(knob, editorNode);
            knob.Rect = rect;

            GUIHelper.PushColor(knob.Color);
            if (GUI.Button(GridToGuiDrawRect(rect), ""))
                OnKnobClick(knob);
            GUIHelper.PopColor();

            // knob label
            if (knob.Description.IsNullOrWhitespace())
                return;

            var style = KnobLabelGUIStyle.Value;
            var labelContent = new GUIContent(knob.Description);
            var labelSize = style.CalcSize(labelContent);

            var labelPosition = knob.Type == KnobType.Input
                ? rect.position + new Vector2(-labelSize.x - KnobLabelOffset.x, KnobSize.y + KnobLabelOffset.y)
                : rect.position + new Vector2(KnobSize.x + KnobLabelOffset.x, -labelSize.y - KnobLabelOffset.y);

            var labelRect = new Rect(labelPosition, labelSize);
            GUI.Label(GridToGuiDrawRect(labelRect), labelContent, style);
        }
        
        private static Rect GetKnobRect(Knob knob, EditorNode editorNode)
        {
            var yPosition = editorNode.Content.HeaderRect.position.y + knob.RelativeYPosition;
            var xPosition = knob.Type == KnobType.Input
                ? editorNode.Content.HeaderRect.position.x - KnobSize.x - KnobHorizontalOffset
                : editorNode.Content.HeaderRect.position.x + editorNode.Content.HeaderRect.width + KnobHorizontalOffset;
            return new Rect(xPosition, yPosition, KnobSize.x, KnobSize.y);
        }
        #endregion
    }
}