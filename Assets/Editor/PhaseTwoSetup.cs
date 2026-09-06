using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using static PenaltyKing.Editor.PhaseOneSetup;

namespace PenaltyKing.Editor
{
    public static class PhaseTwoSetup
    {
        private static Color C(string value) => MenuPixelArt.Hex(value);

        public static void BuildAndPreview()
        {
            CreateOptions();
            MenuPreview.CaptureOptions();
        }

        [MenuItem("Penalty King/Phase 2/Create options screen")]
        public static void CreateOptions()
        {
            var scene = EditorSceneManager.OpenScene("Assets/Scenes/Options.unity");
            var navigator = Object.FindFirstObjectByType<SceneNavigator>();
            var old = Object.FindFirstObjectByType<OptionsController>();
            if (old != null) Object.DestroyImmediate(old);
            var diagnostics = Object.FindFirstObjectByType<PhaseZeroDiagnostics>();
            if (diagnostics != null) Object.DestroyImmediate(diagnostics);
            var oldCanvas = GameObject.Find("Options Canvas");
            if (oldCanvas != null) Object.DestroyImmediate(oldCanvas);
            if (Object.FindFirstObjectByType<EventSystem>() == null)
                new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            var go = new GameObject("Options Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
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
            var panel = Image("Options Panel", safe, new Vector2(440, 566), Vector2.zero, C("101E2B"));
            Image("Top Accent", panel.transform, new Vector2(440, 4), new Vector2(0, 281), C("BAEB71"));
            Image("Bottom Edge", panel.transform, new Vector2(440, 4), new Vector2(0, -281), C("315153"));
            Label("Eyebrow", panel.transform, "PENALTY KING", new Vector2(380, 24), new Vector2(0, 247), 14, C("86A9A6"));
            Label("Title", panel.transform, "OPTIONS", new Vector2(380, 60), new Vector2(0, 180), 42, C("EBF5DF"));
            Label("Subtitle", panel.transform, "SES AYARLARI", new Vector2(380, 24), new Vector2(0, 133), 14, C("86A9A6"));
            var music = MakeSlider(panel.transform, "Music Volume", 58, 0.7f, out var musicText);
            var sfx = MakeSlider(panel.transform, "SFX Volume", -70, 0.8f, out var sfxText);
            var back = Button(panel.transform, "Geri", -185, true, false);
            var backArrow = back.transform.Find("Face/Arrow").GetComponent<Text>();
            backArrow.text = "<";
            backArrow.rectTransform.anchoredPosition = new Vector2(-152, 0);
            var controller = navigator.gameObject.AddComponent<OptionsController>();
            controller.Configure(music, sfx, musicText, sfxText, back);
            EditorUtility.SetDirty(controller);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            Debug.Log("[Phase 2] Options saved with exactly two sliders and Back.");
        }

        private static Slider MakeSlider(Transform parent, string name, float y, float value, out Text percentage)
        {
            var group = Rect(name + " Group", parent, new Vector2(368, 110), new Vector2(0, y));
            var label = Label("Label", group, name, new Vector2(280, 32), new Vector2(-44, 35), 23, C("E3EEE6"));
            label.alignment = TextAnchor.MiddleLeft;
            percentage = Label("Percentage", group, $"{Mathf.RoundToInt(value * 100)}%", new Vector2(78, 32), new Vector2(145, 35), 20, C("BAEB71"));
            var area = Image(name + " Slider", group, new Vector2(368, 60), new Vector2(0, -14), new Color(0, 0, 0, 0));
            area.raycastTarget = true;
            Image("Track", area.transform, new Vector2(344, 12), Vector2.zero, C("304957"));
            var fillArea = Rect("Fill Area", area.transform, new Vector2(344, 12), Vector2.zero);
            var fill = Image("Fill", fillArea, Vector2.zero, Vector2.zero, C("BAEB71"));
            Stretch(fill.rectTransform);
            var handleArea = Rect("Handle Area", area.transform, new Vector2(344, 38), Vector2.zero);
            // Slider stretches the handle vertically; zero sizeDelta avoids adding height twice.
            var handle = Image("Handle", handleArea, new Vector2(24, 0), Vector2.zero, Color.white);
            handle.raycastTarget = true;
            Image("Grip", handle.transform, new Vector2(4, 18), Vector2.zero, C("476139"));
            var slider = area.gameObject.AddComponent<Slider>();
            slider.minValue = 0; slider.maxValue = 1; slider.wholeNumbers = false;
            slider.direction = Slider.Direction.LeftToRight;
            slider.fillRect = fill.rectTransform; slider.handleRect = handle.rectTransform;
            slider.targetGraphic = handle;
            slider.navigation = new Navigation { mode = Navigation.Mode.None };
            var colors = ColorBlock.defaultColorBlock;
            colors.normalColor = C("BAEB71"); colors.highlightedColor = C("D6FFA0");
            colors.pressedColor = C("88B94C"); colors.fadeDuration = 0;
            slider.colors = colors;
            slider.value = value;
            return slider;
        }
    }
}
