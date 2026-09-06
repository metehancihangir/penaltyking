using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PenaltyKing
{
    // A shot commits on touch DOWN, without waiting for release or adding aiming.
    public sealed class ShotZone : Button
    {
        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            if (eventData.button == PointerEventData.InputButton.Left && IsActive() && IsInteractable())
                onClick.Invoke();
        }
        public override void OnPointerClick(PointerEventData eventData) { }
        public override void OnSubmit(BaseEventData eventData) { }
    }
}
