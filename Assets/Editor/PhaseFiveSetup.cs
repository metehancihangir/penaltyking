using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using static PenaltyKing.Editor.PhaseOneSetup;

namespace PenaltyKing.Editor
{
    public static class PhaseFiveSetup
    {
        private static Color C(string hex) => MenuPixelArt.Hex(hex);
        private const string Root = "Assets/Sprites/";

        [MenuItem("Penalty King/Phase 5/Create final gameplay art")]
        public static void CreateGameplay()
        {
            Import("Stadium/NightStadium", false);
            foreach (var path in new[] { "Props/Goal", "Props/Ball", "Characters/KeeperIdle", "Characters/ShooterIdle", "Stadium/CrowdIdle" }) Import(path, true);
            var scene = EditorSceneManager.OpenScene("Assets/Scenes/Gameplay.unity");
            var existing = Object.FindFirstObjectByType<GameplayController>();
            if (existing != null) Object.DestroyImmediate(existing);
            var root = GameObject.Find("Gameplay Canvas");
            Object.DestroyImmediate(root);
            root = new GameObject("Gameplay Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = root.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera; canvas.worldCamera = Camera.main;
            canvas.planeDistance = 5; canvas.pixelPerfect = true;
            Camera.main.orthographic = true;
            var scaler = root.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(960, 720); scaler.matchWidthOrHeight = .5f;
            Stretch(Image("Night Background", root.transform, Vector2.zero, Vector2.zero, C("071222")).rectTransform);
            var safe = Rect("Safe Area", root.transform, Vector2.zero, Vector2.zero);
            Stretch(safe); safe.gameObject.AddComponent<SafeAreaPanel>();
            var stage = Rect("Stadium Stage", safe, new Vector2(960, 640), Vector2.zero);
            stage.gameObject.AddComponent<GameplayStageLayout>();
            var sky = Layer(stage, "01 Sky and Floodlights", 0, 210);
            var stands = Layer(stage, "02 Stands", 210, 285);
            var crowd = Rect("03 Crowd", stage, new Vector2(960, 40), new Vector2(0, 57));
            for (var i = 0; i < 8; i++) Art("Crowd " + i, crowd, "Stadium/CrowdIdle", new Vector2(120, 32), new Vector2(-420 + i * 120, 0));
            var pitch = Layer(stage, "04 Pitch", 495, 529);
            var goal = Art("05 Goal and Net", stage, "Props/Goal", new Vector2(600, 216), new Vector2(0, 94));
            var keeper = Art("06 Keeper", stage, "Characters/KeeperIdle", new Vector2(76, 114), new Vector2(0, 37));
            var shooter = Art("07 Shooter", stage, "Characters/ShooterIdle", new Vector2(91, 160), new Vector2(-100, -224));
            var ball = Art("08 Ball", stage, "Props/Ball", new Vector2(27, 27), new Vector2(0, -213));
            var aim = Rect("09 Touch Zones", stage, new Vector2(600, 190), Vector2.zero);
            var left = Zone(aim, "SOL", -190);
            var center = Zone(aim, "ORTA", 0);
            var right = Zone(aim, "SAĞ", 190);
            var mode = Hud(safe, "Mode", "SABİT ROUND · Orta", new Vector2(400, 24), new Vector2(0, -22), 16, true, C("A9BECD"));
            var score = Hud(safe, "Score", "GOL  0", new Vector2(180, 36), new Vector2(-100, -53), 26, true, C("D2F0A0"));
            var shots = Hud(safe, "Shots", "ŞUT  0 / 5", new Vector2(190, 36), new Vector2(100, -53), 23, true, C("EFF4E9"));
            var feedback = Hud(safe, "Feedback", "BİR YÖNE DOKUN", new Vector2(400, 36), new Vector2(0, 52), 24, false, C("EFF4E9"));
            var directions = Hud(safe, "Directions", "Sol, orta veya sağ.", new Vector2(410, 26), new Vector2(0, 22), 16, false, C("A9BECD"));
            var summary = Image("Round Summary", safe, new Vector2(408, 380), Vector2.zero, C("101E2B"));
            summary.raycastTarget = true;
            var title = Label("Title", summary.transform, "ROUND TAMAMLANDI", new Vector2(390, 44), new Vector2(0, 135), 26, C("EBF5DF"));
            var result = Label("Final Score", summary.transform, "0 / 5 GOL", new Vector2(380, 50), new Vector2(0, 67), 30, C("BAEB71"));
            var replay = Button(summary.transform, "Tekrar Oyna", -33, true, true);
            var home = Button(summary.transform, "Ana Menüye Dön", -125, true, false);
            summary.gameObject.SetActive(false);
            var controller = Object.FindFirstObjectByType<SceneNavigator>().gameObject.AddComponent<GameplayController>();
            controller.Configure(left, center, right, score, shots, mode, feedback, directions, ball.rectTransform, keeper.rectTransform, summary.gameObject, title, result, replay, home);
            controller.ConfigureComposition(new Vector2(0, -213), new Vector2(0, 37), 190, 58, .55f);
            stage.GetComponent<GameplayStageLayout>().Configure(sky, stands, crowd, pitch, goal.rectTransform, shooter.rectTransform, aim, controller);
            EditorUtility.SetDirty(controller);
            Canvas.ForceUpdateCanvases(); stage.GetComponent<GameplayStageLayout>().Fit();
            EditorSceneManager.SaveScene(scene); AssetDatabase.SaveAssets();
            Debug.Log("[Phase 5] Final stadium layers and character sprites saved.");
        }

        public static void BuildAndPreview() { CreateGameplay(); MenuPreview.CaptureFinalArt(); }

        private static void Import(string name, bool trim)
        {
            var path = Root + name + ".png";
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType = TextureImporterType.Sprite;
            importer.filterMode = FilterMode.Point; importer.mipmapEnabled = false;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.maxTextureSize = 4096; importer.alphaIsTransparency = true;
            importer.npotScale = TextureImporterNPOTScale.None;
            importer.spritePixelsPerUnit = 100;
            importer.isReadable = trim; importer.SaveAndReimport();
            if (trim)
            {
                var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                var pixels = texture.GetPixels32();
                var minX = texture.width; var minY = texture.height; var maxX = 0; var maxY = 0;
                for (var y = 0; y < texture.height; y++)
                    for (var x = 0; x < texture.width; x++)
                        if (pixels[y * texture.width + x].a > 200)
                        { minX = Mathf.Min(minX, x); maxX = Mathf.Max(maxX, x); minY = Mathf.Min(minY, y); maxY = Mathf.Max(maxY, y); }
                importer.spriteImportMode = SpriteImportMode.Multiple;
#pragma warning disable 618
                importer.spritesheet = new[] { new SpriteMetaData { name = System.IO.Path.GetFileName(name), rect = new Rect(minX, minY, maxX-minX+1, maxY-minY+1), alignment = 9, pivot = new Vector2(.5f, .5f) } };
#pragma warning restore 618
                importer.isReadable = false; importer.SaveAndReimport();
            }
        }

        private static Image Art(string name, Transform parent, string asset, Vector2 size, Vector2 position)
        {
            var image = Image(name, parent, size, position, Color.white);
            image.sprite = AssetDatabase.LoadAllAssetsAtPath(Root + asset + ".png").OfType<Sprite>().First();
            image.preserveAspect = true;
            return image;
        }

        private static RectTransform Layer(Transform parent, string name, int top, int height)
        {
            var rect = Rect(name, parent, new Vector2(960, height * .625f), new Vector2(0, (512 - top - height * .5f) * .625f));
            var image = rect.gameObject.AddComponent<RawImage>();
            image.texture = AssetDatabase.LoadAssetAtPath<Texture2D>(Root + "Stadium/NightStadium.png");
            image.uvRect = new Rect(0, (1024 - top - height) / 1024f, 1, height / 1024f);
            image.raycastTarget = false;
            return rect;
        }

        private static Text Hud(Transform parent, string name, string value, Vector2 size, Vector2 position, int fontSize, bool top, Color color)
        {
            var label = Label(name, parent, value, size, position, fontSize, color);
            label.rectTransform.anchorMin = label.rectTransform.anchorMax = new Vector2(.5f, top ? 1 : 0);
            return label;
        }

        private static ShotZone Zone(Transform parent, string title, float x)
        {
            var image = Image(title + " Shot Zone", parent, new Vector2(186, 174), new Vector2(x, 87), Color.white);
            image.raycastTarget = true;
            var zone = image.gameObject.AddComponent<ShotZone>(); zone.targetGraphic = image;
            zone.navigation = new Navigation { mode = Navigation.Mode.None };
            var colors = ColorBlock.defaultColorBlock;
            colors.normalColor = new Color(1, 1, 1, 0); colors.highlightedColor = new Color(.8f, 1, .6f, .15f);
            colors.pressedColor = new Color(.8f, 1, .6f, .3f); colors.disabledColor = new Color(1, 1, 1, 0); colors.fadeDuration = 0;
            zone.colors = colors;
            var label = Label("Direction", image.transform, title, new Vector2(100, 26), new Vector2(0, -104), 15, C("F4F5D9"));
            label.gameObject.AddComponent<Outline>().effectColor = C("102220");
            return zone;
        }
    }
}
