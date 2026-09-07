using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using static PenaltyKing.Editor.PhaseOneSetup;

namespace PenaltyKing.Editor
{
    public static class PhaseThreeSetup
    {
        private static Color C(string value) => MenuPixelArt.Hex(value);

        [MenuItem("Penalty King/Phase 3/Create selection screens")]
        public static void CreateSelections()
        {
            Directory.CreateDirectory("Assets/Resources");
            if (AssetDatabase.LoadAssetAtPath<GameRules>("Assets/Resources/GameRules.asset") == null)
                AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<GameRules>(), "Assets/Resources/GameRules.asset");
            CreateDifficulty();
            CreateMode();
            AssetDatabase.SaveAssets();
            EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
            Debug.Log("[Phase 3] Difficulty and mode screens saved. Rules: editable GameRules asset.");
        }

        public static void BuildAndPreview()
        {
            CreateSelections();
            MenuPreview.CaptureSelections();
        }

        private static void CreateDifficulty()
        {
            var panel = CreatePanel("DifficultySelect", "ZORLUK SEÇ", "1 / 2 · SINGLEPLAYER");
            Label("Subtitle", panel, "Kalecinin seviyesini belirle.", new Vector2(380, 24), new Vector2(0, 135), 17, C("AEC4C0"));
            var easy = Button(panel, "Kolay", 65, true, false);
            var medium = Button(panel, "Orta", -23, true, false);
            var hard = Button(panel, "Zor", -111, true, false);
            Label("Fixed Difficulty", panel, "Zorluk maç boyunca sabit kalır.", new Vector2(390, 24), new Vector2(0, -165), 14, C("86A9A6"));
            var back = BackButton(panel, -224);
            var controller = Object.FindFirstObjectByType<SceneNavigator>().gameObject.AddComponent<DifficultySelectController>();
            controller.Configure(easy, medium, hard, back);
            EditorUtility.SetDirty(controller);
            EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        }

        internal static void CreateMode()
        {
            var panel = CreatePanel("ModeSelect", "MOD SEÇ", "2 / 2 · SINGLEPLAYER");
            var difficulty = Label("Selected Difficulty", panel, "Zorluk: Orta", new Vector2(380, 28), new Vector2(0, 129), 18, C("BAEB71"));
            var fixedRound = Button(panel, "Sabit Round", 44, true, true);
            var rules = AssetDatabase.LoadAssetAtPath<GameRules>("Assets/Resources/GameRules.asset");
            var description = Label("Round Description", panel, $"{rules.FixedRoundShots} şut. Her gol skora eklenir.", new Vector2(380, 28), new Vector2(0, -13), 16, C("AEC4C0"));
            var endless = Button(panel, "Endless", -91, true, false);
            Label("Endless Description", panel, "İlk kurtarışa kadar devam et.", new Vector2(380, 28), new Vector2(0, -148), 16, C("AEC4C0"));
            var back = BackButton(panel, -224);
            var controller = Object.FindFirstObjectByType<SceneNavigator>().gameObject.AddComponent<ModeSelectController>();
            controller.Configure(fixedRound, endless, back, difficulty, description);
            EditorUtility.SetDirty(controller);
            EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        }

        private static Transform CreatePanel(string sceneName, string title, string step)
        {
            EditorSceneManager.OpenScene($"Assets/Scenes/{sceneName}.unity");
            var oldDifficulty = Object.FindFirstObjectByType<DifficultySelectController>();
            if (oldDifficulty != null) Object.DestroyImmediate(oldDifficulty);
            var oldMode = Object.FindFirstObjectByType<ModeSelectController>();
            if (oldMode != null) Object.DestroyImmediate(oldMode);
            var diagnostics = Object.FindFirstObjectByType<PhaseZeroDiagnostics>();
            if (diagnostics != null) Object.DestroyImmediate(diagnostics);
            var previous = GameObject.Find("Selection Canvas");
            if (previous != null) Object.DestroyImmediate(previous);
            if (Object.FindFirstObjectByType<EventSystem>() == null)
                new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            var go = new GameObject("Selection Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = go.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = Camera.main;
            canvas.planeDistance = 5;
            canvas.pixelPerfect = true;
            var scaler = go.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(960, 720);
            scaler.matchWidthOrHeight = 0.5f;
            var background = Image("Night Stadium", go.transform, Vector2.zero, Vector2.zero, Color.white);
            Stretch(background.rectTransform);
            background.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/UI/Menu/StadiumPlaceholder.png");
            var safe = Rect("Safe Area", go.transform, Vector2.zero, Vector2.zero);
            Stretch(safe);
            safe.gameObject.AddComponent<SafeAreaPanel>();
            var panel = Image("Selection Panel", safe, new Vector2(440, 566), Vector2.zero, C("101E2B"));
            Image("Top Accent", panel.transform, new Vector2(440, 4), new Vector2(0, 281), C("BAEB71"));
            Image("Bottom Edge", panel.transform, new Vector2(440, 4), new Vector2(0, -281), C("315153"));
            Label("Step", panel.transform, step, new Vector2(380, 24), new Vector2(0, 247), 14, C("86A9A6"));
            Label("Title", panel.transform, title, new Vector2(390, 60), new Vector2(0, 185), 36, C("EBF5DF"));
            return panel.transform;
        }

        private static PixelMenuButton BackButton(Transform panel, float y)
        {
            var button = Button(panel, "Geri", y, true, false);
            var arrow = button.transform.Find("Face/Arrow").GetComponent<Text>();
            arrow.text = "<";
            arrow.rectTransform.anchoredPosition = new Vector2(-152, 0);
            return button;
        }
    }
}
