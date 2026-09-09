using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using static PenaltyKing.Editor.PhaseOneSetup;
namespace PenaltyKing.Editor
{
 public static class MatchPolish
 {
  [MenuItem("Penalty King/Apply Match Polish (no APK)")]
  public static void Setup()
  {
   AssetDatabase.ImportAsset("Assets/Audio/Stadium/UserStadiumLoop.wav",ImportAssetOptions.ForceSynchronousImport);
   var scene=EditorSceneManager.OpenScene("Assets/Scenes/Gameplay.unity");
   var game=Object.FindFirstObjectByType<GameplayController>();var stage=Object.FindFirstObjectByType<GameplayStageLayout>();
   var material=AssetDatabase.LoadAssetAtPath<Material>("Assets/Resources/PlayerKit.mat");
   if(material==null){material=new Material(Shader.Find("PenaltyKing/PlayerKit"));AssetDatabase.CreateAsset(material,"Assets/Resources/PlayerKit.mat");}
   var kits=game.GetComponent<PlayerKitColours>();if(kits==null)kits=game.gameObject.AddComponent<PlayerKitColours>();
   kits.Configure(stage.transform.Find("07 Shooter").GetComponent<Image>(),stage.transform.Find("06 Keeper").GetComponent<Image>(),material);
   var canvas=GameObject.Find("Gameplay Canvas").transform;
   var old=canvas.Find("Exit Confirmation Shade");if(old!=null)Object.DestroyImmediate(old.gameObject);
   var shade=Image("Exit Confirmation Shade",canvas,Vector2.zero,Vector2.zero,new Color(.01f,.025f,.045f,.72f));Stretch(shade.rectTransform);shade.raycastTarget=true;
   var safe=Rect("Safe Area",shade.transform,Vector2.zero,Vector2.zero);Stretch(safe);safe.gameObject.AddComponent<SafeAreaPanel>();
   var card=Image("Exit Card",safe,new Vector2(490,320),Vector2.zero,new Color32(13,32,45,255));card.gameObject.AddComponent<FitPanelToSafeArea>();
   Image("Accent",card.transform,new Vector2(490,5),new Vector2(0,158),new Color32(186,235,113,255));
   Label("Title",card.transform,"Ana menüye dönülsün mü?",new Vector2(455,60),new Vector2(0,106),28,new Color32(227,238,230,255));
   Label("Description",card.transform,"Mevcut maçın ilerlemesi kaybolacak.",new Vector2(450,46),new Vector2(0,53),19,new Color32(174,196,192,255));
   var cancel=Button(card.transform,"Maça devam et",-22,true,true);var confirm=Button(card.transform,"Ana menüye dön",-106,true,false);
   var dialog=game.GetComponent<ExitConfirmation>();if(dialog==null)dialog=game.gameObject.AddComponent<ExitConfirmation>();dialog.Configure(game,shade.gameObject,cancel,confirm);
   shade.gameObject.SetActive(false);
   EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();
   Capture();EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
  }
  public static void Capture()
  {
   EditorSceneManager.OpenScene("Assets/Scenes/Gameplay.unity");
   var game=Object.FindFirstObjectByType<GameplayController>();var kits=game.GetComponent<PlayerKitColours>();
   kits.SetShooterPlayer(1);MenuPreview.Render(1280,720,"player1","match-polish");
   kits.SetShooterPlayer(2);MenuPreview.Render(1280,720,"player2","match-polish");
   var shade=GameObject.Find("Gameplay Canvas").transform.Find("Exit Confirmation Shade");shade.gameObject.SetActive(true);
   MenuPreview.Render(1280,720,"exit","match-polish");shade.gameObject.SetActive(false);
   for(var i=0;i<10;i++)
   {
    game.Presentation.Sample(new ShotResult(ShotDirection.Right,ShotDirection.Left),.9f+i*.08f);kits.UpdateUv();
    MenuPreview.Render(1280,720,"net-"+i,"match-polish");
   }
   EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
  }
 }
}
