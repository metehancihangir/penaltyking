using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PenaltyKing
{
    public sealed class TouchInputProbe : MonoBehaviour
    {
        public event Action<Vector2> Pressed;
        public Vector2 LastPosition { get; private set; }
        public int PressCount { get; private set; }
        private InputAction press;

        private void Awake()
        {
            press = new InputAction("Primary press", InputActionType.Button);
            press.AddBinding("<Touchscreen>/primaryTouch/press");
#if UNITY_EDITOR || UNITY_STANDALONE
            // A mouse is an editor/desktop test aid; mobile uses touch.
            press.AddBinding("<Mouse>/leftButton");
#endif
            press.performed += OnPress;
        }

        private void OnEnable() => press.Enable();
        private void OnDisable() => press.Disable();
        private void OnDestroy() => press.Dispose();

        private void OnPress(InputAction.CallbackContext context)
        {
            if (context.control.device is Touchscreen touchscreen)
                LastPosition = touchscreen.primaryTouch.position.ReadValue();
            else if (context.control.device is Mouse mouse)
                LastPosition = mouse.position.ReadValue();
            else return;
            PressCount++;
            Pressed?.Invoke(LastPosition);
            Debug.Log($"[TouchInputProbe] Press {PressCount}: {LastPosition}", this);
        }
    }
}
