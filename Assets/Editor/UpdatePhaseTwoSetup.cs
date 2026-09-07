using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using static PenaltyKing.Editor.PhaseOneSetup;
namespace PenaltyKing.Editor
{
    public static class UpdatePhaseTwoSetup
    {
        [MenuItem("Penalty King/Updates/Phase 2 - Build turn handoff")]
        public static void Build()
        {
            var scene = EditorSceneManager.OpenScene("Assets/Scenes/Gameplay.unity");
            var game = Object.FindFirstObjectByType<GameplayController>();
            var canvas = GameObject.Find("Gameplay Canvas").transform;
            var previous = canvas.Find("Turn Handoff");
            if (previous != null) Object.DestroyImmediate(previous.gameObject);
            var shade = Image("Turn Handoff", canvas, Vector2.zero, Vector2.zero, MenuPixelArt.Hex("0B1624"));
            Stretch(shade.rectTransform);
            shade.raycastTarget = true;
            var safe = Rect("Safe Area", shade.transform, Vector2.zero, Vector2.zero);
            Stretch(safe); safe.gameObject.AddComponent<SafeAreaPanel>();
            var card = Image("Card", safe, new Vector2(420, 310), Vector2.zero, MenuPixelArt.Hex("101E2B"));
            Image("Accent", card.transform, new Vector2(420, 4), new Vector2(0, 153), MenuPixelArt.Hex("BAEB71"));
            var title = Label("Player", card.transform, "Sıra PLAYER 1'de", new Vector2(396, 48), new Vector2(0, 90), 29, MenuPixelArt.Hex("BAEB71"));
            var role = Label("Role", card.transform, "Şut sırası", new Vector2(380, 84), new Vector2(0, 5), 25, MenuPixelArt.Hex("E3EEE6"));
            Label("Continue", card.transform, "Devam etmek için dokun", new Vector2(380, 35), new Vector2(0, -105), 19, MenuPixelArt.Hex("AEC4C0"));
            var overlay = shade.gameObject.AddComponent<TurnHandoff>();
            overlay.Configure(title, role, card.rectTransform);
            game.ConfigureHandoff(overlay);
            shade.gameObject.SetActive(false);
            foreach (var zone in new[] { game.Left, game.Center, game.Right })
            {
                var colors = zone.colors;
                colors.normalColor = colors.highlightedColor = colors.selectedColor = colors.pressedColor = colors.disabledColor = new Color(1,1,1,0);
                zone.colors = colors;
            }
            GameObject.Find("Gameplay Canvas/Safe Area/Mode").GetComponent<Text>().text = "SABİT ROUND";
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            overlay.Show(2, true, null);
            MenuPreview.Render(390, 844, "portrait", "update2-handoff");
            MenuPreview.Render(1280, 720, "landscape", "update2-handoff");
            EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
            Debug.Log("[Update Phase 2] Turn handoff and private input zones saved.");
        }
    }
}
