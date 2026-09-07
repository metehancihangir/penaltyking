using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace PenaltyKing.Tests
{
    public static class LocalTestInput
    {
        public static PointerEventData Pointer(Component target)
        {
            Canvas.ForceUpdateCanvases();
            return new PointerEventData(EventSystem.current) { pointerId = -1, button = PointerEventData.InputButton.Left,
                position = RectTransformUtility.WorldToScreenPoint(target.GetComponentInParent<Canvas>().worldCamera, target.transform.position) };
        }
        public static void Press(Component target)
        {
            var pointer = Pointer(target);
            var hits = new List<RaycastResult>(); EventSystem.current.RaycastAll(pointer, hits);
            Assert.That(hits, Is.Not.Empty);
            Assert.That(hits[0].gameObject == target.gameObject || hits[0].gameObject.transform.IsChildOf(target.transform), Is.True, target.name);
            ExecuteEvents.ExecuteHierarchy(hits[0].gameObject, pointer, ExecuteEvents.pointerDownHandler);
            ExecuteEvents.ExecuteHierarchy(hits[0].gameObject, pointer, ExecuteEvents.pointerUpHandler);
            ExecuteEvents.ExecuteHierarchy(hits[0].gameObject, pointer, ExecuteEvents.pointerClickHandler);
        }
        public static IEnumerator Continue(GameplayController game)
        {
            yield return null; yield return null;
            Assert.That(game.Handoff.Visible, Is.True);
            Press(game.Handoff);
            Assert.That(game.Handoff.Visible, Is.False);
        }
        public static IEnumerator Choose(GameplayController game, ShotZone shooter, ShotZone keeper)
        {
            if (game.Handoff.Visible) yield return Continue(game);
            Press(shooter);
            Assert.That(game.State, Is.EqualTo(PlayState.PassingPhone));
            yield return Continue(game);
            Press(keeper);
            Assert.That(game.State, Is.EqualTo(PlayState.ShowingShot));
        }
        public static IEnumerator Finish(GameplayController game)
        {
            var deadline = Time.realtimeSinceStartup + 5;
            while (game.State == PlayState.ShowingShot)
            { Assert.That(Time.realtimeSinceStartup, Is.LessThan(deadline)); yield return null; }
        }
    }
}
