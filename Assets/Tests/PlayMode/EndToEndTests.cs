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
        public IEnumerator MenuStartsLocalMatchAndTenHumanShotsReachSummary()
        {
            RuntimeBootstrap.EnsureServices();
            yield return SceneManager.LoadSceneAsync("MainMenu"); yield return null;
            Press(Object.FindFirstObjectByType<MainMenuController>().Play);
            yield return SceneReady("PlayerSelect");
            Press(Object.FindFirstObjectByType<PlayerSelectController>().Local);
            yield return SceneReady("ModeSelect");
            Press(Object.FindFirstObjectByType<ModeSelectController>().FixedRound);
            yield return SceneReady("Gameplay");
            var game = Object.FindFirstObjectByType<GameplayController>();
            for (var i = 0; i < 10; i++)
            {
                yield return LocalTestInput.Choose(game, game.Left, game.Left);
                yield return LocalTestInput.Finish(game);
            }
            Assert.That(game.ResultVisible, Is.True);
            Assert.That(game.ResultTitle.text, Is.EqualTo("BERABERE"));
            Assert.That(game.ResultScore.text, Is.EqualTo("0  -  0"));
            yield return null; // Let the newly activated summary register with the UI raycaster.
            Press(game.Replay);
            Assert.That(game.Round.ShotsTaken, Is.Zero);
            Assert.That(game.Round.ShooterPlayer, Is.EqualTo(1));
            Assert.That(game.Handoff.Visible, Is.True);
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
