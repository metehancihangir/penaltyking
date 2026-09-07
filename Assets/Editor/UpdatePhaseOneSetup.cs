using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using static PenaltyKing.Editor.PhaseOneSetup;

namespace PenaltyKing.Editor
{
    public static class UpdatePhaseOneSetup
    {
        [MenuItem("Penalty King/Updates/Phase 1 - Build menus")]
        public static void Build()
        {
            PhaseOneSetup.CreateMainMenu();
            var main = Object.FindFirstObjectByType<MainMenuController>();
            var panel = GameObject.Find("Menu Panel").transform;
            Remove(panel, "Eyebrow", "Tagline", "Footer");
            Object.DestroyImmediate(main.Multiplayer.gameObject);
            var play = main.Play;
            play.name = "Play Button";
            CenterButton(play, "Play", -30);
            CenterButton(main.Options, "Options", -130);
            main.Configure(play, null, main.Options);
            panel.Find("Penalty King").GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 137);
            Save();

            PhaseTwoSetup.CreateOptions();
            panel = GameObject.Find("Options Panel").transform;
            Remove(panel, "Eyebrow", "Subtitle");
            panel.Find("Title").GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 208);
            panel.Find("Music Volume Group").GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 100);
            panel.Find("SFX Volume Group").GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -15);
            var options = Object.FindFirstObjectByType<OptionsController>();
            CenterButton(options.Back, "Geri", -214);
            var area = Image("Vibration Toggle", panel, new Vector2(368, 64), new Vector2(0, -120), MenuPixelArt.Hex("274653"));
            area.raycastTarget = true;
            var toggle = area.gameObject.AddComponent<Toggle>();
            toggle.targetGraphic = area;
            toggle.navigation = new Navigation { mode = Navigation.Mode.None };
            Label("Label", area.transform, "Titreşim", new Vector2(250, 44), new Vector2(-40, 0), 23, MenuPixelArt.Hex("E3EEE6"));
            var box = Image("Box", area.transform, new Vector2(36, 36), new Vector2(140, 0), MenuPixelArt.Hex("101E2B"));
            var check = Image("Enabled", box.transform, new Vector2(22, 22), Vector2.zero, MenuPixelArt.Hex("BAEB71"));
            toggle.graphic = check;
            toggle.isOn = true;
            options.ConfigureVibration(toggle);
            Save();

            PhaseThreeSetup.CreateMode();
            EditorSceneManager.OpenScene("Assets/Scenes/ModeSelect.unity");
            panel = GameObject.Find("Selection Panel").transform;
            Remove(panel, "Step", "Selected Difficulty", "Round Description", "Endless Description");
            var modes = Object.FindFirstObjectByType<ModeSelectController>();
            CenterButton(modes.FixedRound, "Sabit Round", 40);
            CenterButton(modes.Endless, "Endless", -60);
            CenterButton(modes.Back, "Geri", -214);
            modes.Configure(modes.FixedRound, modes.Endless, modes.Back, null, null);
            Save();

            // Reuse the common camera/input shell, then replace all difficulty content.
            EditorSceneManager.OpenScene("Assets/Scenes/DifficultySelect.unity");
            Object.DestroyImmediate(Object.FindFirstObjectByType<DifficultySelectController>());
            panel = GameObject.Find("Selection Panel").transform;
            for (var i = panel.childCount - 1; i >= 0; i--)
            {
                var child = panel.GetChild(i);
                if (child.name != "Top Accent" && child.name != "Bottom Edge") Object.DestroyImmediate(child.gameObject);
            }
            Label("Title", panel, "OYUN SEÇ", new Vector2(390, 60), new Vector2(0, 185), 36, MenuPixelArt.Hex("EBF5DF"));
            var online = Button(panel, "Online", 58, false, false);
            CenterButton(online, "Online", 58);
            var local = Button(panel, "2 Kişilik", -54, true, true);
            CenterButton(local, "2 Kişilik", -54);
            local.transform.Find("Face/Label").GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 11);
            Label("Description", local.Face, "Tek Telefon", new Vector2(240, 22), new Vector2(0, -20), 14, MenuPixelArt.Hex("385632"));
            DrawIcon(online.Face, new[] { "001111100", "010010010", "100010001", "100010001", "111111111", "100010001", "100010001", "010010010", "001111100" }, MenuPixelArt.Hex("7D939F"));
            DrawIcon(local.Face, new[] { "011000110", "011000110", "000000000", "111101111", "111101111", "111101111" }, MenuPixelArt.Hex("385632"));
            var back = Button(panel, "Geri", -214, true, false);
            CenterButton(back, "Geri", -214);
            var selection = Object.FindFirstObjectByType<SceneNavigator>().gameObject.AddComponent<PlayerSelectController>();
            selection.Configure(online, local, back);
            EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(), "Assets/Scenes/PlayerSelect.unity");
            // Keep the legacy scene asset for the phase 2 migration, but exclude it from builds.
            EditorBuildSettings.scenes = new[] { "MainMenu", "PlayerSelect", "ModeSelect", "Gameplay", "Options" }
                .Select(name => new EditorBuildSettingsScene($"Assets/Scenes/{name}.unity", true)).ToArray();
            AssetDatabase.SaveAssets();
            Capture();
            EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
            Debug.Log("[Update Phase 1] Menus, local selection and vibration preference ready.");
        }

        private static void DrawIcon(Transform parent, string[] rows, Color color)
        {
            var icon = Rect("Pixel Icon", parent, new Vector2(40, 40), new Vector2(-143, 0));
            for (var y = 0; y < rows.Length; y++)
                for (var x = 0; x < rows[y].Length; x++)
                    if (rows[y][x] == '1') Image("Pixel", icon, new Vector2(4, 4), new Vector2((x - rows[y].Length / 2f) * 4, (rows.Length / 2f - y) * 4), color);
        }

        private static void CenterButton(PixelMenuButton button, string text, float y)
        {
            Remove(button.Face, "Arrow", "Coming Soon Badge");
            var label = button.Face.Find("Label").GetComponent<Text>();
            label.text = text;
            label.rectTransform.anchoredPosition = Vector2.zero;
            label.rectTransform.sizeDelta = new Vector2(280, 50);
            ((RectTransform)button.transform).anchoredPosition = new Vector2(0, y);
        }

        private static void Remove(Transform parent, params string[] names)
        { foreach (var name in names) { var child = parent.Find(name); if (child != null) Object.DestroyImmediate(child.gameObject); } }

        private static void Save() => EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());

        public static void Capture()
        {
            foreach (var name in new[] { "MainMenu", "PlayerSelect", "Options", "ModeSelect" })
            {
                EditorSceneManager.OpenScene($"Assets/Scenes/{name}.unity");
                MenuPreview.Render(1280, 720, "landscape", "update1-" + name);
                MenuPreview.Render(390, 844, "portrait", "update1-" + name);
            }
        }
    }
}
