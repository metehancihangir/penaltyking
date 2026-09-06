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
            {
#if DEVELOPMENT_BUILD
                var touchscreen = UnityEngine.InputSystem.Touchscreen.current;
                if (touchscreen != null)
                {
                    var queueMilliseconds = (UnityEngine.InputSystem.LowLevel.InputState.currentTime - touchscreen.primaryTouch.startTime.ReadValue()) * 1000;
                    UnityEngine.Debug.Log($"[MobileShot] queueMs={queueMilliseconds:F3} frame={UnityEngine.Time.frameCount}");
                }
#endif
                onClick.Invoke();
            }
        }
        public override void OnPointerClick(PointerEventData eventData) { }
        public override void OnSubmit(BaseEventData eventData) { }
    }
}
