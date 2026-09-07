using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
namespace PenaltyKing.Tests
{
    public sealed class MatchLifecycleTests
    {
        private static IEnumerator Open(GameMode mode)
        {
            RuntimeBootstrap.EnsureServices();
            GameManager.Instance.SelectLocalMultiplayer();
            GameManager.Instance.SelectMode(mode);
            yield return SceneManager.LoadSceneAsync("Gameplay"); yield return null;
        }
        [UnityTearDown]
        public IEnumerator Close()
        { yield return SceneManager.LoadSceneAsync("MainMenu"); GameManager.Instance.BeginSelection(); }

        [UnityTest]
        public IEnumerator EitherPlayerCanWinOnlyAfterTenShotsAndReplayClearsEveryPlayerField()
        {
            foreach (var winner in new[] { 1, 2 })
            {
                yield return Open(GameMode.FixedRound);
                var game = Object.FindFirstObjectByType<GameplayController>();
                for (var i = 0; i < 10; i++)
                {
                    var scoring = game.Round.ShooterPlayer == winner;
                    yield return LocalTestInput.Choose(game, game.Left, scoring ? game.Right : game.Left);
                    yield return LocalTestInput.Finish(game);
                    Assert.That(game.ResultVisible, Is.EqualTo(i == 9));
                }
                Assert.That(game.ResultTitle.text, Is.EqualTo($"PLAYER {winner} KAZANDI"));
                Assert.That(game.Round.Player1.Shots, Is.EqualTo(5));
                Assert.That(game.Round.Player2.Shots, Is.EqualTo(5));
                Assert.That(game.ResultScore.text, Is.EqualTo(winner == 1 ? "5  -  0" : "0  -  5"));
                yield return null;
                LocalTestInput.Press(game.Replay);
                Assert.That(game.Round.ShotsTaken, Is.Zero);
                Assert.That(game.Round.Player1.Goals + game.Round.Player2.Goals, Is.Zero);
                Assert.That(game.Round.Player1.History.Count + game.Round.Player2.History.Count, Is.Zero);
                Assert.That(game.Round.HasShotSelection, Is.False);
                Assert.That(game.LastShot, Is.Null);
                Assert.That(game.Round.ShooterPlayer, Is.EqualTo(1));
                Assert.That(game.Round.Mode, Is.EqualTo(GameMode.FixedRound));
                Assert.That(game.Presentation.BallPosition, Is.EqualTo(game.Presentation.BallRestPosition));
                Assert.That(game.ResultVisible, Is.False);
                Assert.That(AudioManager.Instance.SfxSource.isPlaying, Is.False);
                Assert.That(AudioManager.Instance.CrowdSource.isPlaying, Is.True);
                yield return LocalTestInput.Choose(game, game.Center, game.Center);
                Assert.That(game.Round.ShotsTaken, Is.EqualTo(1));
                yield return LocalTestInput.Finish(game);
            }
        }

        [UnityTest]
        public IEnumerator EndlessContinuesBeyondTenShotsWithGoalsAndSavesAndCanExit()
        {
            yield return Open(GameMode.Endless);
            var game = Object.FindFirstObjectByType<GameplayController>();
            for (var i = 0; i < 12; i++)
            {
                yield return LocalTestInput.Choose(game, game.Center, i % 2 == 0 ? game.Right : game.Center);
                yield return LocalTestInput.Finish(game);
                Assert.That(game.ResultVisible || game.Round.IsOver, Is.False);
                Assert.That(game.Round.ShooterPlayer, Is.EqualTo((i + 1) % 2 + 1));
            }
            Assert.That(game.Round.Player1.Goals, Is.EqualTo(6));
            Assert.That(game.Round.Player2.Goals, Is.Zero);
            LocalTestInput.Press(game.Exit);
            yield return WaitForHome();
            Assert.That(AudioManager.Instance.StadiumActive, Is.False);
        }

        [UnityTest]
        public IEnumerator ExitWorksDuringEveryTurnStageAndCancelsPendingCuesAndSelections()
        {
            for (var stage = 0; stage < 5; stage++)
            {
                yield return Open(GameMode.Endless);
                var game = Object.FindFirstObjectByType<GameplayController>();
                if (stage >= 1) yield return LocalTestInput.Continue(game);
                if (stage >= 2) LocalTestInput.Press(game.Left);
                if (stage >= 3) yield return LocalTestInput.Continue(game);
                if (stage >= 4) LocalTestInput.Press(game.Center);
                var cueCount = 0;
                game.Presentation.Kick += () => cueCount++;
                game.Presentation.Impact += result => cueCount++;
                LocalTestInput.Press(game.Exit);
                Assert.That(game.State, Is.EqualTo(PlayState.Leaving));
                Assert.That(game.Round, Is.Null);
                Assert.That(game.LastShot, Is.Null);
                Assert.That(game.Presentation.IsPlaying, Is.False);
                Assert.That(GameManager.Instance.HasModeSelection || GameManager.Instance.IsLocalMultiplayer, Is.False);
                Assert.That(AudioManager.Instance.StadiumActive, Is.False);
                yield return WaitForHome();
                yield return new WaitForSecondsRealtime(.3f);
                Assert.That(cueCount, Is.Zero);
            }
            yield return Open(GameMode.FixedRound);
            Assert.That(Object.FindFirstObjectByType<GameplayController>().Round.ShotsTaken, Is.Zero);
        }

        private static IEnumerator WaitForHome()
        {
            var deadline = Time.realtimeSinceStartup + 10;
            while (SceneManager.GetActiveScene().name != "MainMenu" || SceneTransition.IsBusy)
            { Assert.That(Time.realtimeSinceStartup, Is.LessThan(deadline)); yield return null; }
            yield return null;
        }
    }
}
