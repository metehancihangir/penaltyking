using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
namespace PenaltyKing.Tests
{
    public sealed class GameplayTests
    {
        [UnitySetUp]
        public IEnumerator Open()
        {
            RuntimeBootstrap.EnsureServices();
            GameManager.Instance.SelectLocalMultiplayer();
            GameManager.Instance.SelectMode(GameMode.Endless);
            yield return SceneManager.LoadSceneAsync("Gameplay"); yield return null;
        }
        [UnityTearDown]
        public IEnumerator Close()
        { yield return SceneManager.LoadSceneAsync("MainMenu"); GameManager.Instance.BeginSelection(); }

        [UnityTest]
        public IEnumerator HiddenShotDoesNotMoveBallOrPlaySoundAndTwoPeopleResolveIt()
        {
            var game = Object.FindFirstObjectByType<GameplayController>();
            yield return LocalTestInput.Continue(game);
            var rest = game.Presentation.BallPosition;
            LocalTestInput.Press(game.Right);
            Assert.That(game.State, Is.EqualTo(PlayState.PassingPhone));
            Assert.That(game.Handoff.Title, Does.Contain("PLAYER 2"));
            Assert.That(game.Handoff.Role, Does.Contain("Kurtarış"));
            Assert.That(game.LastShot, Is.Null);
            Assert.That(game.Presentation.BallPosition, Is.EqualTo(rest));
            Assert.That(game.Presentation.IsPlaying, Is.False);
            Assert.That(AudioManager.Instance.SfxSource.isPlaying, Is.False);
            Assert.That(game.Round.ShotsTaken, Is.Zero);
            // A second direct direction event cannot overwrite the private shot.
            game.Left.OnPointerDown(new PointerEventData(EventSystem.current));
            yield return LocalTestInput.Continue(game);
            Assert.That(game.State, Is.EqualTo(PlayState.ChoosingKeeper));
            Assert.That(game.Round.ShotsTaken, Is.Zero, "Dismissal is not a direction choice");
            LocalTestInput.Press(game.Right);
            Assert.That(game.LastShot.Value.Outcome, Is.EqualTo(ShotOutcome.Save));
            Assert.That(game.Round.Player1.Shots, Is.EqualTo(1));
            game.Center.OnPointerDown(new PointerEventData(EventSystem.current));
            Assert.That(game.Round.ShotsTaken, Is.EqualTo(1));
            yield return LocalTestInput.Finish(game);
            Assert.That(game.Handoff.Title, Does.Contain("PLAYER 2"));
            Assert.That(game.Handoff.Role, Is.EqualTo("Şut sırası"));
            yield return LocalTestInput.Choose(game, game.Left, game.Right);
            Assert.That(game.Round.Player2.Goals, Is.EqualTo(1));
            Assert.That(game.Round.Player1.Goals, Is.Zero);
            yield return LocalTestInput.Finish(game);
            Assert.That(game.Handoff.Title, Does.Contain("PLAYER 1"));
        }

        [UnityTest]
        public IEnumerator OverlayRejectsOldClickAndMismatchedPointer()
        {
            var game = Object.FindFirstObjectByType<GameplayController>();
            yield return LocalTestInput.Continue(game);
            LocalTestInput.Press(game.Left);
            var old = new PointerEventData(EventSystem.current) { pointerId = 1 };
            game.Handoff.OnPointerClick(old);
            Assert.That(game.Handoff.Visible, Is.True);
            game.Handoff.OnPointerDown(old);
            game.Handoff.OnPointerClick(old);
            Assert.That(game.Handoff.Visible, Is.True, "Same frame as show cannot dismiss");
            yield return null; yield return null;
            game.Handoff.OnPointerDown(old);
            game.Handoff.OnPointerClick(new PointerEventData(EventSystem.current) { pointerId = 2 });
            Assert.That(game.Handoff.Visible, Is.True);
            game.Handoff.OnPointerClick(old);
            Assert.That(game.State, Is.EqualTo(PlayState.ChoosingKeeper));
            Assert.That(game.Round.ShotsTaken, Is.Zero);
        }

        [UnityTest]
        public IEnumerator CanceledGestureAllowsANewPointerToContinue()
        {
            var game = Object.FindFirstObjectByType<GameplayController>();
            yield return null; yield return null;
            game.Handoff.OnPointerDown(new PointerEventData(EventSystem.current) { pointerId = 5 });
            // No click: simulate a canceled gesture, followed by an all-pointers-up frame.
            yield return null; yield return null;
            var fresh = new PointerEventData(EventSystem.current) { pointerId = 6 };
            game.Handoff.OnPointerDown(fresh);
            game.Handoff.OnPointerClick(fresh);
            Assert.That(game.State, Is.EqualTo(PlayState.Ready));
            Assert.That(game.Round.HasShotSelection, Is.False);
        }

        [UnityTest]
        public IEnumerator RealHeldTouchCannotDismissHandoffOnRelease()
        {
            var game = Object.FindFirstObjectByType<GameplayController>();
            yield return LocalTestInput.Continue(game);
            var previousBackground = InputSystem.settings.backgroundBehavior;
#if UNITY_EDITOR
            var previousEditor = InputSystem.settings.editorInputBehaviorInPlayMode;
            InputSystem.settings.editorInputBehaviorInPlayMode = InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
#endif
            InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
            var screen = InputSystem.AddDevice<Touchscreen>();
            var position = LocalTestInput.Pointer(game.Left).position;
            try
            {
                InputSystem.QueueStateEvent(screen, new TouchState { touchId = 88, phase = UnityEngine.InputSystem.TouchPhase.Began, position = position });
                InputSystem.Update(); yield return null; yield return null;
                Assert.That(game.Handoff.Visible, Is.True);
                Assert.That(game.Round.HasShotSelection, Is.True);
                InputSystem.QueueStateEvent(screen, new TouchState { touchId = 88, phase = UnityEngine.InputSystem.TouchPhase.Ended, position = position });
                InputSystem.Update(); yield return null; yield return null;
                Assert.That(game.Handoff.Visible, Is.True);
                Assert.That(game.Round.ShotsTaken, Is.Zero);
                InputSystem.QueueStateEvent(screen, new TouchState { touchId = 89, phase = UnityEngine.InputSystem.TouchPhase.Began, position = position });
                InputSystem.Update(); yield return null;
                InputSystem.QueueStateEvent(screen, new TouchState { touchId = 89, phase = UnityEngine.InputSystem.TouchPhase.Ended, position = position });
                InputSystem.Update(); yield return null; yield return null;
                Assert.That(game.Handoff.Visible, Is.False);
                Assert.That(game.State, Is.EqualTo(PlayState.ChoosingKeeper));
                Assert.That(game.Round.ShotsTaken, Is.Zero);
            }
            finally
            {
                InputSystem.RemoveDevice(screen);
                InputSystem.settings.backgroundBehavior = previousBackground;
#if UNITY_EDITOR
                InputSystem.settings.editorInputBehaviorInPlayMode = previousEditor;
#endif
            }
        }

        [UnityTest]
        public IEnumerator MissingSelectionCannotStartAMatch()
        {
            GameManager.Instance.BeginSelection();
            yield return SceneManager.LoadSceneAsync("Gameplay"); yield return null;
            var game = Object.FindFirstObjectByType<GameplayController>();
            Assert.That(game.State, Is.EqualTo(PlayState.NeedsSelection));
            Assert.That(game.Round, Is.Null);
            Assert.That(game.Left.interactable, Is.False);
            Assert.That(game.Handoff.Visible, Is.False);
        }
    }
}
