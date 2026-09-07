using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace PenaltyKing.Tests
{
    public sealed class SelectionTests
    {
        [UnitySetUp]
        public IEnumerator SetUp()
        {
            RuntimeBootstrap.EnsureServices();
            GameManager.Instance.BeginSelection();
            yield return SceneManager.LoadSceneAsync("MainMenu");
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            yield return SceneManager.LoadSceneAsync("MainMenu");
            GameManager.Instance.BeginSelection();
        }

        private static void Click(PixelMenuButton button)
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
            Assert.That(hits[0].gameObject.GetComponentInParent<PixelMenuButton>(), Is.SameAs(button));
            ExecuteEvents.ExecuteHierarchy(hits[0].gameObject, pointer, ExecuteEvents.pointerClickHandler);
        }

        private static IEnumerator WaitForScene(string scene)
        {
            var deadline = Time.realtimeSinceStartup + 10f;
            while (SceneManager.GetActiveScene().name != scene || SceneTransition.IsBusy)
            {
                Assert.That(Time.realtimeSinceStartup, Is.LessThan(deadline));
                yield return null;
            }
            yield return null;
        }

        [UnityTest]
        public IEnumerator BothModesAcceptLocalSelectionWithoutDifficultyOrStartingABot()
        {
            foreach (var mode in new[] { GameMode.FixedRound, GameMode.Endless })
            {
                yield return SceneManager.LoadSceneAsync("MainMenu"); yield return null;
                Click(Object.FindFirstObjectByType<MainMenuController>().Play);
                yield return WaitForScene("PlayerSelect");
                Click(Object.FindFirstObjectByType<PlayerSelectController>().Local);
                yield return WaitForScene("ModeSelect");
                var modes = Object.FindFirstObjectByType<ModeSelectController>();
                Assert.That(GameManager.Instance.IsLocalMultiplayer, Is.True);
                Click(mode == GameMode.FixedRound ? modes.FixedRound : modes.Endless);
                yield return WaitForScene("Gameplay");
                Assert.That(GameManager.Instance.SelectedMode, Is.EqualTo(mode));
                var game = Object.FindFirstObjectByType<GameplayController>();
                Assert.That(game.Round, Is.Not.Null);
                Assert.That(game.Handoff.Visible, Is.True);
                Assert.That(game.Left.interactable, Is.False);
                game.GetComponent<SceneNavigator>().Navigate(GameScene.MainMenu);
                yield return WaitForScene("MainMenu");
            }
        }

        [UnityTest]
        public IEnumerator BackReturnsThroughPlayerSelectionAndClearsSession()
        {
            Click(Object.FindFirstObjectByType<MainMenuController>().Play);
            yield return WaitForScene("PlayerSelect");
            Click(Object.FindFirstObjectByType<PlayerSelectController>().Local);
            yield return WaitForScene("ModeSelect");
            Click(Object.FindFirstObjectByType<ModeSelectController>().Back);
            yield return WaitForScene("PlayerSelect");
            Assert.That(GameManager.Instance.IsLocalMultiplayer, Is.False);
            Click(Object.FindFirstObjectByType<PlayerSelectController>().Back);
            yield return WaitForScene("MainMenu");
            Assert.That(GameManager.Instance.HasModeSelection, Is.False);
        }

        [UnityTest]
        public IEnumerator DirectModeEntryRequiresPlayerSelectionAndRapidTapsKeepFirstMode()
        {
            yield return SceneManager.LoadSceneAsync("ModeSelect"); yield return null;
            var modes = Object.FindFirstObjectByType<ModeSelectController>();
            Assert.That(modes.FixedRound.interactable || modes.Endless.interactable, Is.False);
            Click(modes.Back);
            yield return WaitForScene("PlayerSelect");
            Click(Object.FindFirstObjectByType<PlayerSelectController>().Local);
            yield return WaitForScene("ModeSelect");
            modes = Object.FindFirstObjectByType<ModeSelectController>();
            Click(modes.Endless);
            modes.FixedRound.onClick.Invoke();
            yield return WaitForScene("Gameplay");
            Assert.That(GameManager.Instance.SelectedMode, Is.EqualTo(GameMode.Endless));
        }
    }
}
