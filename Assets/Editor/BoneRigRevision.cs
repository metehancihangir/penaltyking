using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using static PenaltyKing.Editor.PhaseOneSetup;
namespace PenaltyKing.Editor
{
    public static class BoneRigRevision
    {
        [MenuItem("Penalty King/Apply Continuous Bone Animation (no APK)")]
        public static void Setup()
        {
            var player=Import("StrikerRig");var goalkeeper=Import("KeeperRig");
            var scene=EditorSceneManager.OpenScene("Assets/Scenes/Gameplay.unity");
            var stage=Object.FindFirstObjectByType<GameplayStageLayout>().transform;
            var striker=Build(stage.Find("07 Shooter").GetComponent<Image>(),player,false);
            var goalie=Build(stage.Find("06 Keeper").GetComponent<Image>(),goalkeeper,true);
            var game=Object.FindFirstObjectByType<GameplayController>();game.Presentation.ConfigureRigs(striker,goalie);
            foreach(var zone in game.Targets)
            {
                foreach(var marker in zone.GetComponentsInChildren<TargetReticle>(true))Object.DestroyImmediate(marker.gameObject);
                var colors=zone.colors;colors.normalColor=colors.highlightedColor=colors.pressedColor=colors.selectedColor=colors.disabledColor=Color.clear;zone.colors=colors;
            }
            RebuildCrowd(stage);
            var layout=stage.GetComponent<GameplayStageLayout>();layout.enabled=false;layout.enabled=true;
            game.Presentation.ResetPose();
            EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();
            Capture();
        }
        private static FootballRig Build(Image image,Sprite[] sprites,bool keeper)
        {
            var old=image.GetComponent<FootballRig>();
            if(old!=null){foreach(Transform child in image.transform.Cast<Transform>().ToArray())Object.DestroyImmediate(child.gameObject);Object.DestroyImmediate(old);}
            image.enabled=false;var rig=image.gameObject.AddComponent<FootballRig>();
            var boneNames=new[]{"Hips","Chest","Head","Left Shoulder","Left Elbow","Left Hand","Right Shoulder","Right Elbow","Right Hand","Left Hip","Left Knee","Left Ankle","Right Hip","Right Knee","Right Ankle"};
            var parents=new[]{-1,16,1,1,3,4,1,6,7,0,9,10,0,12,13};var bones=new RectTransform[17];var art=new Image[15];
            var mapping=new[]{2,1,0,3,4,5,3,4,5,6,7,8,6,7,8};
            bones[0]=Rect("Hips",image.transform,Vector2.zero,Vector2.zero);
            bones[15]=Rect("Lower Spine",bones[0],Vector2.zero,Vector2.zero);
            bones[16]=Rect("Upper Spine",bones[15],Vector2.zero,Vector2.zero);
            for(var i=1;i<15;i++)bones[i]=Rect(boneNames[i],bones[parents[i]],Vector2.zero,Vector2.zero);
            for(var i=0;i<15;i++)
            {
                art[i]=Image("Art "+boneNames[i],image.transform,new Vector2(20,30),Vector2.zero,Color.white);
                art[i].sprite=sprites[mapping[i]];art[i].preserveAspect=false;
                if(i==3||i==4||i==5||i==9||i==10||i==11)art[i].rectTransform.localScale=new Vector3(-1,1,1);
            }
            // Legs and upper arms behind shirt/shorts; the held football is behind both gloves.
            foreach(var i in new[]{9,10,11,12,13,14,3,4,6,7,0,1,2})art[i].transform.SetAsLastSibling();
            var waist=Image("Waist Overlap",image.transform,new Vector2(43,29),Vector2.zero,Color.white);
            waist.sprite=sprites[9];waist.raycastTarget=false;
            waist.transform.SetSiblingIndex(art[1].transform.GetSiblingIndex());
            var ball=Image("Held Football",image.transform,new Vector2(24,24),Vector2.zero,Color.white);ball.gameObject.SetActive(false);
            art[5].transform.SetAsLastSibling();art[8].transform.SetAsLastSibling();
            rig.Configure(art,bones,keeper);rig.ConfigureHeldBall(ball);rig.ConfigureWaist(waist);
            BuildSkin(rig,image,bones,art,waist,ball,keeper);return rig;
        }
        private static void BuildSkin(FootballRig rig,Image original,RectTransform[] bones,Image[] art,Image waist,Image held,bool keeper)
        {
            var path="Assets/Sprites/Characters/"+(keeper?"KeeperSix":"StrikerShot12")+".png";
            var sprite=AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().First(s=>s.name==(keeper?"KeeperSix_07":"StrikerShot12_00"));
            // Joint landmarks measured on the intact drawings (top-left texture pixels).
            var source=keeper?new[]{new Vector2(1552,686),new Vector2(1558,627),new Vector2(1560,575),
                new Vector2(1516,623),new Vector2(1486,663),new Vector2(1443,692),new Vector2(1598,623),new Vector2(1634,663),new Vector2(1671,692),
                new Vector2(1531,699),new Vector2(1498,747),new Vector2(1458,803),new Vector2(1582,699),new Vector2(1619,747),new Vector2(1654,803)}
                :new[]{new Vector2(151,220),new Vector2(157,111),new Vector2(169,60),
                new Vector2(106,119),new Vector2(84,171),new Vector2(79,219),new Vector2(197,122),new Vector2(214,168),new Vector2(226,218),
                new Vector2(121,234),new Vector2(95,295),new Vector2(83,380),new Vector2(179,235),new Vector2(198,302),new Vector2(214,380)};
            var origin=new Vector2(source[0].x,sprite.texture.height-source[0].y);
            var points=source.Select(p=>(new Vector2(p.x,sprite.texture.height-p.y)-origin)*(210/sprite.rect.height)).ToList();
            points.Add(points[1]/3);points.Add(points[1]*2/3);
            var angles=new float[17];var next=new[]{0,1,2,4,5,5,7,8,8,10,11,11,13,14,14};
            foreach(var i in new[]{3,4,6,7,9,10,12,13}){var d=points[next[i]]-points[i];angles[i]=Mathf.Atan2(d.y,d.x)*Mathf.Rad2Deg+90;}
            angles[5]=angles[4];angles[8]=angles[7];
            var body=Rect("Continuous Skin",original.transform,new Vector2(320,320),Vector2.zero).gameObject.AddComponent<FootballSkin>();
            body.Configure(sprite,bones,origin,points.ToArray(),angles,keeper);
            held.transform.SetAsLastSibling();
            var hands=Rect("Skin Hands",original.transform,new Vector2(320,320),Vector2.zero).gameObject.AddComponent<FootballSkin>();
            hands.Configure(sprite,bones,origin,points.ToArray(),angles,keeper,true);
            var sourceTexture=new Texture2D(2,2,TextureFormat.RGBA32,false);
            sourceTexture.LoadImage(System.IO.File.ReadAllBytes(path));
            var sourcePixels=sourceTexture.GetPixels32();
            body.BakeSurfaceWeights(sourcePixels,sourceTexture.width,sourceTexture.height);
            hands.BakeSurfaceWeights(sourcePixels,sourceTexture.width,sourceTexture.height);
            Object.DestroyImmediate(sourceTexture);
            var nativeSprite=body.CreateWeightedSprite();nativeSprite.name=keeper?"Keeper Weighted":"Striker Weighted";
            var assetPath="Assets/Sprites/Characters/"+(keeper?"KeeperWeighted":"StrikerWeighted")+".asset";
            var saved=AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
            if(saved==null){AssetDatabase.CreateAsset(nativeSprite,assetPath);saved=nativeSprite;}
            else{EditorUtility.CopySerialized(nativeSprite,saved);Object.DestroyImmediate(nativeSprite);EditorUtility.SetDirty(saved);}
            var nativeObject=new GameObject("Unity Sprite Skin");nativeObject.transform.SetParent(original.transform,false);
            var renderer=nativeObject.AddComponent<SpriteRenderer>();renderer.sprite=saved;renderer.forceRenderingOff=true;
            var native=nativeObject.AddComponent<UnityEngine.U2D.Animation.SpriteSkin>();
            native.alwaysUpdate=true;native.forceCpuDeformation=true;native.SetRootBone(bones[0]);native.SetBoneTransforms(bones.Cast<Transform>().ToArray());
            body.ConfigureNative(native);hands.ConfigureNative(native);
            foreach(var piece in art)piece.enabled=false;waist.enabled=false;
            rig.ConfigureSkin(body,hands);rig.SetMaterial(original.material);
        }
        private static void RebuildCrowd(Transform stage)
        {
            var stands=stage.Find("02 Stands");var old=stands.Find("Animated Sections");if(old!=null)Object.DestroyImmediate(old.gameObject);
            var root=Rect("Animated Sections",stands,Vector2.zero,Vector2.zero);var sections=new RectTransform[12];var texture=stands.GetComponent<RawImage>().texture;
            for(var row=0;row<2;row++)for(var column=0;column<6;column++)
            {
                var top=row==0?210:339;var bottom=row==0?324:464;var h=(bottom-top)*.625f;
                var mask=Image("Continuous fan section",root,new Vector2(160,h),new Vector2((column*256+128-768)*.625f,(352.5f-(top+bottom)*.5f)*.625f),new Color32(20,32,57,255));mask.gameObject.AddComponent<RectMask2D>();
                var rt=Rect("Fans",mask.transform,new Vector2(160,h),Vector2.zero);var raw=rt.gameObject.AddComponent<RawImage>();raw.texture=texture;raw.uvRect=new Rect(column/6f,(1024-bottom)/1024f,1/6f,(bottom-top)/1024f);raw.raycastTarget=false;sections[row*6+column]=rt;
            }
            root.localScale=new Vector3(((RectTransform)stage).rect.width/960,1,1);root.gameObject.AddComponent<CrowdCelebration>().Configure(sections);
        }
        private static Sprite[] Import(string name)
        {
            var path="Assets/Sprites/Characters/"+name+".png";AssetDatabase.ImportAsset(path,ImportAssetOptions.ForceSynchronousImport);
            var importer=(TextureImporter)AssetImporter.GetAtPath(path);importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Multiple;
            importer.filterMode=FilterMode.Point;importer.mipmapEnabled=false;importer.isReadable=true;importer.maxTextureSize=2048;importer.textureCompression=TextureImporterCompression.Uncompressed;importer.npotScale=TextureImporterNPOTScale.None;importer.SaveAndReimport();
            var t=AssetDatabase.LoadAssetAtPath<Texture2D>(path);var pixels=t.GetPixels32();var data=new SpriteMetaData[10];
            for(var i=0;i<9;i++)
            {
                int x0=i%3*t.width/3,x1=(i%3+1)*t.width/3,y0=t.height-(i/3+1)*t.height/3,y1=t.height-i/3*t.height/3;
                int a=x1,b=x0,c=y1,d=y0;
                for(var y=y0;y<y1;y++)for(var x=x0;x<x1;x++){var p=pixels[y*t.width+x];if(p.r>110&&p.b>85&&p.g<Mathf.Min(p.r,p.b)*.6f)continue;a=Mathf.Min(a,x);b=Mathf.Max(b,x);c=Mathf.Min(c,y);d=Mathf.Max(d,y);}
                var bounds=new Rect(a,c,b-a+1,d-c+1);
                // Trim the atlas' construction end caps; adjacent bones overlap at joints.
                if(i==3||i==4||i==6||i==7){bounds.yMin+=bounds.height*.09f;bounds.yMax-=bounds.height*.08f;}
                if(name=="StrikerRig"&&i==1){var inset=bounds.width*.15f;bounds.xMin+=inset;bounds.xMax-=inset;}
                if(name=="StrikerRig"&&i==0)bounds.yMin+=bounds.height*.10f;
                data[i]=new SpriteMetaData{name=name+"_"+i,rect=bounds,alignment=0,pivot=new Vector2(.5f,.5f)};
            }
            var torso=data[1].rect;
            data[9]=new SpriteMetaData{name=name+"_9",rect=new Rect(torso.x+torso.width*.22f,torso.y+torso.height*.08f,torso.width*.56f,torso.height*.22f),alignment=0,pivot=new Vector2(.5f,.5f)};
#pragma warning disable 618
            importer.spritesheet=data;
#pragma warning restore 618
            importer.isReadable=false;importer.SaveAndReimport();return AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().OrderBy(s=>s.name).ToArray();
        }
        public static void Capture()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/Gameplay.unity");var game=Object.FindFirstObjectByType<GameplayController>();var kits=game.GetComponent<PlayerKitColours>();kits.SetShooterPlayer(1);
            MenuPreview.Render(1280,720,"ready","detailed-rig");kits.SetShooterPlayer(1);MenuPreview.Render(1280,720,"ready","detailed-rig");
            foreach(var d in System.Enum.GetValues(typeof(ShotDirection)).Cast<ShotDirection>())
            {
                game.Presentation.Sample(new ShotResult(d,d),ShotPresentation.ImpactTime);MenuPreview.Render(1280,720,"contact-"+d,"detailed-rig");
                game.Presentation.Sample(new ShotResult(d,d),ShotPresentation.ImpactTime+.6f);MenuPreview.Render(1280,720,"hold-"+d,"detailed-rig");
            }
            var shot=new ShotResult(ShotDirection.RightHigh,ShotDirection.RightHigh);
            for(var i=0;i<80;i++){game.Presentation.Sample(shot,i/30f);MenuPreview.Render(1280,720,"save-"+i.ToString("D3"),"detailed-rig");}
            var goal=new ShotResult(ShotDirection.LeftHigh,ShotDirection.Right);
            for(var i=0;i<132;i++){game.Presentation.Sample(goal,i/30f);MenuPreview.Render(1280,720,"goal-"+i.ToString("D3"),"detailed-rig");}
            game.Presentation.ResetPose();kits.SetShooterPlayer(2);MenuPreview.Render(1280,720,"player2","detailed-rig");
            EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
        }
    }
}
