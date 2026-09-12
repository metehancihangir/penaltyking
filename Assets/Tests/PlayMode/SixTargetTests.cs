using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using Object=UnityEngine.Object;
namespace PenaltyKing.Tests
{
    public sealed class SixTargetTests
    {
        [Test] public void AllThirtySixPairsRequireExactColumnAndHeightToSave()
        {
            var values=(ShotDirection[])Enum.GetValues(typeof(ShotDirection));Assert.That(values.Length,Is.EqualTo(6));
            foreach(var shot in values)foreach(var keeper in values)
            {
                var round=new PenaltyRound(GameMode.Endless,5);round.ChooseShot(shot);
                Assert.That(round.ChooseKeeper(keeper).Outcome,Is.EqualTo(shot==keeper?ShotOutcome.Save:ShotOutcome.Goal));
            }
        }
        [UnityTest] public IEnumerator SixTargetsAreReachableAndGlovesMeetBallWithoutResizingNet()
        {
            var guidePref=PlayerPrefs.GetInt(FirstPlayGuide.CompletedKey,0);PlayerPrefs.SetInt(FirstPlayGuide.CompletedKey,1);
            RuntimeBootstrap.EnsureServices();GameManager.Instance.SelectLocalMultiplayer();GameManager.Instance.SelectMode(GameMode.Endless);
            yield return SceneManager.LoadSceneAsync("Gameplay");yield return null;
            var game=Object.FindFirstObjectByType<GameplayController>();var p=game.Presentation;
            var stage=Object.FindFirstObjectByType<GameplayStageLayout>().transform;
            Assert.That(stage.Find("03 Crowd").GetComponent<Image>().enabled,Is.False);
            var shadow=stage.Find("Ball Effects/Ball Shadow").GetComponent<Image>();Assert.That(shadow.sprite,Is.Null);Assert.That(shadow.GetComponent<BallShadowMesh>(),Is.Not.Null);
            var points=new HashSet<Vector2>();var keeperImage=stage.Find("06 Keeper").GetComponent<Image>();
            for(var i=0;i<6;i++)
            {
                var direction=(ShotDirection)i;Assert.That(game.Targets[i],Is.Not.Null);
                p.Sample(new ShotResult(direction,direction),ShotPresentation.ImpactTime);
                points.Add(p.BallPosition);Assert.That(p.BallPosition,Is.EqualTo(p.TargetPoint(direction)));
                var scale=keeperImage.rectTransform.rect.height/keeperImage.sprite.rect.height;
                var offset=(keeperImage.sprite.pivot-keeperImage.sprite.rect.size*.5f)*scale;offset.x*=keeperImage.rectTransform.localScale.x;
                Assert.That(Vector2.Distance(p.KeeperPosition+offset,p.BallPosition),Is.LessThan(.01f),"Gloves must meet "+direction);
            }
            Assert.That(points.Count,Is.EqualTo(6));
            var net=Object.FindFirstObjectByType<GoalNetRipple>();p.ResetPose();Canvas.ForceUpdateCanvases();
            var mesh=new Mesh();net.GetComponent<CanvasRenderer>().GetMesh(mesh);var before=mesh.bounds;
            p.Sample(new ShotResult(ShotDirection.CenterHigh,ShotDirection.Left),ShotPresentation.ImpactTime+.14f);Canvas.ForceUpdateCanvases();
            net.GetComponent<CanvasRenderer>().GetMesh(mesh);Assert.That(Vector3.Distance(mesh.bounds.size,before.size),Is.LessThan(.01f));Assert.That(Vector3.Distance(mesh.bounds.center,before.center),Is.LessThan(.01f));Object.Destroy(mesh);
            p.ResetPose();
            for(var i=0;i<6;i++)
            {
                yield return LocalTestInput.Choose(game,game.Targets[i],game.Targets[i]);
                Assert.That(game.LastShot.Value.Outcome,Is.EqualTo(ShotOutcome.Save));
                Assert.That(game.LastShot.Value.PlayerDirection,Is.EqualTo((ShotDirection)i));
                yield return LocalTestInput.Finish(game);
            }
            yield return SceneManager.LoadSceneAsync("MainMenu");PlayerPrefs.SetInt(FirstPlayGuide.CompletedKey,guidePref);
        }
    }
}
