using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using static PenaltyKing.Editor.PhaseOneSetup;
namespace PenaltyKing.Editor
{
 public static class UpdatePhaseFiveSetup
 {
  static Color C(string hex)=>MenuPixelArt.Hex(hex);
  public static void Build()
  {
   var scene=EditorSceneManager.OpenScene("Assets/Scenes/Gameplay.unity");
   var game=Object.FindFirstObjectByType<GameplayController>();
   var stage=Object.FindFirstObjectByType<GameplayStageLayout>();
   var canvas=GameObject.Find("Gameplay Canvas").transform;
   ((RectTransform)stage.transform.Find("05 Goal and Net")).sizeDelta=new Vector2(480,172.8f);
   var pitch=stage.transform.Find("04 Pitch");
   var old=pitch.GetComponent<RawImage>();if(old!=null)Object.DestroyImmediate(old);
   var grass=pitch.GetComponent<PixelPitch>();if(grass==null)grass=pitch.gameObject.AddComponent<PixelPitch>();grass.raycastTarget=false;
   var index=0;
   foreach(var zone in new[]{game.Left,game.Center,game.Right})
   { var r=(RectTransform)zone.transform;r.sizeDelta=new Vector2(148,136);r.anchoredPosition=new Vector2((index++-1)*152,66.8f); }
   var previous=canvas.Find("First Play Guide");if(previous!=null)Object.DestroyImmediate(previous.gameObject);
   var root=Rect("First Play Guide",canvas,Vector2.zero,Vector2.zero);Stretch(root);
   root.SetSiblingIndex(canvas.Find("Live HUD").GetSiblingIndex());
   var shade=Image("Guide Shade",root,Vector2.zero,Vector2.zero,new Color(.02f,.05f,.08f,.94f));Stretch(shade.rectTransform);shade.raycastTarget=true;
   var safe=Rect("Safe Area",shade.transform,Vector2.zero,Vector2.zero);Stretch(safe);safe.gameObject.AddComponent<SafeAreaPanel>();
   var card=Image("Guide Card",safe,new Vector2(440,440),Vector2.zero,C("101E2B"));card.gameObject.AddComponent<FitPanelToSafeArea>();
   Image("Edge",card.transform,new Vector2(440,4),new Vector2(0,218),C("BAEB71"));
   var page=Label("Page",card.transform,"1 / 3",new Vector2(200,28),new Vector2(0,180),16,C("AEC4C0"));
   var title=Label("Title",card.transform,"ŞUTUNU SEÇ",new Vector2(420,48),new Vector2(0,133),28,C("E3EEE6"));
   var detail=Label("Description",card.transform,"",new Vector2(420,64),new Vector2(0,-68),20,C("E3EEE6"));
   var directions=Rect("Directions",card.transform,new Vector2(360,120),new Vector2(0,35));
   Image("Crossbar",directions,new Vector2(320,4),new Vector2(0,46),C("AEC4C0"));
   for(var i=0;i<3;i++)
   {
    float x=(i-1)*112;
    Image("Target",directions,new Vector2(90,48),new Vector2(x,18),C("274653"));
    Image("Arrow stem",directions,new Vector2(6,24),new Vector2(x,-32),C("BAEB71"));
    for(var row=0;row<4;row++)Image("Arrow pixel",directions,new Vector2(6+row*6,4),new Vector2(x,-10-row*4),C("BAEB71"));
    Label("Direction",directions,new[]{"SOL","ORTA","SAĞ"}[i],new Vector2(100,28),new Vector2(x,18),17,C("E3EEE6"));
   }
   var handoff=Rect("Phone Handoff",card.transform,new Vector2(360,120),new Vector2(0,35));
   foreach(var x in new[]{-125,125})
   {Image("Head",handoff,new Vector2(24,24),new Vector2(x,24),C("BAEB71"));Image("Body",handoff,new Vector2(42,26),new Vector2(x,-6),C("3A6170"));}
   var phone=Image("Phone",handoff,new Vector2(44,72),Vector2.zero,C("AEC4C0"));Image("Screen",phone.transform,new Vector2(32,50),new Vector2(0,4),C("274653"));
   foreach(var x in new[]{-64,64})Label("Pass",handoff,">",new Vector2(32,42),new Vector2(x,0),30,C("BAEB71"));
   var next=Button(card.transform,"Devam",-157,true,true);Object.DestroyImmediate(next.Face.Find("Arrow").gameObject);
   var label=next.Face.Find("Label").GetComponent<Text>();label.rectTransform.anchoredPosition=Vector2.zero;
   var guide=root.gameObject.AddComponent<FirstPlayGuide>();guide.Configure(game,shade.gameObject,directions.gameObject,handoff.gameObject,title,detail,page,label,next);
   shade.gameObject.SetActive(false);stage.enabled=false;stage.enabled=true;Canvas.ForceUpdateCanvases();stage.Fit();
   EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();Capture();
   EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");Debug.Log("[Update Phase 5] Scene and previews saved.");
  }
  public static void Capture()
  {
   EditorSceneManager.OpenScene("Assets/Scenes/Gameplay.unity");
   MenuPreview.Render(390,844,"portrait","update5-gameplay");MenuPreview.Render(1280,720,"landscape","update5-gameplay");
   var game=Object.FindFirstObjectByType<GameplayController>();
   game.Presentation.Sample(new ShotResult(ShotDirection.Right,ShotDirection.Right),.9f);
   MenuPreview.Render(1280,720,"save","update5-gameplay");
   game.Presentation.Sample(new ShotResult(ShotDirection.Left,ShotDirection.Right),.24f);
   MenuPreview.Render(1280,720,"contact","update5-gameplay");game.Presentation.ResetPose();
   var guide=Object.FindFirstObjectByType<FirstPlayGuide>();
   for(var i=0;i<3;i++){guide.ShowPreview(i);MenuPreview.Render(390,844,"step"+(i+1),"update5-guide");}
   guide.ShowPreview(0);MenuPreview.Render(1280,720,"landscape","update5-guide");
  }
 }
}

