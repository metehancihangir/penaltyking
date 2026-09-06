using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace PenaltyKing.Editor
{
    public static class PhaseOneSetup
    {
        private static Color C(string hex) => MenuPixelArt.Hex(hex);

        [MenuItem("Penalty King/Phase 1/Create main menu")]
        public static void CreateMainMenu()
        {
            Directory.CreateDirectory("Assets/UI/Menu");
            var backdrop = MenuPixelArt.Stadium();
            var title = MenuPixelArt.Title();
            var scene = EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
            var navigator = Object.FindFirstObjectByType<SceneNavigator>();
            var previous = Object.FindFirstObjectByType<MainMenuController>();
            if (previous != null) Object.DestroyImmediate(previous);
            var diagnostics = Object.FindFirstObjectByType<PhaseZeroDiagnostics>();
            if (diagnostics != null) Object.DestroyImmediate(diagnostics);
            var previousCanvas = GameObject.Find("Main Menu Canvas");
            if (previousCanvas != null) Object.DestroyImmediate(previousCanvas);
            var eventSystem = Object.FindFirstObjectByType<EventSystem>();
            if (eventSystem == null)
                new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));

            var canvasObject = new GameObject("Main Menu Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = Camera.main;
            canvas.planeDistance = 5;
            canvas.pixelPerfect = true;
            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(960, 720);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            var background = Image("Night Stadium", canvas.transform, Vector2.zero, Vector2.zero, Color.white);
            Stretch(background.rectTransform);
            background.sprite = backdrop;
            var safe = Rect("Safe Area", canvas.transform, Vector2.zero, Vector2.zero);
            Stretch(safe);
            safe.gameObject.AddComponent<SafeAreaPanel>();
            var panel = Image("Menu Panel", safe, new Vector2(440, 566), Vector2.zero, C("101E2B"));
            Image("Top Accent", panel.transform, new Vector2(440, 4), new Vector2(0, 281), C("BAEB71"));
            Image("Bottom Edge", panel.transform, new Vector2(440, 4), new Vector2(0, -281), C("315153"));
            Label("Eyebrow", panel.transform, "FLOODLIGHT FOOTBALL", new Vector2(380, 24), new Vector2(0, 247), 14, C("86A9A6"));
            var logo = Image("Penalty King", panel.transform, new Vector2(301, 140), new Vector2(0, 149), Color.white);
            logo.sprite = title;
            logo.preserveAspect = true;
            Label("Tagline", panel.transform, "ÜÇ YÖN. TEK ŞANS.", new Vector2(380, 28), new Vector2(0, 59), 17, C("AEC4C0"));

            var single = Button(panel.transform, "Singleplayer", 0, true, true);
            var multi = Button(panel.transform, "Multiplayer", -88, false, false);
            var options = Button(panel.transform, "Options", -176, true, false);
            Label("Footer", panel.transform, "PENALTI SENİN.", new Vector2(380, 24), new Vector2(0, -247), 12, C("6F9591"));
            var controller = navigator.gameObject.AddComponent<MainMenuController>();
            controller.Configure(single, multi, options);
            EditorUtility.SetDirty(controller);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            Debug.Log("[Phase 1] MainMenu saved: Singleplayer, disabled Multiplayer, Options.");
        }

        private static PixelMenuButton Button(Transform parent, string text, float y, bool interactable, bool primary)
        {
            var root = Rect(text + " Button", parent, new Vector2(368, 70), new Vector2(0, y));
            Image("Shadow", root, new Vector2(368, 70), new Vector2(0, -5), C("070F1A"));
            var face = Image("Face", root, new Vector2(368, 70), Vector2.zero, Color.white);
            face.raycastTarget = true;
            var button = root.gameObject.AddComponent<PixelMenuButton>();
            button.Face = face.rectTransform;
            button.targetGraphic = face;
            button.transition = Selectable.Transition.ColorTint;
            button.navigation = new Navigation { mode = Navigation.Mode.None };
            var colors = ColorBlock.defaultColorBlock;
            colors.normalColor = C(primary ? "BAEB71" : "274653");
            colors.highlightedColor = C(primary ? "D6FFA0" : "3A6170");
            colors.selectedColor = colors.normalColor;
            colors.pressedColor = C(primary ? "88B94C" : "193640");
            colors.disabledColor = C("1D2E3B");
            colors.fadeDuration = 0;
            button.colors = colors;
            button.interactable = interactable;
            Image("Highlight", face.transform, new Vector2(360, 2), new Vector2(0, 32), C(primary ? "DEFFAD" : "3B5360"));
            Label("Label", face.transform, text, new Vector2(interactable ? 302 : 230, 50),
                new Vector2(interactable ? -10 : -46, 0), 24, C(primary ? "1B3229" : interactable ? "E3EEE6" : "7D939F"));
            if (!interactable)
            {
                var badge = Image("Coming Soon Badge", face.transform, new Vector2(94, 26), new Vector2(121, 0), C("304351"));
                Label("Coming Soon", badge.transform, "YAKINDA", new Vector2(90, 24), Vector2.zero, 12, C("A9BBC2"));
            }
            else Label("Arrow", face.transform, ">", new Vector2(30, 40), new Vector2(152, 0), 26, C(primary ? "385632" : "8BB6B5"));
            return button;
        }

        private static RectTransform Rect(string name, Transform parent, Vector2 size, Vector2 position)
        {
            var go = new GameObject(name, typeof(RectTransform));
            var rect = (RectTransform)go.transform;
            rect.SetParent(parent, false);
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size;
            rect.anchoredPosition = position;
            return rect;
        }

        private static Image Image(string name, Transform parent, Vector2 size, Vector2 position, Color color)
        {
            var image = Rect(name, parent, size, position).gameObject.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            return image;
        }

        private static void Label(string name, Transform parent, string content, Vector2 size, Vector2 position, int fontSize, Color color)
        {
            var text = Rect(name, parent, size, position).gameObject.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.text = content;
            text.fontSize = fontSize;
            text.fontStyle = FontStyle.Bold;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = color;
            text.raycastTarget = false;
        }

        private static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = rect.offsetMax = Vector2.zero;
        }
    }
}
