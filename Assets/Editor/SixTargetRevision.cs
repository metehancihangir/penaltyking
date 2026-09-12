using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
namespace PenaltyKing.Editor
{
    public static class SixTargetRevision
    {
        [MenuItem("Penalty King/Apply Six Targets (no APK)")]
        public static void Setup()
        {
            var shots=Import("StrikerShot12",12,out var toe);
            var saves=Import("KeeperSix",8,out _);
            var scene=EditorSceneManager.OpenScene("Assets/Scenes/Gameplay.unity");
            var game=Object.FindFirstObjectByType<GameplayController>();
            game.Presentation.ConfigureSixAnimation(shots,toe,saves);
            var parent=game.Left.transform.parent;var zones=game.Targets;
            for(var i=0;i<6;i++)
            {
                if(zones[i]==null)zones[i]=Object.Instantiate(zones[i%3],parent);
                var zone=zones[i];zone.name=ShotTargets.Label((ShotDirection)i)+" Target";
                foreach(var text in zone.GetComponentsInChildren<Text>(true))Object.DestroyImmediate(text.gameObject);
                var marker=zone.GetComponentInChildren<TargetReticle>();
                if(marker==null)
                {
                    var obj=new GameObject("Aim brackets",typeof(RectTransform),typeof(TargetReticle));obj.transform.SetParent(zone.transform,false);
                    marker=obj.GetComponent<TargetReticle>();marker.color=new Color32(208,239,181,150);
                    marker.rectTransform.sizeDelta=new Vector2(48,32);marker.raycastTarget=false;
                }
                if(marker.GetComponent<CanvasRenderer>()==null)marker.gameObject.AddComponent<CanvasRenderer>();
                marker.rectTransform.sizeDelta=new Vector2(48,24);
            }
            game.ConfigureSixTargets(zones);
            var data=new SerializedObject(game.Presentation);
            var crowd=(RectTransform)data.FindProperty("crowd").objectReferenceValue;
            // Retain its transform for old references, but remove the large foreground faces.
            var crowdImage=crowd.GetComponent<Image>();if(crowdImage!=null)crowdImage.enabled=false;
            foreach(Transform child in crowd)child.gameObject.SetActive(false);
            var shadow=(Image)data.FindProperty("shadow").objectReferenceValue;
            shadow.sprite=null;shadow.preserveAspect=false;shadow.color=new Color(0.035f,.08f,.035f,.28f);
            shadow.rectTransform.sizeDelta=new Vector2(20,6);
            if(shadow.GetComponent<BallShadowMesh>()==null)shadow.gameObject.AddComponent<BallShadowMesh>();
            UpgradeGuide();
            var layout=Object.FindFirstObjectByType<GameplayStageLayout>();layout.enabled=false;layout.enabled=true;
            Canvas.ForceUpdateCanvases();layout.Fit();
            EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();
            Capture();
        }
        private static void UpgradeGuide()
        {
            var practice=Object.FindFirstObjectByType<GuidePractice>(FindObjectsInactive.Include);if(practice==null)return;
            var data=new SerializedObject(practice);var targets=data.FindProperty("targets");
            var original=Enumerable.Range(0,targets.arraySize).Select(i=>(Button)targets.GetArrayElementAtIndex(i).objectReferenceValue).ToArray();
            var diagram=original[0].transform.parent;
            foreach(Transform child in diagram.Cast<Transform>().ToArray())
                if(!original.Any(b=>b.transform==child))Object.DestroyImmediate(child.gameObject);
            var mini=PhaseOneSetup.Image("Mini goal",diagram,new Vector2(300,126),Vector2.zero,new Color32(28,57,60,255));mini.transform.SetAsFirstSibling();
            PhaseOneSetup.Image("Bar",mini.transform,new Vector2(284,2),new Vector2(0,59),new Color32(174,196,192,255));
            for(var side=-1;side<=1;side+=2)PhaseOneSetup.Image("Post",mini.transform,new Vector2(2,118),new Vector2(side*142,0),new Color32(174,196,192,255));
            targets.arraySize=6;
            for(var i=0;i<6;i++)
            {
                var button=i<original.Length?original[i]:Object.Instantiate(original[i%3],original[0].transform.parent);
                button.name="Practice "+ShotTargets.Label((ShotDirection)i);
                var rt=(RectTransform)button.transform;rt.anchoredPosition=new Vector2((i%3-1)*86,i>=3?31:-31);rt.sizeDelta=new Vector2(76,48);
                foreach(Transform child in button.transform.Cast<Transform>().ToArray())Object.DestroyImmediate(child.gameObject);
                PhaseOneSetup.Label("Zone label",button.transform,ShotTargets.Label((ShotDirection)i),new Vector2(72,38),Vector2.zero,14,new Color32(227,238,230,255));
                targets.GetArrayElementAtIndex(i).objectReferenceValue=button;
            }
            data.ApplyModifiedPropertiesWithoutUndo();
            var card=(RectTransform)practice.transform;card.sizeDelta=new Vector2(440,430);card.anchoredPosition=new Vector2(0,-45);
            ((RectTransform)card.Find("Edge")).anchoredPosition=new Vector2(0,213);
            ((RectTransform)card.Find("Page")).anchoredPosition=new Vector2(0,163);
            ((RectTransform)card.Find("Title")).anchoredPosition=new Vector2(0,104);
            ((RectTransform)diagram).anchoredPosition=new Vector2(0,10);
            ((RectTransform)card.Find("Description")).anchoredPosition=new Vector2(0,-84);
            ((RectTransform)card.Find("Practice Extras/Caption")).anchoredPosition=new Vector2(0,190);
            var hint=(RectTransform)card.Find("Practice Extras/Hint");hint.anchoredPosition=new Vector2(0,-132);hint.sizeDelta=new Vector2(400,18);hint.GetComponent<Text>().fontSize=12;
            var steps=card.Find("Practice Extras").Cast<Transform>().Where(t=>t.name=="Step").ToArray();
            foreach(var step in steps){var r=(RectTransform)step;r.anchoredPosition=new Vector2(r.anchoredPosition.x,140);}
            var next=(RectTransform)Object.FindFirstObjectByType<FirstPlayGuide>().Next.transform;next.anchoredPosition=new Vector2(0,-178);next.sizeDelta=new Vector2(next.sizeDelta.x,64);
        }
        private static bool Ink(Color32 c)=>c.a>32&&!(c.r>110&&c.b>85&&c.g<Mathf.Min(c.r,c.b)*.6f);
        private static Sprite[] Import(string name,int count,out Vector2 contact)
        {
            var path="Assets/Sprites/Characters/"+name+".png";AssetDatabase.ImportAsset(path,ImportAssetOptions.ForceSynchronousImport);
            var importer=(TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Multiple;
            importer.filterMode=FilterMode.Point;importer.mipmapEnabled=false;importer.isReadable=true;
            importer.textureCompression=TextureImporterCompression.Uncompressed;importer.npotScale=TextureImporterNPOTScale.None;importer.maxTextureSize=2048;importer.SaveAndReimport();
            var texture=AssetDatabase.LoadAssetAtPath<Texture2D>(path);var pixels=texture.GetPixels32();var frames=new SpriteMetaData[count];contact=default;
            for(var i=0;i<count;i++)
            {
                var cols=count==8?(i<4?new[]{0f,.29f,.585f,.755f,1f}:new[]{0f,.305f,.575f,.76f,1f}):(i>=8?new[]{0f,.272f,.5f,.75f,1f}:new[]{0f,.25f,.5f,.75f,1f});
                int x0=Mathf.RoundToInt(cols[i%4]*texture.width),x1=Mathf.RoundToInt(cols[i%4+1]*texture.width);
                int y0=texture.height-(i/4+1)*texture.height/(count/4),y1=texture.height-i/4*texture.height/(count/4);
                int minX=x1,maxX=x0,minY=y1,maxY=y0;
                for(var y=y0;y<y1;y++)for(var x=x0;x<x1;x++)if(Ink(pixels[y*texture.width+x])){minX=Mathf.Min(minX,x);maxX=Mathf.Max(maxX,x);minY=Mathf.Min(minY,y);maxY=Mathf.Max(maxY,y);}
                if(maxX<=minX||maxY<=minY)throw new System.InvalidOperationException("Empty sprite "+i);
                var rect=new Rect(minX,minY,maxX-minX+1,maxY-minY+1);var pivot=Vector2.zero;int n=0;
                for(var y=minY;y<=maxY;y++)for(var x=minX;x<=maxX;x++)
                {
                    bool anchor=count==8&&i<2?x>maxX-20:count==8&&i==2?y>maxY-20:count==8&&i==3?y<minY+24&&Mathf.Abs(x-rect.center.x)<rect.width*.23f:y<minY+14;
                    if(anchor&&Ink(pixels[y*texture.width+x])){pivot+=new Vector2(x,y);n++;}
                }
                pivot/=Mathf.Max(1,n);
                frames[i]=new SpriteMetaData{name=name+"_"+i.ToString("D2"),rect=rect,alignment=9,pivot=new Vector2((pivot.x-minX)/rect.width,(pivot.y-minY)/rect.height)};
                if(count==12&&i==8)
                {
                    var toe=Vector2.zero;int t=0;
                    for(var y=minY;y<minY+rect.height*.5f;y++)for(var x=maxX-20;x<=maxX;x++)if(Ink(pixels[y*texture.width+x])){toe+=new Vector2(x,y);t++;}
                    contact=toe/Mathf.Max(1,t)-pivot;
                }
            }
#pragma warning disable 618
            importer.spritesheet=frames;
#pragma warning restore 618
            importer.isReadable=false;importer.SaveAndReimport();
            return AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().OrderBy(s=>s.name).ToArray();
        }
        public static void Capture()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/Gameplay.unity");
            var game=Object.FindFirstObjectByType<GameplayController>();var kits=game.GetComponent<PlayerKitColours>();kits.SetShooterPlayer(1);
            var markers=Object.FindObjectsByType<TargetReticle>(FindObjectsSortMode.None);
            foreach(var marker in markers)marker.color=new Color32(208,239,181,165);
            MenuPreview.Render(1280,720,"ready","six-target");kits.UpdateUv();MenuPreview.Render(1280,720,"ready","six-target");
            foreach(var marker in markers)Debug.Log($"[Target] {marker.transform.parent.name} verts={marker.canvasRenderer.GetMesh()?.vertexCount} cull={marker.canvasRenderer.cull} depth={marker.canvasRenderer.absoluteDepth} color={marker.color} active={marker.isActiveAndEnabled}");
            foreach(var marker in markers)marker.color=new Color(0,0,0,0);
            foreach(var d in System.Enum.GetValues(typeof(ShotDirection)).Cast<ShotDirection>())
            {
                game.Presentation.Sample(new ShotResult(d,d),ShotPresentation.ImpactTime);kits.UpdateUv();MenuPreview.Render(1280,720,"save-"+d,"six-target");
                game.Presentation.Sample(new ShotResult(d,(ShotDirection)(((int)d+1)%6)),ShotPresentation.ImpactTime+.2f);kits.UpdateUv();MenuPreview.Render(1280,720,"goal-"+d,"six-target");
            }
            var shot=new ShotResult(ShotDirection.RightHigh,ShotDirection.Left);
            for(var i=0;i<88;i++){game.Presentation.Sample(shot,i*.05f);kits.UpdateUv();MenuPreview.Render(1280,720,"frame-"+i.ToString("D2"),"six-target");}
            game.Presentation.ResetPose();Object.FindFirstObjectByType<FirstPlayGuide>().ShowPreview(0);MenuPreview.Render(1280,720,"guide","six-target");
            EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
        }
    }
}
