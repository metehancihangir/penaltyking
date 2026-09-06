using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using static PenaltyKing.Editor.PhaseOneSetup;

namespace PenaltyKing.Editor
{
    public static class PhaseSixSetup
    {
        [MenuItem("Penalty King/Phase 6/Create animated gameplay")]
        public static void CreateGameplay()
        {
            PhaseFiveSetup.CreateGameplay();
            var playerFrames = ImportSheet("ShooterActions");
            var sideFrames = ImportSheet("KeeperSide");
            var centerFrames = ImportSheet("KeeperCenter");
            var layout = Object.FindFirstObjectByType<GameplayStageLayout>();
            var stage = (RectTransform)layout.transform;
            var keeper = stage.Find("06 Keeper").GetComponent<Image>();
            var shooter = stage.Find("07 Shooter").GetComponent<Image>();
            var ball = stage.Find("08 Ball").GetComponent<Image>();
            var crowd = (RectTransform)stage.Find("03 Crowd");
            var effects = Rect("Ball Effects", stage, Vector2.zero, Vector2.zero);
            effects.SetSiblingIndex(ball.transform.GetSiblingIndex());
            var shadow = Image("Ball Shadow", effects, new Vector2(27, 8), Vector2.zero, new Color(0, .05f, .03f, .32f));
            shadow.sprite = ball.sprite;
            var trails = new Image[3];
            for (var i = 0; i < trails.Length; i++)
            { trails[i] = Image("Ball Trail " + i, effects, new Vector2(27, 27), Vector2.zero, Color.white); trails[i].sprite = ball.sprite; trails[i].enabled = false; }
            var dust = new Image[5];
            for (var i = 0; i < dust.Length; i++) { dust[i] = Image("Kick Dust " + i, effects, new Vector2(4, 3), Vector2.zero, Color.white); dust[i].enabled = false; }
            var particles = Rect("Crowd Celebration", stage, Vector2.zero, Vector2.zero);
            particles.SetSiblingIndex(4);
            var confetti = new Image[20];
            for (var i = 0; i < confetti.Length; i++)
            {
                confetti[i] = Image("Confetti " + i, particles, new Vector2(3, 6), Vector2.zero, i % 2 == 0 ? new Color32(255, 216, 71, 255) : new Color32(111, 221, 232, 255));
                confetti[i].enabled = false;
            }
            var game = Object.FindFirstObjectByType<GameplayController>();
            var animation = game.gameObject.AddComponent<ShotPresentation>();
            animation.Configure(ball, keeper, shooter, stage, crowd, shadow, trails, dust, confetti, playerFrames, sideFrames, centerFrames);
            game.ConfigurePresentation(animation);
            game.ConfigureComposition(ball.rectTransform.anchoredPosition, keeper.rectTransform.anchoredPosition, 190, keeper.rectTransform.anchoredPosition.y + 21, .55f);
            EditorUtility.SetDirty(game); EditorUtility.SetDirty(animation);
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene()); AssetDatabase.SaveAssets();
            Debug.Log("[Phase 6] Player, three keeper directions, ball trajectory, crowd and sound callbacks configured.");
        }

        public static void BuildAndPreview() { CreateGameplay(); MenuPreview.CaptureFinalArt(); MenuPreview.CaptureAnimations(); }

        private static Sprite[] ImportSheet(string name)
        {
            var path = "Assets/Sprites/Characters/" + name + ".png";
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType = TextureImporterType.Sprite; importer.spriteImportMode = SpriteImportMode.Multiple;
            importer.filterMode = FilterMode.Point; importer.mipmapEnabled = false;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.maxTextureSize = 4096; importer.npotScale = TextureImporterNPOTScale.None;
            importer.alphaIsTransparency = true; importer.isReadable = true; importer.SaveAndReimport();
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            var pixels = texture.GetPixels32(); var width = texture.width; var height = texture.height;
            var frames = new SpriteMetaData[4];
            for (var index = 0; index < 4; index++)
            {
                var x0 = index % 2 * (width / 2); var x1 = x0 + width / 2;
                var y0 = (1 - index / 2) * (height / 2); var y1 = y0 + height / 2;
                // The wide side-dive atlas has more horizontal room for the prone frame.
                if (name == "KeeperSide")
                {
                    x0 = index == 1 ? 720 : index == 3 ? 900 : 0;
                    x1 = index == 0 ? 700 : index == 2 ? 900 : width;
                }
                var minX = x1; var maxX = x0; var minY = y1; var maxY = y0;
                for (var y = y0; y < y1; y++) for (var x = x0; x < x1; x++)
                    if (pixels[y * width + x].a > 200)
                    { minX = Mathf.Min(x, minX); maxX = Mathf.Max(x, maxX); minY = Mathf.Min(y, minY); maxY = Mathf.Max(y, maxY); }
                if (maxX <= minX || maxY <= minY) throw new System.InvalidOperationException("Empty animation cell: " + name + index);
                frames[index] = new SpriteMetaData { name = name + "_" + index, rect = new Rect(minX, minY, maxX-minX+1, maxY-minY+1), alignment = 9, pivot = new Vector2(.5f, .5f) };
            }
#pragma warning disable 618
            importer.spritesheet = frames;
#pragma warning restore 618
            importer.isReadable = false; importer.SaveAndReimport();
            return AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().OrderBy(sprite => sprite.name).ToArray();
        }
    }
}
