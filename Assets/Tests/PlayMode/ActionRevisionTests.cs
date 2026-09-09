using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
namespace PenaltyKing.Tests
{
    public sealed class ActionRevisionTests
    {
        [UnitySetUp] public IEnumerator Open()
        {
            RuntimeBootstrap.EnsureServices();GameManager.Instance.SelectLocalMultiplayer();GameManager.Instance.SelectMode(GameMode.Endless);
            yield return SceneManager.LoadSceneAsync("Gameplay");yield return null;
        }
        [UnityTearDown] public IEnumerator Close(){yield return SceneManager.LoadSceneAsync("MainMenu");}
        [UnityTest] public IEnumerator WindupContactsBallThenGoalSettlesWithFrontFansStill()
        {
            var p=Object.FindFirstObjectByType<ShotPresentation>();var stage=Object.FindFirstObjectByType<GameplayStageLayout>().transform;
            var player=stage.Find("07 Shooter").GetComponent<Image>();var ball=stage.Find("08 Ball").GetComponent<Image>();
            var front=(RectTransform)stage.Find("03 Crowd");var frontPosition=front.anchoredPosition;
            var goal=new ShotResult(ShotDirection.Right,ShotDirection.Left);
            p.Sample(goal,.3f);Assert.That(player.sprite.name,Is.EqualTo("Performance_1"));Assert.That(p.BallPosition,Is.EqualTo(p.BallRestPosition));
            p.Sample(goal,ShotPresentation.ContactTime);Assert.That(player.sprite.name,Is.EqualTo("Performance_2"));
            Assert.That(Vector2.Distance(p.ContactBootPosition,p.BallPosition),Is.LessThan(1));
            p.Sample(goal,ShotPresentation.ImpactTime+.7f);var settled=p.BallPosition;var rotation=ball.rectTransform.localRotation;
            var net=Object.FindFirstObjectByType<GoalNetRipple>();var local=(Vector2)net.transform.InverseTransformPoint(ball.transform.position);
            Assert.That(((RectTransform)net.transform).rect.Contains(local),Is.True);
            p.Sample(goal,ShotPresentation.ImpactTime+2);Assert.That(p.BallPosition,Is.EqualTo(settled));
            Assert.That(ball.rectTransform.localRotation,Is.EqualTo(rotation));Assert.That(front.anchoredPosition,Is.EqualTo(frontPosition));
            Assert.That(player.sprite.name,Is.EqualTo("Performance_5"));
            Assert.That(net.Moving,Is.False);
            var invalidations=0;net.GetComponent<Image>().RegisterDirtyVerticesCallback(()=>invalidations++);
            net.Sample(2.4f,Vector2.zero);net.Sample(2.5f,Vector2.zero);Assert.That(invalidations,Is.Zero,"Settled net must not request new geometry");
            p.ResetPose();Assert.That(player.sprite.name,Is.EqualTo("Performance_0"));Assert.That(p.GoalCelebrating,Is.False);
            yield return null;
        }
    }
}
