using UnityEngine;

namespace Sleipnir.Editor
{
    public partial class GraphEditor
    {
        [HideInInspector]
        public EditorNode CurrentlyDrawedNode;

        protected override void DrawEditor(int index)
        {
            if (_graph == null)
                return;

            var matrix = GUI.matrix;
            ProcessInput();
            DrawGrid();
            BeginZoomed();
            base.DrawEditor(index);
            DrawConnectionToMouse();
            EndZoomed();
            GUI.matrix = matrix;
        }
        
        private void DrawGrid()
        {
            if (_gridTexture == null || _crossTexture == null)
                return;

            var windowRect = new Rect(Vector2.zero, position.size);
            var center = windowRect.size * 0.5f;

            // Offset from origin in tile units
            var xOffset = -(center.x * Scale + Pan.x)
                / _gridTexture.width;
            var yOffset = ((center.y - windowRect.size.y) * Scale + Pan.y)
                / _gridTexture.height;

            var tileOffset = new Vector2(xOffset, yOffset);

            // Amount of tiles
            var tileAmountX = Mathf.Round(windowRect.size.x * Scale)
                / _gridTexture.width;
            var tileAmountY = Mathf.Round(windowRect.size.y * Scale)
                / _gridTexture.height;

            var tileAmount = new Vector2(tileAmountX, tileAmountY);

            // Draw tiled background
            GUI.DrawTextureWithTexCoords(windowRect, _gridTexture,
                new Rect(tileOffset, tileAmount));
            GUI.DrawTextureWithTexCoords(windowRect, _crossTexture,
                new Rect(tileOffset + new Vector2(0.5f, 0.5f), tileAmount));
        }

        private void DrawConnectionToMouse()
        {
            var selectedKnob = _selectedInputKnob ?? _selectedOutputKnob;

            if (selectedKnob == null)
                return;

            var knobPosition = GridToGuiPositionNoClip(GetKnobRect(selectedKnob).center);
            var mousePosition = Event.current.mousePosition;

            if (selectedKnob == _selectedOutputKnob)
                EditorConnectionDrawer.DrawConnection(knobPosition, mousePosition);
            else
                EditorConnectionDrawer.DrawConnection(mousePosition, knobPosition);
        }
    }
}