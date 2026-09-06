using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace PenaltyKing.Tests
{
    public sealed class EndToEndTests
    {
        private static IEnumerator SceneReady(string name)
        {
            var deadline = Time.realtimeSinceStartup + 12;
            while (SceneManager.GetActiveScene().name != name || SceneTransition.IsBusy)
            { Assert.That(Time.realtimeSinceStartup, Is.LessThan(deadline)); yield return null; }
            yield return null;
        }
        private static void Press(Button button)
        {
            Canvas.ForceUpdateCanvases();
            var canvas = button.GetComponentInParent<Canvas>();
            var pointer = new PointerEventData(EventSystem.current) { button = PointerEventData.InputButton.Left,
                position = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, button.transform.position) };
            var hits = new List<RaycastResult>(); EventSystem.current.RaycastAll(pointer, hits);
            Assert.That(hits[0].gameObject.GetComponentInParent<Button>(), Is.SameAs(button));
            ExecuteEvents.ExecuteHierarchy(hits[0].gameObject, pointer, ExecuteEvents.pointerDownHandler);
            ExecuteEvents.ExecuteHierarchy(hits[0].gameObject, pointer, ExecuteEvents.pointerUpHandler);
            ExecuteEvents.ExecuteHierarchy(hits[0].gameObject, pointer, ExecuteEvents.pointerClickHandler);
        }
        [UnityTearDown]
        public IEnumerator CleanUp()
        { Time.timeScale = 1; yield return SceneManager.LoadSceneAsync("MainMenu"); GameManager.Instance.BeginSelection(); }

        [UnityTest]
        public IEnumerator BothModesCompleteFullMenuToSummaryReplayAndHomeFlow()
        {
            RuntimeBootstrap.EnsureServices();
            foreach (var mode in new[] { GameMode.FixedRound, GameMode.Endless })
            {
                yield return SceneManager.LoadSceneAsync("MainMenu"); yield return null;
                var menu = Object.FindFirstObjectByType<MainMenuController>();
                Assert.That(menu.Multiplayer.interactable, Is.False); Press(menu.Singleplayer);
                yield return SceneReady("DifficultySelect");
                var state = GameManager.Instance; var backup = JsonUtility.ToJson(state.Rules);
                try
                {
                    // Deterministic outcome; menu selection still performs the real snapshot.
                    JsonUtility.FromJsonOverwrite("{\"mediumSaveProbability\":1}", state.Rules);
                    Press(Object.FindFirstObjectByType<DifficultySelectController>().Medium);
                }
                finally { JsonUtility.FromJsonOverwrite(backup, state.Rules); }
                yield return SceneReady("ModeSelect");
                var modes = Object.FindFirstObjectByType<ModeSelectController>();
                Press(mode == GameMode.FixedRound ? modes.FixedRound : modes.Endless);
                yield return SceneReady("Gameplay");
                var game = Object.FindFirstObjectByType<GameplayController>();
                var shots = mode == GameMode.FixedRound ? game.Round.ShotLimit : 1;
                for (var i = 0; i < shots; i++)
                {
                    Press(i % 2 == 0 ? game.Left : game.Right);
                    var deadline = Time.realtimeSinceStartup + 4;
                    while (game.State == PlayState.ShowingShot)
                    { Assert.That(Time.realtimeSinceStartup, Is.LessThan(deadline)); yield return null; }
                    yield return null;
                }
                Assert.That(game.ResultVisible, Is.True);
                Assert.That(game.ResultScore.text, Is.EqualTo(mode == GameMode.FixedRound ? "0 / 5 GOL" : "0 GOL"));
                Press(game.Replay); Assert.That(game.State, Is.EqualTo(PlayState.Ready));
                Assert.That(game.Round.Goals, Is.Zero);
                // Return through the visible summary again, never via hidden controls.
                for (var i = 0; i < shots; i++)
                {
                    Press(game.Center);
                    while (game.State == PlayState.ShowingShot) yield return null;
                    yield return null;
                }
                Press(game.Home); yield return SceneReady("MainMenu");
                Assert.That(AudioManager.Instance.StadiumActive, Is.False);
            }
        }

        [UnityTest]
        public IEnumerator FadeBlocksInputAcrossSceneBoundaryAndWorksWithPausedTime()
        {
            RuntimeBootstrap.EnsureServices();
            yield return SceneManager.LoadSceneAsync("MainMenu"); yield return null;
            Time.timeScale = 0;
            Press(Object.FindFirstObjectByType<MainMenuController>().Options);
            Assert.That(SceneTransition.IsBusy, Is.True);
            yield return new WaitForSecondsRealtime(.08f);
            Assert.That(SceneTransition.Instance.Opacity, Is.GreaterThan(0).And.LessThan(1));
            var hits = new List<RaycastResult>();
            EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current) { position = new Vector2(Screen.width / 2f, Screen.height / 2f) }, hits);
            Assert.That(hits[0].gameObject.name, Is.EqualTo("Transition Shade"));
            yield return SceneReady("Options");
            Assert.That(SceneTransition.Instance.Opacity, Is.Zero);
            Assert.That(Object.FindObjectsByType<Slider>(FindObjectsSortMode.None).Length, Is.EqualTo(2));
            Press(Object.FindFirstObjectByType<OptionsController>().Back);
            yield return SceneReady("MainMenu");
        }
    }
}
