using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace PenaltyKing.Editor
{
    public static class MenuPreview
    {
        public static void BuildAndCapture()
        {
            PhaseOneSetup.CreateMainMenu();
            Capture();
        }

        public static void Capture()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
            Directory.CreateDirectory("docs/previews");
            Render(1280, 720, "landscape");
            Render(390, 844, "portrait");
            Debug.Log("[Phase 1] Landscape and portrait previews captured.");
        }

        public static void CaptureOptions()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/Options.unity");
            Directory.CreateDirectory("docs/previews");
            Render(1280, 720, "landscape", "options");
            Render(390, 844, "portrait", "options");
            Debug.Log("[Phase 2] Options previews captured.");
        }

        public static void CaptureSelections()
        {
            Directory.CreateDirectory("docs/previews");
            foreach (var sceneName in new[] { "DifficultySelect", "ModeSelect" })
            {
                EditorSceneManager.OpenScene($"Assets/Scenes/{sceneName}.unity");
                Render(1280, 720, "landscape", sceneName);
                Render(390, 844, "portrait", sceneName);
            }
            Debug.Log("[Phase 3] Selection previews captured in landscape and portrait.");
        }

        private static void Render(int width, int height, string name, string prefix = "main-menu")
        {
            var camera = Camera.main;
            var canvas = Object.FindFirstObjectByType<Canvas>();
            var scaler = canvas.GetComponent<CanvasScaler>();
            scaler.enabled = false;
            canvas.scaleFactor = Mathf.Sqrt(width / 960f * height / 720f);
            var target = new RenderTexture(width, height, 24);
            var previous = RenderTexture.active;
            camera.targetTexture = target;
            Canvas.ForceUpdateCanvases();
            foreach (var text in Object.FindObjectsByType<Text>(FindObjectsSortMode.None))
                text.font.RequestCharactersInTexture(text.text, text.fontSize, text.fontStyle);
            Canvas.ForceUpdateCanvases();
            // Render twice to allow the font atlas and target-sized UI geometry to settle.
            camera.Render();
            Canvas.ForceUpdateCanvases();
            camera.Render();
            foreach (var button in Object.FindObjectsByType<Selectable>(FindObjectsSortMode.None))
            {
                var corners = new Vector3[4];
                ((RectTransform)button.transform).GetWorldCorners(corners);
                foreach (var corner in corners)
                {
                    var point = camera.WorldToViewportPoint(corner);
                    if (point.x < 0 || point.x > 1 || point.y < 0 || point.y > 1)
                        throw new System.InvalidOperationException($"{button.name} exceeds {width}x{height} viewport.");
                }
            }
            RenderTexture.active = target;
            var texture = new Texture2D(width, height, TextureFormat.RGB24, false);
            texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            texture.Apply();
            File.WriteAllBytes($"docs/previews/{prefix}-{name}.png", texture.EncodeToPNG());
            camera.targetTexture = null;
            RenderTexture.active = previous;
            Object.DestroyImmediate(texture);
            target.Release();
            Object.DestroyImmediate(target);
        }
    }
}
