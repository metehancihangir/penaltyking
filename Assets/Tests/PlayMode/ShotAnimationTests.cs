using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace PenaltyKing.Tests
{
    public sealed class ShotAnimationTests
    {
        [UnitySetUp]
        public IEnumerator OpenGame()
        {
            RuntimeBootstrap.EnsureServices();
            GameManager.Instance.SelectLocalMultiplayer();
            GameManager.Instance.SelectMode(GameMode.FixedRound);
            yield return SceneManager.LoadSceneAsync("Gameplay"); yield return null;
        }

        [UnityTearDown]
        public IEnumerator CloseGame()
        {
            Time.timeScale = 1;
            yield return SceneManager.LoadSceneAsync("MainMenu");
            GameManager.Instance.BeginSelection();
        }

        [UnityTest]
        public IEnumerator TouchStartsRunUpThenExactlyOneKickImpactAndCompletionEvenAtZeroTimeScale()
        {
            var game = Object.FindFirstObjectByType<GameplayController>();
            var animation = game.Presentation;
            var cues = new List<string>();
            animation.Kick += () => { Assert.That(animation.Elapsed, Is.GreaterThanOrEqualTo(ShotPresentation.ContactTime)); cues.Add("kick"); };
            animation.Impact += shot => { Assert.That(animation.Elapsed, Is.GreaterThanOrEqualTo(ShotPresentation.ImpactTime)); cues.Add("impact"); };
            animation.Completed += () => cues.Add("complete");
            Time.timeScale = 0;
            yield return LocalTestInput.Choose(game, game.Right, game.Left);
            Assert.That(game.Round.ShotsTaken, Is.EqualTo(1));
            Assert.That(animation.BallPosition, Is.EqualTo(animation.BallRestPosition));
            Assert.That(cues, Is.Empty);
            yield return new WaitForSecondsRealtime(ShotPresentation.ContactTime + .18f);
            Assert.That(cues, Is.EqualTo(new[] { "kick" }));
            Assert.That(Vector2.Distance(animation.BallPosition, animation.BallRestPosition), Is.GreaterThan(10));
            game.Left.OnPointerDown(new PointerEventData(EventSystem.current));
            Assert.That(game.Round.ShotsTaken, Is.EqualTo(1));
            var deadline = Time.realtimeSinceStartup + ShotPresentation.DurationFor(new ShotResult(ShotDirection.Right, ShotDirection.Left)) + 2;
            while (game.State == PlayState.ShowingShot)
            { Assert.That(Time.realtimeSinceStartup, Is.LessThan(deadline)); yield return null; }
            Assert.That(cues, Is.EqualTo(new[] { "kick", "impact", "complete" }));
            Assert.That(animation.IsPlaying, Is.False);
            Assert.That(animation.BallPosition, Is.EqualTo(animation.BallRestPosition));
            Assert.That(game.State, Is.EqualTo(PlayState.PassingPhone));
        }

        [UnityTest]
        public IEnumerator ThreeSaveDirectionsHaveDistinctPosesAndOnlyGoalsCelebrate()
        {
            var animation = Object.FindFirstObjectByType<ShotPresentation>();
            foreach (var direction in new[] { ShotDirection.Left, ShotDirection.Center, ShotDirection.Right })
            {
                animation.Sample(new ShotResult(direction, direction), ShotPresentation.ImpactTime);
                Assert.That(animation.KeeperSprite.name, Is.EqualTo(direction == ShotDirection.Center ? "KeeperSix_03" : "KeeperSix_01"));
                Assert.That(animation.KeeperScale.x, Is.EqualTo(direction == ShotDirection.Left ? -1 : 1));
                Assert.That(animation.CrowdCelebrating, Is.False);
                animation.Sample(new ShotResult(direction, direction), 1.2f + ShotPresentation.WindupDelay);
                Assert.That(animation.KeeperSprite.name, Is.EqualTo(direction == ShotDirection.Center ? "KeeperSix_06" : "KeeperSix_04"));
                animation.Sample(new ShotResult(direction, direction), 1.7f + ShotPresentation.WindupDelay);
                Assert.That(animation.KeeperSprite.name, Is.EqualTo("KeeperSix_07"));
            }
            var goal = new ShotResult(ShotDirection.Right, ShotDirection.Left);
            animation.Sample(goal, .5f); Assert.That(animation.CrowdCelebrating, Is.False);
            animation.Sample(goal, ShotPresentation.ImpactTime + .3f); Assert.That(animation.CrowdCelebrating, Is.True);
            animation.ResetPose(); Assert.That(animation.CrowdCelebrating, Is.False);
            yield return null;
        }

        [UnityTest]
        public IEnumerator LeavingDuringRunUpCancelsPendingSoundCues()
        {
            var game = Object.FindFirstObjectByType<GameplayController>();
            var cueCount = 0;
            game.Presentation.Kick += () => cueCount++;
            game.Presentation.Impact += shot => cueCount++;
            yield return LocalTestInput.Choose(game, game.Left, game.Center);
            yield return SceneManager.LoadSceneAsync("MainMenu");
            yield return new WaitForSecondsRealtime(1.1f);
            Assert.That(cueCount, Is.Zero);
        }
    }
}

