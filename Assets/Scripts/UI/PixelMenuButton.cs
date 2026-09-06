using UnityEngine;
using UnityEngine.UI;

namespace PenaltyKing
{
    // Move only the face: the touch target stays fixed during a press.
    public sealed class PixelMenuButton : Button
    {
        [SerializeField] private RectTransform face;
        public RectTransform Face { get => face; set => face = value; }

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            base.DoStateTransition(state, instant);
            if (face != null)
                face.anchoredPosition = state == SelectionState.Pressed
                    ? new Vector2(0, -4) : Vector2.zero;
        }
    }
}
