using System;
using Sirenix.Utilities.Editor;
using UnityEngine;

namespace Sleipnir.Editor
{        
    public partial class GraphEditor
    {
        [HideInInspector, NonSerialized]
        public Lazy<Texture2D> GridTexture = new Lazy<Texture2D>(() =>
            GenerateGridTexture(new Color(0.2f, 0.2f, 0.2f, 1f), Color.gray));

        [HideInInspector, NonSerialized]
        public Lazy<Texture2D> CrossTexture = new Lazy<Texture2D>(() => GenerateCrossTexture(Color.white));
    
        public static Texture2D GenerateGridTexture(Color line, Color bg) {
            var tex = new Texture2D(64, 64);
            var cols = new Color[64 * 64];
            for (var y = 0; y < 64; y++) {
                for (var x = 0; x < 64; x++) {
                    var col = bg;
                    if (y % 16 == 0 || x % 16 == 0) col = Color.Lerp(line, bg, 0.65f);
                    if (y == 63 || x == 63) col = Color.Lerp(line, bg, 0.35f);
                    cols[(y * 64) + x] = col;
                }
            }
            tex.SetPixels(cols);
            tex.wrapMode = TextureWrapMode.Repeat;
            tex.filterMode = FilterMode.Bilinear;
            tex.name = "Grid";
            tex.Apply();
            return tex;
        }

        public static Texture2D GenerateCrossTexture(Color line) {
            var tex = new Texture2D(64, 64);
            var cols = new Color[64 * 64];
            for (var y = 0; y < 64; y++) {
                for (var x = 0; x < 64; x++) {
                    var col = line;
                    if (y != 31 && x != 31) col.a = 0;
                    cols[(y * 64) + x] = col;
                }
            }
            tex.SetPixels(cols);
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Bilinear;
            tex.name = "Cross";
            tex.Apply();
            return tex;
        }
        
        private void BeginZoomed()
        {
            GUI.EndClip();
            GUI.EndClip();

            GUIUtility.ScaleAroundPivot(Vector2.one / Zoom, position.size * 0.5f);
            GUI.BeginClip(new Rect(-(position.width * Zoom - position.width) * 0.5f,
                -((position.height * Zoom - position.height) * 0.5f) + 22 * Zoom,
                position.width * Zoom,
                position.height * Zoom));
        }

        private void EndZoomed()
        {
            GUIUtility.ScaleAroundPivot(Vector2.one * Zoom, position.size * 0.5f);
            GUI.BeginClip(GUIHelper.CurrentWindow.position);
        }
        
        private void DrawGrid()
        {
            var rect = new Rect(Vector2.zero, position.size);
            var center = rect.size / 2f;
            // Offset from origin in tile units
            var xOffset = -(center.x * Zoom + Position.x) / GridTexture.Value.width;
            var yOffset = ((center.y - rect.size.y) * Zoom + Position.y) / GridTexture.Value.height;

            var tileOffset = new Vector2(xOffset, yOffset);

            // Amount of tiles
            var tileAmountX = Mathf.Round(rect.size.x * Zoom) / GridTexture.Value.width;
            var tileAmountY = Mathf.Round(rect.size.y * Zoom) / GridTexture.Value.height;

            var tileAmount = new Vector2(tileAmountX, tileAmountY);

            // Draw tiled background
            GUI.DrawTextureWithTexCoords(rect, GridTexture.Value, new Rect(tileOffset, tileAmount));
            GUI.DrawTextureWithTexCoords(rect, CrossTexture.Value, new Rect(tileOffset + new Vector2(0.5f, 0.5f), tileAmount));
        }
    }
}