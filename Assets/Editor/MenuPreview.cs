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

        public static void CaptureGameplay()
        {
            Directory.CreateDirectory("docs/previews");
            EditorSceneManager.OpenScene("Assets/Scenes/Gameplay.unity");
            Render(1280, 720, "landscape", "gameplay-placeholder");
            Render(390, 844, "portrait", "gameplay-placeholder");
            GameObject.Find("Gameplay Canvas/Safe Area/Placeholder Pitch").transform.Find("Round Summary").gameObject.SetActive(true);
            Render(1280, 720, "landscape", "round-summary-placeholder");
            Render(390, 844, "portrait", "round-summary-placeholder");
            Debug.Log("[Phase 4] Gameplay and summary previews captured.");
        }

        public static void CaptureFinalArt()
        {
            Directory.CreateDirectory("docs/previews");
            EditorSceneManager.OpenScene("Assets/Scenes/Gameplay.unity");
            Render(1280, 720, "landscape", "phase5-gameplay");
            Render(390, 844, "portrait", "phase5-gameplay");
            Debug.Log("[Phase 5] Stadium composition previews captured.");
        }

        public static void CaptureAnimations()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/Gameplay.unity");
            var animation = Object.FindFirstObjectByType<ShotPresentation>();
            Render(1280, 720, "idle", "phase6");
            var stage = Object.FindFirstObjectByType<GameplayStageLayout>(); stage.Fit();
            animation.SetComposition(stage.transform.Find("08 Ball").GetComponent<RectTransform>().anchoredPosition,
                stage.transform.Find("06 Keeper").GetComponent<RectTransform>().anchoredPosition, 190,
                stage.transform.Find("06 Keeper").GetComponent<RectTransform>().anchoredPosition.y + 21);
            foreach (var direction in new[] { ShotDirection.Left, ShotDirection.Center, ShotDirection.Right })
            {
                animation.Sample(new ShotResult(direction, direction), .9f);
                Render(1280, 720, direction.ToString().ToLowerInvariant() + "-save", "phase6");
            }
            animation.Sample(new ShotResult(ShotDirection.Right, ShotDirection.Left), .24f);
            Render(1280, 720, "kick-contact", "phase6");
            animation.Sample(new ShotResult(ShotDirection.Right, ShotDirection.Left), 1.15f);
            Render(1280, 720, "goal-celebration", "phase6");
            animation.ResetPose();
        }

        internal static void Render(int width, int height, string name, string prefix = "main-menu")
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
            foreach (var stage in Object.FindObjectsByType<GameplayStageLayout>(FindObjectsSortMode.None)) stage.Fit();
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
