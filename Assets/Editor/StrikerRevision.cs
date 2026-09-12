using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace PenaltyKing.Editor
{
    public static class StrikerRevision
    {
        [MenuItem("Penalty King/Apply Adult Striker and Wide View (no APK)")]
        public static void Setup()
        {
            const string path="Assets/Sprites/Characters/StrikerAdult.png";
            AssetDatabase.ImportAsset(path,ImportAssetOptions.ForceSynchronousImport);
            var importer=(TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Multiple;
            importer.filterMode=FilterMode.Point;importer.mipmapEnabled=false;importer.isReadable=true;
            importer.textureCompression=TextureImporterCompression.Uncompressed;importer.npotScale=TextureImporterNPOTScale.None;
            importer.maxTextureSize=2048;importer.SaveAndReimport();
            var texture=AssetDatabase.LoadAssetAtPath<Texture2D>(path);var pixels=texture.GetPixels32();
            var frames=new SpriteMetaData[12];Vector2 contact=default;
            // Raised celebration hands extend into the gutter above the last row.
            var rowEdges=new[]{0,texture.height/3,Mathf.RoundToInt(texture.height*.642f),texture.height};
            for(var index=0;index<12;index++)
            {
                int x0=index%4*texture.width/4,x1=(index%4+1)*texture.width/4;
                int y0=texture.height-rowEdges[index/4+1],y1=texture.height-rowEdges[index/4];
                int minX=x1,maxX=x0,minY=y1,maxY=y0;
                for(var y=y0;y<y1;y++)for(var x=x0;x<x1;x++)if(IsPlayer(pixels[y*texture.width+x]))
                {minX=Mathf.Min(minX,x);maxX=Mathf.Max(maxX,x);minY=Mathf.Min(minY,y);maxY=Mathf.Max(maxY,y);}
                if(maxX<=minX||maxY<=minY)throw new System.InvalidOperationException("Empty adult frame "+index);
                var feet=Vector2.zero;var count=0;
                for(var y=minY;y<minY+14;y++)for(var x=minX;x<=maxX;x++)if(IsPlayer(pixels[y*texture.width+x])){feet+=new Vector2(x,y);count++;}
                feet/=Mathf.Max(1,count);var rect=new Rect(minX,minY,maxX-minX+1,maxY-minY+1);
                frames[index]=new SpriteMetaData{name="Adult_"+index.ToString("D2"),rect=rect,alignment=9,pivot=new Vector2((feet.x-minX)/rect.width,(feet.y-minY)/rect.height)};
                if(index==5)
                {
                    var toe=Vector2.zero;var n=0;
                    for(var y=minY;y<minY+rect.height*.5f;y++)for(var x=maxX-20;x<=maxX;x++)if(IsPlayer(pixels[y*texture.width+x])){toe+=new Vector2(x,y);n++;}
                    contact=toe/Mathf.Max(1,n)-feet;
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
        private static bool IsPlayer(Color32 c)=>!(c.r>110&&c.b>85&&c.g<Mathf.Min(c.r,c.b)*.6f);
        public static void Capture()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/Gameplay.unity");
            var game=Object.FindFirstObjectByType<GameplayController>();var kits=game.GetComponent<PlayerKitColours>();
            kits.SetShooterPlayer(1);MenuPreview.Render(1280,720,"ready","striker-wide");kits.UpdateUv();MenuPreview.Render(1280,720,"ready","striker-wide");
            var shot=new ShotResult(ShotDirection.Left,ShotDirection.Right);
            for(var i=0;i<=88;i++)
            {
                if(i*.05f>=ShotPresentation.DurationFor(shot))game.Presentation.ResetPose();else game.Presentation.Sample(shot,i*.05f);
                kits.UpdateUv();MenuPreview.Render(1280,720,"frame-"+i.ToString("D2"),"striker-wide");
            }
            foreach(var direction in new[]{ShotDirection.Left,ShotDirection.Center,ShotDirection.Right})
            {
                game.Presentation.Sample(new ShotResult(direction,direction==ShotDirection.Left?ShotDirection.Right:ShotDirection.Left),ShotPresentation.ImpactTime+2);
                kits.UpdateUv();MenuPreview.Render(1280,720,"goal-"+direction,"striker-wide");
            }
            game.Presentation.ResetPose();kits.SetShooterPlayer(2);MenuPreview.Render(2340,1080,"wide","striker-wide");kits.UpdateUv();MenuPreview.Render(2340,1080,"wide","striker-wide");
            EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
        }
    }
}
