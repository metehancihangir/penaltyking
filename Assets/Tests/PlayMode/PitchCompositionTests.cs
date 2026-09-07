using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
namespace PenaltyKing.Tests
{
 public sealed class PitchCompositionTests
 {
  [UnityTest] public IEnumerator TargetsAndSpotFollowPortraitAndLandscapeComposition()
  {
   RuntimeBootstrap.EnsureServices();GameManager.Instance.SelectLocalMultiplayer();GameManager.Instance.SelectMode(GameMode.FixedRound);
   yield return SceneManager.LoadSceneAsync("Gameplay");yield return null;
   var game=Object.FindFirstObjectByType<GameplayController>();var layout=Object.FindFirstObjectByType<GameplayStageLayout>();
   var viewport=new GameObject("Test Viewport",typeof(RectTransform)).GetComponent<RectTransform>();viewport.SetParent(layout.transform.parent,false);layout.transform.SetParent(viewport,false);
   var goal=(RectTransform)layout.transform.Find("05 Goal and Net");var pitch=Object.FindFirstObjectByType<PixelPitch>();
   foreach(var size in new[]{new Vector2(960,540),new Vector2(390,844)})
   {
    viewport.sizeDelta=size;Canvas.ForceUpdateCanvases();layout.Fit();Canvas.ForceUpdateCanvases();
    Assert.That(goal.rect.width,Is.EqualTo(480));Assert.That(game.Presentation.ShooterHeight,Is.InRange(210f,240f));
    Assert.That(pitch.rectTransform.anchoredPosition.y+pitch.SpotY,Is.EqualTo(game.Presentation.BallRestPosition.y-12).Within(.1f));
    foreach(var direction in new[]{ShotDirection.Left,ShotDirection.Center,ShotDirection.Right})
    {
     game.Presentation.Sample(new ShotResult(direction,direction),ShotPresentation.ImpactTime);
     var target=game.Presentation.BallPosition-goal.anchoredPosition;
     Assert.That(goal.rect.Contains(target),Is.True,"Impact must remain inside resized goal");
    }
    game.Presentation.ResetPose();
   }
   yield return SceneManager.LoadSceneAsync("MainMenu");
  }
 }
}
