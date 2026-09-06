using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using static PenaltyKing.Editor.PhaseOneSetup;

namespace PenaltyKing.Editor
{
    public static class PhaseFourSetup
    {
        private static Color C(string hex) => MenuPixelArt.Hex(hex);
        [MenuItem("Penalty King/Phase 4/Create placeholder gameplay")]
        public static void CreateGameplay()
        {
            var scene = EditorSceneManager.OpenScene("Assets/Scenes/Gameplay.unity");
            var existing = Object.FindFirstObjectByType<GameplayController>();
            if (existing != null) Object.DestroyImmediate(existing);
            var diagnostics = Object.FindFirstObjectByType<PhaseZeroDiagnostics>();
            if (diagnostics != null) Object.DestroyImmediate(diagnostics);
            var oldCanvas = GameObject.Find("Gameplay Canvas");
            if (oldCanvas != null) Object.DestroyImmediate(oldCanvas);
            if (Object.FindFirstObjectByType<EventSystem>() == null)
                new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            var root = new GameObject("Gameplay Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = root.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera; canvas.worldCamera = Camera.main;
            canvas.planeDistance = 5; canvas.pixelPerfect = true;
            var scaler = root.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(960, 720); scaler.matchWidthOrHeight = 0.5f;
            var background = Image("Background", root.transform, Vector2.zero, Vector2.zero, C("0B1429"));
            Stretch(background.rectTransform);
            var safe = Rect("Safe Area", root.transform, Vector2.zero, Vector2.zero);
            Stretch(safe); safe.gameObject.AddComponent<SafeAreaPanel>();
            var panel = Image("Placeholder Pitch", safe, new Vector2(440, 566), Vector2.zero, C("153C35"));
            var mode = Label("Mode", panel.transform, "SABİT ROUND · Orta", new Vector2(390, 24), new Vector2(0, 252), 16, C("AEC4C0"));
            var score = Label("Score", panel.transform, "GOL  0", new Vector2(180, 38), new Vector2(-100, 209), 26, C("BAEB71"));
            var shots = Label("Shots", panel.transform, "ŞUT  0 / 5", new Vector2(190, 38), new Vector2(95, 209), 23, C("EBF5DF"));
            Image("Goal Frame", panel.transform, new Vector2(398, 152), new Vector2(0, 65), C("AEC4C0"));
            var left = Zone(panel.transform, "SOL", -128);
            var center = Zone(panel.transform, "ORTA", 0);
            var right = Zone(panel.transform, "SAĞ", 128);
            var keeper = Image("Keeper Placeholder", panel.transform, new Vector2(30, 42), new Vector2(0, 80), C("E5B554"));
            Label("Keeper Label", keeper.transform, "K", new Vector2(24, 32), Vector2.zero, 20, C("17322B"));
            var ball = Image("Ball Placeholder", panel.transform, new Vector2(16, 16), new Vector2(0, -58), C("EBF5DF"));
            var shooter = Image("Shooter Placeholder", panel.transform, new Vector2(30, 40), new Vector2(0, -120), C("C96960"));
            Label("Shooter Label", shooter.transform, "O", new Vector2(24, 32), Vector2.zero, 20, C("EBF5DF"));
            var feedback = Label("Feedback", panel.transform, "BİR YÖNE DOKUN", new Vector2(400, 36), new Vector2(0, -183), 25, C("EBF5DF"));
            var directions = Label("Directions", panel.transform, "Sol, orta veya sağ.", new Vector2(410, 26), new Vector2(0, -221), 16, C("AEC4C0"));

            var summary = Image("Round Summary", panel.transform, new Vector2(408, 380), new Vector2(0, -20), C("101E2B"));
            summary.raycastTarget = true;
            var title = Label("Title", summary.transform, "ROUND TAMAMLANDI", new Vector2(390, 44), new Vector2(0, 135), 26, C("EBF5DF"));
            var result = Label("Final Score", summary.transform, "0 / 5 GOL", new Vector2(380, 50), new Vector2(0, 67), 30, C("BAEB71"));
            var replay = Button(summary.transform, "Tekrar Oyna", -33, true, true);
            var home = Button(summary.transform, "Ana Menüye Dön", -125, true, false);
            summary.gameObject.SetActive(false);
            var controller = Object.FindFirstObjectByType<SceneNavigator>().gameObject.AddComponent<GameplayController>();
            controller.Configure(left, center, right, score, shots, mode, feedback, directions, ball.rectTransform, keeper.rectTransform,
                summary.gameObject, title, result, replay, home);
            EditorUtility.SetDirty(controller);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            Debug.Log("[Phase 4] Placeholder gameplay, three touch zones and round summary created.");
        }

        public static void BuildAndPreview() { CreateGameplay(); MenuPreview.CaptureGameplay(); }

        private static ShotZone Zone(Transform parent, string title, float x)
        {
            var image = Image(title + " Shot Zone", parent, new Vector2(124, 140), new Vector2(x, 65), Color.white);
            image.raycastTarget = true;
            var button = image.gameObject.AddComponent<ShotZone>();
            button.targetGraphic = image;
            button.navigation = new Navigation { mode = Navigation.Mode.None };
            var colors = ColorBlock.defaultColorBlock;
            colors.normalColor = C("274D49"); colors.highlightedColor = C("376D5D");
            colors.pressedColor = C("63865F"); colors.disabledColor = C("233D39"); colors.fadeDuration = 0;
            button.colors = colors;
            Label("Direction", image.transform, title, new Vector2(116, 28), new Vector2(0, -49), 18, C("EBF5DF"));
            return button;
        }
    }
}
