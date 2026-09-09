using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
namespace PenaltyKing.Tests
{
    public sealed class LandscapeRevisionTests
    {
        [UnityTest] public IEnumerator NetMovesOnGoalsPinsFrameAndResetsWhileKeeperScaleStaysConstant()
        {
            RuntimeBootstrap.EnsureServices();GameManager.Instance.SelectLocalMultiplayer();GameManager.Instance.SelectMode(GameMode.Endless);
            yield return SceneManager.LoadSceneAsync("Gameplay");yield return null;
            var shot=Object.FindFirstObjectByType<ShotPresentation>();var net=Object.FindFirstObjectByType<GoalNetRipple>();
            var rect=((RectTransform)net.transform).rect;
            shot.Sample(new ShotResult(ShotDirection.Center,ShotDirection.Right),ShotPresentation.ImpactTime+.14f);
            Assert.That(net.Moving,Is.True);
            Assert.That(net.Displacement(Vector2.zero).sqrMagnitude,Is.GreaterThan(.001f));
            Assert.That(net.Displacement(new Vector2(rect.xMin,0)),Is.EqualTo(Vector2.zero));
            Assert.That(net.Displacement(new Vector2(0,rect.yMax)),Is.EqualTo(Vector2.zero));
            shot.Sample(new ShotResult(ShotDirection.Right,ShotDirection.Right),ShotPresentation.ImpactTime+.14f);Assert.That(net.Moving,Is.False);
            var keeper=Object.FindFirstObjectByType<GameplayStageLayout>().transform.Find("06 Keeper").GetComponent<Image>();
            shot.Sample(new ShotResult(ShotDirection.Right,ShotDirection.Right),1.3f+ShotPresentation.WindupDelay);
            var pixelScale=keeper.rectTransform.rect.height/keeper.sprite.rect.height;
            shot.Sample(new ShotResult(ShotDirection.Right,ShotDirection.Right),1.6f+ShotPresentation.WindupDelay);
            Assert.That(keeper.rectTransform.rect.height/keeper.sprite.rect.height,Is.EqualTo(pixelScale).Within(.0001f),"Recovery must not inflate the character");
            shot.ResetPose();Assert.That(net.Moving,Is.False);
            yield return SceneManager.LoadSceneAsync("MainMenu");
        }
    }
}
