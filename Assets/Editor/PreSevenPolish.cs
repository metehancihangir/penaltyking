using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using static PenaltyKing.Editor.PhaseOneSetup;
namespace PenaltyKing.Editor
{
 public static class PreSevenPolish
 {
  static Color C(string hex)=>MenuPixelArt.Hex(hex);
  public static void Build()
  {
   var scene=EditorSceneManager.OpenScene("Assets/Scenes/Gameplay.unity");var game=Object.FindFirstObjectByType<GameplayController>();var canvas=GameObject.Find("Gameplay Canvas").transform;
   var handoff=game.Handoff;handoff.GetComponent<Image>().color=new Color(0,0,0,.12f);
   var card=(RectTransform)handoff.transform.Find("Safe Area/Card");card.sizeDelta=new Vector2(420,124);
   var title=card.Find("Player").GetComponent<Text>();title.rectTransform.anchoredPosition=Vector2.zero;title.rectTransform.sizeDelta=new Vector2(410,100);title.fontSize=30;
   card.Find("Role").gameObject.SetActive(false);card.Find("Continue").gameObject.SetActive(false);
   var accent=(RectTransform)card.Find("Accent");accent.anchoredPosition=new Vector2(0,60);if(card.GetComponent<FitPanelToSafeArea>()==null)card.gameObject.AddComponent<FitPanelToSafeArea>();
   PolishGuide(game,canvas);Crowds();
   var sound=game.GetComponent<GameplayAudio>();
   foreach(var path in new[]{"Assets/Audio/Stadium/CalmDrumLoop.wav","Assets/Audio/Stadium/GoalVictory.wav","Assets/Audio/Stadium/SaveOff.wav","Assets/Resources/Audio/MenuPixelTheme.wav"})
   {
    AssetDatabase.ImportAsset(path,ImportAssetOptions.ForceSynchronousImport);var importer=(AudioImporter)AssetImporter.GetAtPath(path);var settings=importer.defaultSampleSettings;settings.loadType=AudioClipLoadType.DecompressOnLoad;settings.compressionFormat=AudioCompressionFormat.PCM;settings.preloadAudioData=true;importer.defaultSampleSettings=settings;importer.SaveAndReimport();
   }
   sound.Configure(AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Stadium/CalmDrumLoop.wav"),sound.KickClip,AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Stadium/GoalVictory.wav"),AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Stadium/SaveOff.wav"));
   EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();Capture();EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
   Debug.Log("[Pre7] Polish scene and audio saved.");
  }
  static void PolishGuide(GameplayController game,Transform canvas)
  {
   var guide=Object.FindFirstObjectByType<FirstPlayGuide>();var shade=guide.transform.Find("Guide Shade").GetComponent<Image>();shade.color=new Color(.02f,.05f,.08f,.60f);
   var card=(RectTransform)shade.transform.Find("Safe Area/Guide Card");card.sizeDelta=new Vector2(440,490);
   ((RectTransform)card.Find("Edge")).anchoredPosition=new Vector2(0,243);
   ((RectTransform)card.Find("Page")).anchoredPosition=new Vector2(0,185);
   ((RectTransform)card.Find("Title")).anchoredPosition=new Vector2(0,133);
   ((RectTransform)guide.Next.transform).anchoredPosition=new Vector2(0,-195);
   if(card.Find("Practice Extras")!=null)Object.DestroyImmediate(card.Find("Practice Extras").gameObject);
   var extras=Rect("Practice Extras",card,Vector2.zero,Vector2.zero);
   Label("Caption",extras,"İKİ KİŞİ  /  TEK TELEFON",new Vector2(400,28),new Vector2(0,217),15,C("AEC4C0"));
   Label("Hint",extras,"Güvenli bir deneme. Skoru etkilemez.",new Vector2(400,28),new Vector2(0,-136),14,C("AEC4C0"));
   var dots=new Image[3];for(var i=0;i<3;i++)dots[i]=Image("Step",extras,new Vector2(94,5),new Vector2((i-1)*105,165),C("274653"));
   var diagram=card.Find("Directions");
   if(diagram.Find("Mini pitch")!=null)Object.DestroyImmediate(diagram.Find("Mini pitch").gameObject);
   var mini=Image("Mini pitch",diagram,new Vector2(340,90),new Vector2(0,21),C("1C393C"));mini.transform.SetAsFirstSibling();
   Image("Left post",mini.transform,new Vector2(4,50),new Vector2(-160,0),C("AEC4C0"));
   Image("Right post",mini.transform,new Vector2(4,50),new Vector2(160,0),C("AEC4C0"));
   for(var x=-140;x<=140;x+=20)Image("Net",mini.transform,new Vector2(1,50),new Vector2(x,0),C("3A6170"));
   var choices=new List<Button>();foreach(Transform child in card.Find("Directions"))
   {
    if(child.name!="Target")continue;
    var image=child.GetComponent<Image>();image.raycastTarget=true;var button=child.GetComponent<Button>();if(button==null)button=child.gameObject.AddComponent<Button>();button.targetGraphic=image;button.transition=Selectable.Transition.None;button.navigation=new Navigation{mode=Navigation.Mode.None};choices.Add(button);
   }
   var practice=card.GetComponent<GuidePractice>();if(practice==null)practice=card.gameObject.AddComponent<GuidePractice>();practice.Configure(guide,game,choices.ToArray(),dots,card.Find("Description").GetComponent<Text>());
   card.Find("Description").GetComponent<Text>().fontSize=21;
  }
  static void Crowds()
  {
   var stage=Object.FindFirstObjectByType<GameplayStageLayout>();var stands=stage.transform.Find("02 Stands");var old=stands.Find("Animated Sections");if(old!=null)Object.DestroyImmediate(old.gameObject);
   var root=Rect("Animated Sections",stands,Vector2.zero,Vector2.zero);var parts=new List<RectTransform>();var texture=stands.GetComponent<RawImage>().texture;
   var lefts=new[]{0,282,528,801,1038,1286};var rights=new[]{253,502,764,1006,1257,1536};
   foreach(var top in new[]{215,342})for(var i=0;i<6;i++)
   {
    int bottom=top==215?322:460;float width=(rights[i]-lefts[i])*.625f,height=(bottom-top)*.625f;
    var mask=Image("Fan section",root,new Vector2(width,height),new Vector2(((lefts[i]+rights[i])*.5f-768)*.625f,(352.5f-(top+bottom)*.5f)*.625f),C("142039"));mask.gameObject.AddComponent<RectMask2D>();
    var rect=Rect("Fans",mask.transform,new Vector2(width,height),Vector2.zero);var image=rect.gameObject.AddComponent<RawImage>();image.texture=texture;image.uvRect=new Rect(lefts[i]/1536f,(1024-bottom)/1024f,(rights[i]-lefts[i])/1536f,(bottom-top)/1024f);image.raycastTarget=false;parts.Add(rect);
   }
   root.gameObject.AddComponent<CrowdCelebration>().Configure(parts.ToArray());
  }
  public static void Capture()
  {
   EditorSceneManager.OpenScene("Assets/Scenes/Gameplay.unity");MenuPreview.Render(1280,720,"landscape","pre7-gameplay");MenuPreview.Render(390,844,"portrait","pre7-gameplay");
   var game=Object.FindFirstObjectByType<GameplayController>();game.Handoff.Show(1,false,null);MenuPreview.Render(1280,720,"landscape","pre7-handoff");MenuPreview.Render(390,844,"portrait","pre7-handoff");game.Handoff.Hide();
   var guide=Object.FindFirstObjectByType<FirstPlayGuide>();guide.ShowPreview(0);MenuPreview.Render(390,844,"portrait","pre7-guide");MenuPreview.Render(1280,720,"landscape","pre7-guide");guide.transform.Find("Guide Shade").gameObject.SetActive(false);
   game.Presentation.Sample(new ShotResult(ShotDirection.Left,ShotDirection.Right),1.03f);MenuPreview.Render(1280,720,"goal","pre7-gameplay");
  }
 }
}

