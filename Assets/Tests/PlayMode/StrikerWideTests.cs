using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
namespace PenaltyKing.Tests
{
    public sealed class StrikerWideTests
    {
        [UnityTest] public IEnumerator AllGoalsRestOnTheirShadowBehindThePlayerAndShotHasEightPoses()
        {
            RuntimeBootstrap.EnsureServices();GameManager.Instance.SelectLocalMultiplayer();GameManager.Instance.SelectMode(GameMode.Endless);
            yield return SceneManager.LoadSceneAsync("Gameplay");yield return null;
            var p=Object.FindFirstObjectByType<ShotPresentation>();var stage=Object.FindFirstObjectByType<GameplayStageLayout>().transform;
            var ball=(RectTransform)stage.Find("08 Ball");var player=stage.Find("07 Shooter").GetComponent<Image>();
            var effects=stage.Find("Ball Effects");var shadow=effects.Find("Ball Shadow").GetComponent<Image>();
            var originalOrder=ball.GetSiblingIndex();var effectOrder=effects.GetSiblingIndex();
            Assert.That(((RectTransform)stage).rect.width,Is.GreaterThanOrEqualTo(1120));
            var frames=new HashSet<string>();var goal=new ShotResult(ShotDirection.Left,ShotDirection.Right);
            foreach(var t in new[]{0f,.15f,.3f,.42f,.6f,ShotPresentation.ContactTime,.82f,.98f}){p.Sample(goal,t);frames.Add(player.sprite.name);}
            Assert.That(frames.Count,Is.EqualTo(8));
            foreach(var direction in new[]{ShotDirection.Left,ShotDirection.Center,ShotDirection.Right})
            {
                p.ResetPose();p.Sample(new ShotResult(direction,direction==ShotDirection.Left?ShotDirection.Right:ShotDirection.Left),ShotPresentation.ImpactTime+1);
                Assert.That(ball.GetSiblingIndex(),Is.LessThan(player.transform.GetSiblingIndex()),"Distant ball must be occluded by the striker, never paint over his body");
                Assert.That(ball.GetSiblingIndex(),Is.LessThan(stage.Find("06 Keeper").GetSiblingIndex()));
                Assert.That(shadow.enabled,Is.True);
                var radius=ball.rect.height*ball.localScale.y*.5f;
                Assert.That(ball.anchoredPosition.y-radius,Is.EqualTo(shadow.rectTransform.anchoredPosition.y).Within(.01f));
                var position=ball.anchoredPosition;var spin=ball.localRotation;
                p.Sample(new ShotResult(direction,direction==ShotDirection.Left?ShotDirection.Right:ShotDirection.Left),ShotPresentation.ImpactTime+2);
                Assert.That(ball.anchoredPosition,Is.EqualTo(position));Assert.That(ball.localRotation,Is.EqualTo(spin));
            }
            p.ResetPose();Assert.That(ball.GetSiblingIndex(),Is.EqualTo(originalOrder));Assert.That(effects.GetSiblingIndex(),Is.EqualTo(effectOrder));
            yield return SceneManager.LoadSceneAsync("MainMenu");
        }
    }
}
