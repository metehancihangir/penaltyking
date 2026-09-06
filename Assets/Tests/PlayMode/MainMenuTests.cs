using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace PenaltyKing.Tests
{
    public sealed class MainMenuTests
    {
        private static PointerEventData Pointer(PixelMenuButton button)
        {
            Canvas.ForceUpdateCanvases();
            var canvas = button.GetComponentInParent<Canvas>();
            var position = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, button.transform.position);
            return new PointerEventData(EventSystem.current) { position = position, button = PointerEventData.InputButton.Left };
        }

        private static void ClickThroughRaycast(PixelMenuButton button)
        {
            var data = Pointer(button);
            var hits = new List<RaycastResult>();
            EventSystem.current.RaycastAll(data, hits);
            Assert.That(hits, Is.Not.Empty, "Button must be reachable by UI raycasting");
            Assert.That(hits[0].gameObject.GetComponentInParent<PixelMenuButton>(), Is.SameAs(button));
            ExecuteEvents.ExecuteHierarchy(hits[0].gameObject, data, ExecuteEvents.pointerClickHandler);
        }

        [UnityTest]
        public IEnumerator ActiveButtonsNavigateToTheirExactDestinations()
        {
            foreach (var destination in new[] { GameScene.DifficultySelect, GameScene.Options })
            {
                yield return SceneManager.LoadSceneAsync("MainMenu");
                yield return null;
                var menu = Object.FindFirstObjectByType<MainMenuController>();
                Assert.That(menu, Is.Not.Null);
                Assert.That(Object.FindObjectsByType<PixelMenuButton>(FindObjectsSortMode.None), Has.Length.EqualTo(3));
                Assert.That(EventSystem.current.GetComponent<InputSystemUIInputModule>(), Is.Not.Null);
                Assert.That(Object.FindFirstObjectByType<PhaseZeroDiagnostics>(), Is.Null);
                var button = destination == GameScene.DifficultySelect ? menu.Singleplayer : menu.Options;
                Assert.That(button.interactable, Is.True);
                ClickThroughRaycast(button);
                var deadline = Time.realtimeSinceStartup + 10f;
                while (SceneManager.GetActiveScene().name != destination.ToString() || SceneTransition.IsBusy)
                {
                    Assert.That(Time.realtimeSinceStartup, Is.LessThan(deadline));
                    yield return null;
                }
            }
        }

        [UnityTest]
        public IEnumerator MultiplayerIsVisibleDisabledAndCannotNavigate()
        {
            yield return SceneManager.LoadSceneAsync("MainMenu");
            yield return null;
            var menu = Object.FindFirstObjectByType<MainMenuController>();
            Assert.That(menu.Multiplayer.gameObject.activeInHierarchy, Is.True);
            Assert.That(menu.Multiplayer.interactable, Is.False);
            var label = menu.Multiplayer.transform.Find("Face/Coming Soon Badge/Coming Soon").GetComponent<Text>();
            Assert.That(label.text, Is.EqualTo("YAKINDA"));
            ClickThroughRaycast(menu.Multiplayer);
            yield return null;
            yield return null;
            Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo("MainMenu"));
            Assert.That(menu.GetComponent<SceneNavigator>().IsLoading, Is.False);
        }

        [UnityTest]
        public IEnumerator ButtonHoverAndPressHaveDistinctFeedbackAndReleaseRestoresFace()
        {
            yield return SceneManager.LoadSceneAsync("MainMenu");
            yield return null;
            var button = Object.FindFirstObjectByType<MainMenuController>().Singleplayer;
            var data = Pointer(button);
            var normal = button.targetGraphic.canvasRenderer.GetColor();
            button.OnPointerEnter(data);
            var highlighted = button.targetGraphic.canvasRenderer.GetColor();
            Assert.That(highlighted, Is.Not.EqualTo(normal));
            button.OnPointerDown(data);
            Assert.That(button.targetGraphic.canvasRenderer.GetColor(), Is.Not.EqualTo(highlighted));
            Assert.That(button.Face.anchoredPosition.y, Is.EqualTo(-4));
            button.OnPointerUp(data);
            Assert.That(button.Face.anchoredPosition, Is.EqualTo(Vector2.zero));
            button.OnPointerExit(data);
            Assert.That(button.targetGraphic.canvasRenderer.GetColor(), Is.EqualTo(normal));
        }
    }
}
