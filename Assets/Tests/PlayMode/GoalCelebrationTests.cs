using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
namespace PenaltyKing.Tests
{
    public sealed class GoalCelebrationTests
    {
        [UnitySetUp] public IEnumerator Open()
        {
            RuntimeBootstrap.EnsureServices();
            GameManager.Instance.SelectLocalMultiplayer(); GameManager.Instance.SelectMode(GameMode.Endless);
            yield return SceneManager.LoadSceneAsync("Gameplay"); yield return null;
        }
        [UnityTearDown] public IEnumerator Close() { yield return SceneManager.LoadSceneAsync("MainMenu"); }
        [UnityTest] public IEnumerator GoalLastsThreeSecondsPausesAndResetsBeforeNextPlayer()
        {
            var game = Object.FindFirstObjectByType<GameplayController>(); var p = game.Presentation;
            yield return LocalTestInput.Choose(game, game.Left, game.Right);
            var deadline = Time.realtimeSinceStartup + 3;
            while (!p.GoalCelebrating) { Assert.That(Time.realtimeSinceStartup, Is.LessThan(deadline)); yield return null; }
            yield return new WaitForSecondsRealtime(.55f);
            Assert.That(p.CrowdCelebrating, Is.True);
            var kits = game.GetComponent<PlayerKitColours>(); Assert.That(kits.ShooterPlayer, Is.EqualTo(1));
            LocalTestInput.Press(game.Exit);
            var age = p.Elapsed; var position = p.ShooterPosition;
            yield return new WaitForSecondsRealtime(.4f);
            Assert.That(p.Elapsed, Is.EqualTo(age)); Assert.That(p.ShooterPosition, Is.EqualTo(position));
            LocalTestInput.Press(game.GetComponent<ExitConfirmation>().CancelButton);
            deadline = Time.realtimeSinceStartup + 7;
            var lastVisible = p.Elapsed;
            while (game.State == PlayState.ShowingShot)
            {
                Assert.That(Time.realtimeSinceStartup, Is.LessThan(deadline));
                Assert.That(p.GoalCelebrating, Is.True);
                Assert.That(kits.ShooterPlayer, Is.EqualTo(1));
                lastVisible = p.Elapsed;
                yield return null;
            }
            Assert.That(lastVisible, Is.GreaterThan(ShotPresentation.ImpactTime + 2.8f));
            Assert.That(p.GoalCelebrating || p.CrowdCelebrating, Is.False);
            Assert.That(kits.ShooterPlayer, Is.EqualTo(2));
            Assert.That(game.State, Is.EqualTo(PlayState.PassingPhone));
            Assert.That(p.BallPosition, Is.EqualTo(p.BallRestPosition));
        }
        [UnityTest] public IEnumerator SavesHaveNoGoalOverlayAndCancellationClearsCelebration()
        {
            var p = Object.FindFirstObjectByType<ShotPresentation>();
            var goal = new ShotResult(ShotDirection.Left, ShotDirection.Right);
            var save = new ShotResult(ShotDirection.Left, ShotDirection.Left);
            Assert.That(ShotPresentation.DurationFor(save), Is.EqualTo(ShotPresentation.Duration));
            Assert.That(ShotPresentation.DurationFor(goal) - ShotPresentation.ImpactTime, Is.EqualTo(3).Within(.001f));
            p.Sample(goal, ShotPresentation.ImpactTime - .01f); Assert.That(p.GoalCelebrating, Is.False);
            p.Sample(goal, ShotPresentation.ImpactTime + 2.99f); Assert.That(p.GoalCelebrating, Is.True);
            p.Sample(goal, ShotPresentation.ImpactTime + 3); Assert.That(p.GoalCelebrating, Is.False);
            p.Sample(goal, 1.5f); p.Cancel(); Assert.That(p.GoalCelebrating, Is.False);
            p.Sample(save, 1.5f); Assert.That(p.GoalCelebrating || p.CrowdCelebrating, Is.False);
            yield return null;
        }
    }
}
