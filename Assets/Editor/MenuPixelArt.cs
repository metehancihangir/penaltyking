using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace PenaltyKing.Editor
{
    // Original, deliberately temporary menu artwork. No reference image pixels are copied.
    internal static class MenuPixelArt
    {
        internal static Color Hex(string color) { ColorUtility.TryParseHtmlString("#" + color, out var value); return value; }

        internal static Sprite Stadium()
        {
            var texture = new Texture2D(320, 180, TextureFormat.RGBA32, false);
            var pixels = new Color[320 * 180];
            for (var y = 0; y < 180; y++)
                for (var x = 0; x < 320; x++)
                    pixels[y * 320 + x] = Hex(y < 62 ? ((y / 9) % 2 == 0 ? "123B38" : "16443F") : y < 108 ? "152636" : "0B1429");
            texture.SetPixels(pixels);
            void Box(int x, int y, int w, int h, string color)
            {
                for (var py = Mathf.Max(0, y); py < Mathf.Min(180, y + h); py++)
                    for (var px = Mathf.Max(0, x); px < Mathf.Min(320, x + w); px++) texture.SetPixel(px, py, Hex(color));
            }
            var random = new System.Random(19);
            for (var i = 0; i < 55; i++) Box(random.Next(320), random.Next(120, 176), 1, 1, "4A647F");
            Box(0, 106, 320, 3, "2B4554");
            for (var row = 0; row < 5; row++)
            {
                Box(0, 72 + row * 7, 320, 1, "29414B");
                for (var x = 2; x < 319; x += 5)
                    Box(x, 74 + row * 7, 2, 2, random.Next(3) == 0 ? "648387" : "35515F");
            }
            Box(0, 63, 320, 6, "203B49");
            Box(0, 62, 320, 1, "76A6A0");
            Box(0, 28, 320, 1, "48776A");
            for (var y = 0; y < 62; y++)
            {
                Box(54 - (62 - y) / 2, y, 1, 1, "48776A");
                Box(265 + (62 - y) / 2, y, 1, 1, "48776A");
            }
            // Stadium floodlights and stepped, subtle cones.
            foreach (var x in new[] { 25, 281 })
            {
                Box(x + 5, 91, 2, 62, "45606E");
                for (var y = 108; y < 153; y++)
                {
                    var half = (153 - y) / 3 + 5;
                    Box(x + 6 - half, y, half * 2, 1, "172D40");
                }
                Box(x - 7, 153, 27, 9, "486C7B");
                for (var k = 0; k < 4; k++) Box(x - 5 + k * 6, 155, 4, 5, "C9F4E6");
            }
            // Distant goal silhouette, behind the menu.
            for (var x = 101; x <= 219; x += 5) Box(x, 49, 1, 31, "365B60");
            for (var y = 49; y <= 80; y += 5) Box(101, y, 119, 1, "365B60");
            Box(99, 48, 2, 35, "90B9B0"); Box(219, 48, 2, 35, "90B9B0"); Box(99, 81, 122, 2, "AED1C3");
            return Save(texture, "Assets/UI/Menu/StadiumPlaceholder.png");
        }

        internal static Sprite Title()
        {
            var glyphs = new Dictionary<char, string>
            {
                ['P']="11110/10001/10001/11110/10000/10000/10000",
                ['E']="11111/10000/10000/11110/10000/10000/11111",
                ['N']="10001/11001/11001/10101/10011/10011/10001",
                ['A']="01110/10001/10001/11111/10001/10001/10001",
                ['L']="10000/10000/10000/10000/10000/10000/11111",
                ['T']="11111/00100/00100/00100/00100/00100/00100",
                ['Y']="10001/10001/01010/00100/00100/00100/00100",
                ['K']="10001/10010/10100/11000/10100/10010/10001",
                ['I']="11111/00100/00100/00100/00100/00100/11111",
                ['G']="01111/10000/10000/10111/10001/10001/01110"
            };
            var texture = new Texture2D(43, 20, TextureFormat.RGBA32, false);
            texture.SetPixels(new Color[43 * 20]);
            void Word(string word, int startX, int startY, string color)
            {
                for (var i = 0; i < word.Length; i++)
                {
                    var rows = glyphs[word[i]].Split('/');
                    for (var y = 0; y < 7; y++)
                        for (var x = 0; x < 5; x++)
                            if (rows[y][x] == '1') texture.SetPixel(startX + i * 6 + x, startY + 6 - y, Hex(color));
                }
            }
            Word("PENALTY", 1, 12, "EBF5DF");
            Word("KING", 10, 2, "BAEB71");
            return Save(texture, "Assets/UI/Menu/Title.png");
        }

        private static Sprite Save(Texture2D texture, string path)
        {
            texture.Apply();
            File.WriteAllBytes(path, texture.EncodeToPNG());
            Object.DestroyImmediate(texture);
            AssetDatabase.ImportAsset(path);
            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = 1;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.mipmapEnabled = false;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.SaveAndReimport();
            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }
    }
}
