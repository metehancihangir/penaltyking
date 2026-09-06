using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace PenaltyKing.Tests
{
    public sealed class GameplayTests
    {
        private Difficulty oldDifficulty;
        private GameMode oldMode;

        [SetUp]
        public void RememberSelections()
        {
            RuntimeBootstrap.EnsureServices();
            oldDifficulty = GameManager.Instance.SelectedDifficulty;
            oldMode = GameManager.Instance.SelectedMode;
        }

        [UnityTearDown]
        public IEnumerator RestoreSelections()
        {
            yield return SceneManager.LoadSceneAsync("MainMenu");
            GameManager.Instance.SelectDifficulty(oldDifficulty);
            GameManager.Instance.SelectMode(oldMode);
            GameManager.Instance.BeginSelection();
        }

        private static IEnumerator StartGame(GameMode mode, float probability)
        {
            var state = GameManager.Instance;
            var backup = JsonUtility.ToJson(state.Rules);
            try
            {
                var json = "{\"mediumSaveProbability\":" + probability.ToString(System.Globalization.CultureInfo.InvariantCulture) + ",\"fixedRoundShots\":5}";
                JsonUtility.FromJsonOverwrite(json, state.Rules);
                state.SelectDifficulty(Difficulty.Medium);
                state.SelectMode(mode);
            }
            finally { JsonUtility.FromJsonOverwrite(backup, state.Rules); }
            yield return SceneManager.LoadSceneAsync("Gameplay");
            yield return null;
        }

        private static PointerEventData Pointer(Button button)
        {
            Canvas.ForceUpdateCanvases();
            var canvas = button.GetComponentInParent<Canvas>();
            var pointer = new PointerEventData(EventSystem.current)
            {
                position = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, button.transform.position),
                button = PointerEventData.InputButton.Left
            };
            var hits = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer, hits);
            Assert.That(hits, Is.Not.Empty);
            Assert.That(hits[0].gameObject.GetComponentInParent<Button>(), Is.SameAs(button));
            return pointer;
        }

        private static void Tap(ShotZone zone)
        {
            var pointer = Pointer(zone);
            zone.OnPointerDown(pointer);
            zone.OnPointerUp(pointer);
            zone.OnPointerClick(pointer); // Release must NOT create a second shot.
        }

        private static IEnumerator WaitForResult(GameplayController game)
        {
            var deadline = Time.realtimeSinceStartup + 4;
            while (game.State == PlayState.ShowingShot)
            {
                Assert.That(Time.realtimeSinceStartup, Is.LessThan(deadline));
                yield return null;
            }
            // Newly activated summary graphics join the canvas/raycast registry on the next frame.
            yield return null;
        }

        [UnityTest]
        public IEnumerator FixedRoundScoresFiveShotsBlocksDoubleTapsAndReplaysCleanly()
        {
            yield return StartGame(GameMode.FixedRound, .35f);
            var game = Object.FindFirstObjectByType<GameplayController>();
            Assert.That(Object.FindFirstObjectByType<PhaseZeroDiagnostics>(), Is.Null);
            Assert.That(Object.FindObjectsByType<ShotZone>(FindObjectsSortMode.None), Has.Length.EqualTo(3));
            Assert.That(Object.FindObjectsByType<Slider>(FindObjectsSortMode.None), Is.Empty);
            var goals = 0;
            for (var i = 0; i < 5; i++)
            {
                var zone = i % 3 == 0 ? game.Left : i % 3 == 1 ? game.Center : game.Right;
                Tap(zone);
                Assert.That(game.State, Is.EqualTo(PlayState.ShowingShot));
                Assert.That(game.LastShot.Value.PlayerDirection, Is.EqualTo((ShotDirection)(i % 3)));
                game.Right.OnPointerDown(new PointerEventData(EventSystem.current) { button = PointerEventData.InputButton.Left });
                Assert.That(game.Round.ShotsTaken, Is.EqualTo(i + 1));
                if (game.LastShot.Value.Outcome == ShotOutcome.Goal) goals++;
                Assert.That(game.Round.Goals, Is.EqualTo(goals));
                Assert.That(game.Round.SaveProbability, Is.EqualTo(.35f));
                yield return WaitForResult(game);
                Assert.That(game.ScoreLabel.text, Is.EqualTo($"GOL  {goals}"));
                Assert.That(game.ShotCounter.text, Is.EqualTo($"ŞUT  {i + 1} / 5"));
            }
            Assert.That(game.State, Is.EqualTo(PlayState.Finished));
            Assert.That(game.ResultVisible, Is.True);
            Assert.That(game.ResultScore.text, Is.EqualTo($"{goals} / 5 GOL"));
            game.Left.OnPointerDown(new PointerEventData(EventSystem.current));
            Assert.That(game.Round.ShotsTaken, Is.EqualTo(5));
            game.Replay.OnPointerClick(Pointer(game.Replay));
            Assert.That(game.State, Is.EqualTo(PlayState.Ready));
            Assert.That(game.Round.Goals, Is.Zero);
            Assert.That(game.Round.ShotsTaken, Is.Zero);
            Assert.That(game.LastShot, Is.Null);
            Assert.That(game.ResultVisible, Is.False);
            Assert.That(game.Round.SaveProbability, Is.EqualTo(.35f));
            Tap(game.Center);
            Assert.That(game.Round.ShotsTaken, Is.EqualTo(1));
            yield return WaitForResult(game);
        }

        [UnityTest]
        public IEnumerator EndlessFirstSaveShowsZeroScoreAndHomeReturnsToMenu()
        {
            yield return StartGame(GameMode.Endless, 1f);
            var game = Object.FindFirstObjectByType<GameplayController>();
            Tap(game.Right);
            Assert.That(game.LastShot.Value.KeeperDirection, Is.EqualTo(ShotDirection.Right));
            yield return WaitForResult(game);
            Assert.That(game.Round.IsOver, Is.True);
            Assert.That(game.Round.Goals, Is.Zero);
            Assert.That(game.ResultScore.text, Is.EqualTo("0 GOL"));
            game.Home.OnPointerClick(Pointer(game.Home));
            var deadline = Time.realtimeSinceStartup + 10;
            while (SceneManager.GetActiveScene().name != "MainMenu")
            {
                Assert.That(Time.realtimeSinceStartup, Is.LessThan(deadline));
                yield return null;
            }
        }

        [UnityTest]
        public IEnumerator EndlessGoalsContinueBeyondFixedRoundShotCount()
        {
            yield return StartGame(GameMode.Endless, 0f);
            var game = Object.FindFirstObjectByType<GameplayController>();
            for (var i = 0; i < 6; i++)
            {
                Tap(game.Left);
                Assert.That(game.LastShot.Value.KeeperDirection, Is.Not.EqualTo(ShotDirection.Left));
                yield return WaitForResult(game);
                Assert.That(game.State, Is.EqualTo(PlayState.Ready));
                Assert.That(game.Round.Goals, Is.EqualTo(i + 1));
                Assert.That(game.ResultVisible, Is.False);
            }
            Assert.That(game.ShotCounter.text, Is.EqualTo("ŞUT  6"));
        }

        [UnityTest]
        public IEnumerator TouchDeviceCommitsShotOnPressAndReleaseDoesNotShootAgain()
        {
            yield return StartGame(GameMode.FixedRound, 0f);
            var game = Object.FindFirstObjectByType<GameplayController>();
            var point = Pointer(game.Right).position;
            var oldBackground = InputSystem.settings.backgroundBehavior;
#if UNITY_EDITOR
            var oldEditor = InputSystem.settings.editorInputBehaviorInPlayMode;
            InputSystem.settings.editorInputBehaviorInPlayMode = InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
#endif
            InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
            var screen = InputSystem.AddDevice<Touchscreen>();
            try
            {
                InputSystem.QueueStateEvent(screen, new TouchState { touchId = 77, phase = UnityEngine.InputSystem.TouchPhase.Began, position = point });
                InputSystem.Update();
                yield return null;
                yield return null;
                Assert.That(game.Round.ShotsTaken, Is.EqualTo(1));
                Assert.That(game.LastShot.Value.PlayerDirection, Is.EqualTo(ShotDirection.Right));
                InputSystem.QueueStateEvent(screen, new TouchState { touchId = 77, phase = UnityEngine.InputSystem.TouchPhase.Ended, position = point });
                InputSystem.Update();
                yield return null;
                yield return null;
                Assert.That(game.Round.ShotsTaken, Is.EqualTo(1));
            }
            finally
            {
                InputSystem.RemoveDevice(screen);
                InputSystem.settings.backgroundBehavior = oldBackground;
#if UNITY_EDITOR
                InputSystem.settings.editorInputBehaviorInPlayMode = oldEditor;
#endif
            }
            yield return WaitForResult(game);
        }

        [UnityTest]
        public IEnumerator GameplayWithoutSelectionsCannotShoot()
        {
            GameManager.Instance.BeginSelection();
            yield return SceneManager.LoadSceneAsync("Gameplay");
            yield return null;
            var game = Object.FindFirstObjectByType<GameplayController>();
            Assert.That(game.State, Is.EqualTo(PlayState.NeedsSelection));
            Assert.That(game.Round, Is.Null);
            Assert.That(game.Left.interactable || game.Center.interactable || game.Right.interactable, Is.False);
            Assert.That(game.Home.gameObject.activeInHierarchy, Is.True);
        }
    }
}
