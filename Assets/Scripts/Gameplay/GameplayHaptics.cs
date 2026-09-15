using System;
using UnityEngine;

namespace PenaltyKing
{
    /// <summary>
    /// Platform-agnostic haptics service interface.
    /// Abstracts Android/iOS vibration implementations for testability and cross-platform support.
    /// </summary>
    public interface IHapticsService
    {
        bool IsSupported { get; }
        void Pulse(long milliseconds);
    }

    /// <summary>
    /// Android-specific haptics implementation using Unity's AndroidJava API.
    /// </summary>
    public sealed class AndroidHaptics : IHapticsService
    {
        public bool IsSupported
        {
            get
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                try
                {
                    using (var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                    using (var activity = player.GetStatic<AndroidJavaObject>("currentActivity"))
                    using (var vibrator = activity.Call<AndroidJavaObject>("getSystemService", "vibrator"))
                    {
                        return vibrator != null && vibrator.Call<bool>("hasVibrator");
                    }
                }
                catch (AndroidJavaException)
                {
                    return false;
                }
#else
                return false;
#endif
            }
        }

        public void Pulse(long milliseconds)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                using (var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                using (var activity = player.GetStatic<AndroidJavaObject>("currentActivity"))
                using (var vibrator = activity.Call<AndroidJavaObject>("getSystemService", "vibrator"))
                {
                    if (vibrator == null || !vibrator.Call<bool>("hasVibrator"))
                        return;

                    using (var effectClass = new AndroidJavaClass("android.os.VibrationEffect"))
                    using (var effect = effectClass.CallStatic<AndroidJavaObject>("createOneShot", milliseconds, -1))
                    {
                        vibrator.Call("vibrate", effect);
                    }
                }
            }
            catch (AndroidJavaException exception)
            {
                UnityEngine.Debug.LogWarning($"Haptic unavailable: {exception.Message}");
            }
#endif
        }
    }

    /// <summary>
    /// iOS-specific haptics implementation using Unity's Handheld API.
    /// Falls back to no-op on unsupported platforms.
    /// </summary>
    public sealed class IOShaptics : IHapticsService
    {
        public bool IsSupported => Application.platform == RuntimePlatform.IPhonePlayer;

        public void Pulse(long milliseconds)
        {
#if UNITY_IOS && !UNITY_EDITOR
            // iOS uses discrete haptic patterns rather than timed pulses
            // Map duration to appropriate system haptic type
            if (milliseconds <= 20)
                Handheld.SetiOSShakeToShake(); // Light impact
            else if (milliseconds <= 50)
                Handheld.PlayHandheldWithImpactFeedback(ImpactFeedback.Medium);
            else
                Handheld.PlayHandheldWithImpactFeedback(ImpactFeedback.Heavy);
#endif
        }
    }

    /// <summary>
    /// No-op haptics implementation for platforms without vibration support.
    /// </summary>
    public sealed class NoOpHaptics : IHapticsService
    {
        public bool IsSupported => false;
        public void Pulse(long milliseconds) { /* No-op */ }
    }

    /// <summary>
    /// Haptics service locator providing platform-appropriate implementation.
    /// Thread-safe singleton pattern with lazy initialization.
    /// </summary>
    public static class HapticsServiceLocator
    {
        private static readonly object lockObject = new object();
        private static IHapticsService instance;
        private static bool initialized;

        public static IHapticsService Instance
        {
            get
            {
                if (!initialized)
                {
                    lock (lockObject)
                    {
                        if (!initialized)
                        {
                            instance = CreatePlatformAppropriateInstance();
                            initialized = true;
                        }
                    }
                }
                return instance;
            }
        }

        private static IHapticsService CreatePlatformAppropriateInstance()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            var android = new AndroidHaptics();
            if (android.IsSupported) return android;
#endif

#if UNITY_IOS && !UNITY_EDITOR
            var ios = new IOShaptics();
            if (ios.IsSupported) return ios;
#endif

            return new NoOpHaptics();
        }

        /// <summary>
        /// Override the default haptics implementation (useful for testing).
        /// </summary>
        public static void SetInstance(IHapticsService customImplementation)
        {
            lock (lockObject)
            {
                instance = customImplementation;
                initialized = true;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            lock (lockObject)
            {
                instance = null;
                initialized = false;
            }
        }
    }

    [DisallowMultipleComponent, RequireComponent(typeof(ShotPresentation))]
    public sealed class GameplayHaptics : MonoBehaviour
    {
        public const long PulseMilliseconds = 65;
        public event Action<long> PulseRequested;
        private ShotPresentation presentation;
        private bool resolved;
        private readonly IHapticsService haptics = HapticsServiceLocator.Instance;

        private void OnEnable()
        {
            presentation = GetComponent<ShotPresentation>();
            presentation.Kick += OnKick;
            presentation.Impact += OnImpact;
        }

        private void OnKick() => resolved = false;

        private void OnImpact(ShotResult shot)
        {
            if (resolved) return;
            resolved = true;

            if (shot.Outcome != ShotOutcome.Goal || !GameManager.Instance.VibrationEnabled)
                return;

            PulseRequested?.Invoke(PulseMilliseconds);

            if (haptics.IsSupported)
            {
                haptics.Pulse(PulseMilliseconds);
            }
        }

        private void OnDisable()
        {
            if (presentation != null)
            {
                presentation.Kick -= OnKick;
                presentation.Impact -= OnImpact;
            }
        }
    }
}
