using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace PenaltyKing
{
    // A full-screen input barrier. Only a fresh down/up gesture can dismiss it.
    public sealed class TurnHandoff : MonoBehaviour, IPointerDownHandler, IPointerClickHandler
    {
        [SerializeField] private Text title, role;
        [SerializeField] private RectTransform card;
        private Action continueAction;
        private bool released;
        private int freeFrames;
        private int shownFrame, pressedPointer = int.MinValue;
        private float shownAt;
        public bool InputBlocked { get; set; }
        public bool Visible => gameObject.activeSelf;
        public string Title => title.text;
        public string Role => role.text;
        public void Configure(Text heading, Text description, RectTransform panel)
        { title = heading; role = description; card = panel; }

        public void Show(int player, bool keeper, Action onContinue)
        {
            continueAction = onContinue;
            title.text = $"{(keeper ? "Kurtarış" : "Şut")} Sırası\nPLAYER {player}'de";
            role.text = keeper ? "Kurtarış sırası" : "Şut sırası";
            shownFrame = Time.frameCount;
            shownAt = Time.unscaledTime;
            released = false;
            freeFrames = 0;
            pressedPointer = int.MinValue;
            card.localScale = Vector3.one * .94f;
            gameObject.SetActive(true);
            if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
        }

        private static bool AnyPointerHeld()
        {
            if (Mouse.current != null && Mouse.current.leftButton.isPressed) return true;
            if (Touchscreen.current != null)
                foreach (var touch in Touchscreen.current.touches)
                    if (touch.press.isPressed) return true;
            return false;
        }

        private void Update()
        {
            if (Time.frameCount > shownFrame && !AnyPointerHeld())
            {
                released = true;
                // Recover if the previous gesture was canceled outside the application.
                if (++freeFrames >= 2) pressedPointer = int.MinValue;
            }
            else freeFrames = 0;
            var t = Mathf.Clamp01((Time.unscaledTime - shownAt) / .22f);
            card.localScale = Vector3.one * Mathf.Lerp(.94f, 1, 1 - (1 - t) * (1 - t));
        }

        public void OnPointerDown(PointerEventData data)
        {
            if (InputBlocked || !released || Time.frameCount <= shownFrame || data.button != PointerEventData.InputButton.Left) return;
            freeFrames = 0;
            if (pressedPointer == int.MinValue) pressedPointer = data.pointerId;
        }

        public void OnPointerClick(PointerEventData data)
        {
            if (InputBlocked || pressedPointer != data.pointerId || data.button != PointerEventData.InputButton.Left) return;
            var action = continueAction;
            Hide();
            action?.Invoke();
        }

        public void Hide()
        {
            continueAction = null;
            pressedPointer = int.MinValue;
            gameObject.SetActive(false);
        }
    }
}
