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
    public sealed class GameplayArtTests
    {
        [UnityTest]
        public IEnumerator FinalArtKeepsTouchTargetsAndActorsVisibleWhenAspectChanges()
        {
            RuntimeBootstrap.EnsureServices();
            GameManager.Instance.SelectLocalMultiplayer();
            GameManager.Instance.SelectMode(GameMode.FixedRound);
            yield return SceneManager.LoadSceneAsync("Gameplay");
            yield return null;
            var layout = Object.FindFirstObjectByType<GameplayStageLayout>();
            var safe = (RectTransform)layout.transform.parent;
            safe.GetComponent<SafeAreaPanel>().enabled = false;
            safe.anchorMin = safe.anchorMax = new Vector2(.5f, .5f);
            var controller = Object.FindFirstObjectByType<GameplayController>();
            yield return LocalTestInput.Continue(controller);
            foreach (var size in new[] { new Vector2(960, 720), new Vector2(390, 844) })
            {
                safe.sizeDelta = size;
                Canvas.ForceUpdateCanvases(); layout.Fit(); Canvas.ForceUpdateCanvases();
                foreach (var image in layout.GetComponentsInChildren<Image>())
                {
                    if (image.sprite == null) continue;
                    Assert.That(image.sprite.texture.filterMode, Is.EqualTo(FilterMode.Point));
                    Assert.That(image.raycastTarget, Is.False, image.name + " must not intercept shots");
                    var corners = new Vector3[4]; image.rectTransform.GetWorldCorners(corners);
                    foreach (var corner in corners)
                    {
                        var local = safe.InverseTransformPoint(corner);
                        Assert.That(Mathf.Abs(local.x), Is.LessThanOrEqualTo(size.x * .5f + 1), image.name);
                        Assert.That(Mathf.Abs(local.y), Is.LessThanOrEqualTo(size.y * .5f + 1), image.name);
                    }
                }
                foreach (var zone in new[] { controller.Left, controller.Center, controller.Right })
                {
                    var pointer = new PointerEventData(EventSystem.current) { position = RectTransformUtility.WorldToScreenPoint(Camera.main, zone.transform.position) };
                    var hits = new List<RaycastResult>(); EventSystem.current.RaycastAll(pointer, hits);
                    Assert.That(hits, Is.Not.Empty);
                    Assert.That(hits[0].gameObject.GetComponentInParent<ShotZone>(), Is.SameAs(zone));
                }
            }
            Assert.That(Camera.main.orthographic, Is.True);
            yield return SceneManager.LoadSceneAsync("MainMenu");
            GameManager.Instance.BeginSelection();
        }
    }
}
