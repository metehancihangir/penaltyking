using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace PenaltyKing.Editor
{
    public static class ActionRevision
    {
        [MenuItem("Penalty King/Apply Action Revision (no APK)")]
        public static void Setup()
        {
            const string path = "Assets/Sprites/Characters/ShooterPerformance.png";
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType = TextureImporterType.Sprite; importer.spriteImportMode = SpriteImportMode.Multiple;
            importer.filterMode = FilterMode.Point; importer.mipmapEnabled = false; importer.isReadable = true;
            importer.textureCompression = TextureImporterCompression.Uncompressed; importer.npotScale = TextureImporterNPOTScale.None;
            importer.maxTextureSize = 2048; importer.SaveAndReimport();
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path); var pixels = texture.GetPixels32();
            var frames = new SpriteMetaData[8]; var boundaries = new[] { 0, 384, 768, 1164, 1536 };
            Vector2 contact = default;
            for (var index=0; index<8; index++)
            {
                var x0=boundaries[index%4]; var x1=boundaries[index%4+1];
                var y0=index<4?524:0; var y1=index<4?1024:524;
                var minX=x1;var maxX=x0;var minY=y1;var maxY=y0;
                for(var y=y0;y<y1;y++) for(var x=x0;x<x1;x++) if(IsPlayer(pixels[y*texture.width+x]))
                { minX=Mathf.Min(minX,x);maxX=Mathf.Max(maxX,x);minY=Mathf.Min(minY,y);maxY=Mathf.Max(maxY,y); }
                if(maxX<=minX || maxY<=minY) throw new System.InvalidOperationException("Empty player frame "+index);
                var feet=Vector2.zero;var count=0;
                for(var y=minY;y<minY+18;y++)for(var x=minX;x<=maxX;x++)if(IsPlayer(pixels[y*texture.width+x]))
                {feet+=new Vector2(x,y);count++;}
                feet/=Mathf.Max(1,count);
                var rect=new Rect(minX,minY,maxX-minX+1,maxY-minY+1);
                frames[index]=new SpriteMetaData{name="Performance_"+index,rect=rect,alignment=9,pivot=new Vector2((feet.x-minX)/rect.width,(feet.y-minY)/rect.height)};
                if(index==2)
                {
                    var toe=Vector2.zero;var toeCount=0;
                    for(var y=minY;y<minY+rect.height*.5f;y++)for(var x=maxX-24;x<=maxX;x++)if(IsPlayer(pixels[y*texture.width+x]))
                    {toe+=new Vector2(x,y);toeCount++;}
                    contact=toe/Mathf.Max(1,toeCount)-feet;
                }
            }
#pragma warning disable 618
            importer.spritesheet=frames;
#pragma warning restore 618
            importer.isReadable=false;importer.SaveAndReimport();
            var sprites=AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().OrderBy(s=>s.name).ToArray();
            var scene=EditorSceneManager.OpenScene("Assets/Scenes/Gameplay.unity");
            Object.FindFirstObjectByType<ShotPresentation>().ConfigurePerformance(sprites,contact);
            EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();
            Capture();
        }
        private static bool IsPlayer(Color32 c) => !(c.r>110 && c.b>85 && c.g<Mathf.Min(c.r,c.b)*.6f);
        public static void Capture()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/Gameplay.unity");
            var game=Object.FindFirstObjectByType<GameplayController>();var kits=game.GetComponent<PlayerKitColours>();
            kits.SetShooterPlayer(1);MenuPreview.Render(1280,720,"ready","action-revision");
            kits.UpdateUv();MenuPreview.Render(1280,720,"ready","action-revision");
            var shot=new ShotResult(ShotDirection.Right,ShotDirection.Left);
            for(var i=0;i<=44;i++)
            {
                if(i*.1f>=ShotPresentation.DurationFor(shot))game.Presentation.ResetPose();
                else game.Presentation.Sample(shot,i*.1f);
                kits.UpdateUv();
                MenuPreview.Render(1280,720,"frame-"+i.ToString("D2"),"action-revision");
            }
            game.Presentation.ResetPose();kits.SetShooterPlayer(2);
            game.Presentation.Sample(shot,ShotPresentation.ImpactTime+1);kits.UpdateUv();MenuPreview.Render(1280,720,"player2","action-revision");
            EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
        }
    }
}
