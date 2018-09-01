using System;
using UnityEngine;

namespace Sleipnir.Editor
{
    public partial class GraphEditor
    {
        public static Lazy<Texture2D> GridTexture = new Lazy<Texture2D>
            (() => GenerateGridTexture(new Color(0.25f, 0.25f, 0.25f), Color.gray));

        public static Lazy<Texture2D> CrossTexture = new Lazy<Texture2D>
            (() => GenerateCrossTexture(new Color(0.75f, 0.75f, 0.75f)));

        private static Texture2D GenerateGridTexture(Color line, Color background)
        {
            var textures = new Texture2D(64, 64);
            var colors = new Color[64 * 64];
            for (var y = 0; y < 64; y++)
            {
                for (var x = 0; x < 64; x++)
                {
                    var col = background;
                    if (y % 16 == 0 || x % 16 == 0) col = Color.Lerp(line, background, 0.65f);
                    if (y == 63 || x == 63) col = Color.Lerp(line, background, 0.35f);
                    colors[y * 64 + x] = col;
                }
            }
            textures.SetPixels(colors);
            textures.wrapMode = TextureWrapMode.Repeat;
            textures.filterMode = FilterMode.Bilinear;
            textures.name = "Grid";
            textures.Apply();
            return textures;
        }

        private static Texture2D GenerateCrossTexture(Color line)
        {
            var texture = new Texture2D(64, 64);
            var colors = new Color[64 * 64];
            for (var y = 0; y < 64; y++)
            {
                for (var x = 0; x < 64; x++)
                {
                    var col = line;
                    if (y != 31 && x != 31) col.a = 0;
                    colors[y * 64 + x] = col;
                }
            }
            texture.SetPixels(colors);
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Bilinear;
            texture.name = "Cross";
            texture.Apply();
            return texture;
        }
    }
}