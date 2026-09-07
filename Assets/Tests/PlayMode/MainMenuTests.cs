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
            foreach (var destination in new[] { GameScene.PlayerSelect, GameScene.Options })
            {
                yield return SceneManager.LoadSceneAsync("MainMenu");
                yield return null;
                var menu = Object.FindFirstObjectByType<MainMenuController>();
                Assert.That(menu, Is.Not.Null);
                Assert.That(Object.FindObjectsByType<PixelMenuButton>(FindObjectsSortMode.None), Has.Length.EqualTo(2));
                Assert.That(EventSystem.current.GetComponent<InputSystemUIInputModule>(), Is.Not.Null);
                Assert.That(Object.FindFirstObjectByType<PhaseZeroDiagnostics>(), Is.Null);
                var button = destination == GameScene.PlayerSelect ? menu.Play : menu.Options;
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
        public IEnumerator OnlineIsVisibleDisabledAndCannotNavigate()
        {
            yield return SceneManager.LoadSceneAsync("PlayerSelect");
            yield return null;
            var screen = Object.FindFirstObjectByType<PlayerSelectController>();
            Assert.That(screen.Online.gameObject.activeInHierarchy, Is.True);
            Assert.That(screen.Online.interactable, Is.False);
            Assert.That(screen.Online.transform.Find("Face/Coming Soon Badge"), Is.Null);
            ClickThroughRaycast(screen.Online);
            yield return null;
            Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo("PlayerSelect"));
            Assert.That(SceneTransition.IsBusy, Is.False);
        }

        [UnityTest]
        public IEnumerator ButtonHoverAndPressHaveDistinctFeedbackAndReleaseRestoresFace()
        {
            yield return SceneManager.LoadSceneAsync("MainMenu");
            yield return null;
            var button = Object.FindFirstObjectByType<MainMenuController>().Play;
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
