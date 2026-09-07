using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using static PenaltyKing.Editor.PhaseOneSetup;
namespace PenaltyKing.Editor
{
    public static class UpdatePhaseThreeSetup
    {
        [MenuItem("Penalty King/Updates/Phase 3 - Build match controls")]
        public static void Build()
        {
            var scene = EditorSceneManager.OpenScene("Assets/Scenes/Gameplay.unity");
            var game = Object.FindFirstObjectByType<GameplayController>();
            var canvas = GameObject.Find("Gameplay Canvas").transform;
            var previous = canvas.Find("Match Controls");
            if (previous != null) Object.DestroyImmediate(previous.gameObject);
            var safe = Rect("Match Controls", canvas, Vector2.zero, Vector2.zero);
            Stretch(safe); safe.gameObject.AddComponent<SafeAreaPanel>();
            var root = Rect("Menu Button", safe, new Vector2(104, 64), Vector2.zero);
            root.anchorMin = root.anchorMax = root.pivot = new Vector2(0,1);
            root.anchoredPosition = new Vector2(12,-12);
            var face = Image("Face", root, new Vector2(104,64), Vector2.zero, Color.white);
            face.raycastTarget = true;
            var exit = root.gameObject.AddComponent<PixelMenuButton>();
            exit.Face = face.rectTransform; exit.targetGraphic = face;
            exit.navigation = new Navigation { mode = Navigation.Mode.None };
            var colors = ColorBlock.defaultColorBlock;
            colors.normalColor = MenuPixelArt.Hex("274653"); colors.highlightedColor = MenuPixelArt.Hex("3A6170");
            colors.pressedColor = MenuPixelArt.Hex("193640"); colors.selectedColor = colors.normalColor; colors.fadeDuration = 0;
            exit.colors = colors;
            Label("Label", face.transform, "Menü", new Vector2(100,50), Vector2.zero, 22, MenuPixelArt.Hex("E3EEE6"));
            var summary = canvas.Find("Safe Area/Round Summary").GetComponent<RectTransform>();
            summary.sizeDelta = new Vector2(408,430);
            summary.Find("Title").GetComponent<RectTransform>().anchoredPosition = new Vector2(0,170);
            summary.Find("Title").GetComponent<Text>().fontSize = 25;
            summary.Find("Final Score").GetComponent<RectTransform>().anchoredPosition = new Vector2(0,100);
            foreach (var name in new[] { "Player 1", "Player 2" })
            { var old = summary.Find(name); if (old != null) Object.DestroyImmediate(old.gameObject); }
            var first = Label("Player 1", summary, "PLAYER 1\n5 / 5 şut", new Vector2(184,64), new Vector2(-96,32), 19, MenuPixelArt.Hex("AEC4C0"));
            var second = Label("Player 2", summary, "PLAYER 2\n5 / 5 şut", new Vector2(184,64), new Vector2(96,32), 19, MenuPixelArt.Hex("AEC4C0"));
            ((RectTransform)game.Replay.transform).anchoredPosition = new Vector2(0,-65);
            ((RectTransform)game.Home.transform).anchoredPosition = new Vector2(0,-157);
            game.ConfigureMatchUi(exit, first, second);
            EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene); AssetDatabase.SaveAssets();
            summary.gameObject.SetActive(true);
            game.ResultTitle.text = "PLAYER 2 KAZANDI"; game.ResultScore.text = "2  -  4";
            MenuPreview.Render(390,844,"portrait","update3-result");
            MenuPreview.Render(1280,720,"landscape","update3-result");
            summary.gameObject.SetActive(false);
            game.Handoff.Show(2,true,null);
            MenuPreview.Render(390,844,"portrait","update3-exit");
            EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
            Debug.Log("[Update Phase 3] Result panel and persistent menu exit saved.");
        }
    }
}
