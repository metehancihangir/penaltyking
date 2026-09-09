using UnityEngine;
using UnityEngine.UI;

namespace PenaltyKing
{
    // Code-native pixel lettering and sparks, sampled by the shot's paused timeline.
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class GoalCelebration : MaskableGraphic
    {
        private float age = -1;
        public bool Visible => age >= 0 && age < ShotPresentation.GoalCelebrationDuration;
        private static readonly string[] Letters = {
            "01110100001000010111100011000101110",
            "01110100011000110001100011000101110",
            "01110100011000111111100011000110001",
            "10000100001000010000100001000011111",
            "00100001000010000100001000000000100"
        };
        public void Sample(float seconds)
        {
            age = seconds;
            raycastTarget = false;
            SetVerticesDirty();
        }
        protected override void OnPopulateMesh(VertexHelper mesh)
        {
            mesh.Clear();
            if (!Visible) return;
            var alpha = Mathf.Clamp01(age / .10f) * Mathf.Clamp01((5 - age) / .4f);
            var pop = age < .35f ? 1 - Mathf.Exp(-age * 12) * Mathf.Cos(age * 16) : 1;
            var scale = Mathf.Max(.1f, pop) * (1 + .018f * Mathf.Sin(age * 4));
            const float pixel = 9;
            // Dark extrusion, lime outline, warm ivory face; no full-screen flash.
            for (var layer = 0; layer < 3; layer++)
            for (var letter = 0; letter < Letters.Length; letter++)
            for (var row = 0; row < 7; row++)
            for (var col = 0; col < 5; col++)
            {
                if (Letters[letter][row * 5 + col] != '1') continue;
                var p = new Vector2((letter * 6 + col - 14.5f) * pixel, (3 - row) * pixel);
                var size = pixel;
                Color tint;
                if (layer == 0) { p += new Vector2(4, -7); size += 8; tint = new Color32(10, 30, 43, 255); }
                else if (layer == 1) { size += 4; tint = new Color32(183, 233, 108, 255); }
                else tint = new Color32(242, 247, 220, 255);
                tint.a *= alpha;
                Quad(mesh, p * scale, Vector2.one * size * scale, tint);
            }
            for (var burst = 0; burst < 4; burst++)
            {
                var t = age - (.25f + burst * 1.05f);
                if (t < 0 || t > 1.1f) continue;
                var origin = new Vector2(burst % 2 == 0 ? -195 : 195, burst < 2 ? 8 : 30);
                for (var i = 0; i < 12; i++)
                {
                    var angle = i * Mathf.PI / 6 + burst * .27f;
                    var p = origin + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * (8 + t * 65);
                    p.y -= t * t * 32;
                    p = new Vector2(Mathf.Round(p.x / 3) * 3, Mathf.Round(p.y / 3) * 3);
                    Color tint = burst % 2 == 0 ? new Color32(255, 205, 98, 255) : new Color32(115, 217, 235, 255);
                    tint.a *= alpha * Mathf.Clamp01((1.1f - t) / .65f);
                    Quad(mesh, p, Vector2.one * (t < .25f ? 5 : 3), tint);
                }
            }
        }
        private static void Quad(VertexHelper mesh, Vector2 p, Vector2 size, Color colour)
        {
            var n = mesh.currentVertCount;
            var half = size * .5f;
            mesh.AddVert(p + new Vector2(-half.x, -half.y), colour, Vector2.zero);
            mesh.AddVert(p + new Vector2(-half.x, half.y), colour, Vector2.zero);
            mesh.AddVert(p + new Vector2(half.x, half.y), colour, Vector2.zero);
            mesh.AddVert(p + new Vector2(half.x, -half.y), colour, Vector2.zero);
            mesh.AddTriangle(n, n + 1, n + 2); mesh.AddTriangle(n + 2, n + 3, n);
        }
    }
}
