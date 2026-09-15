using UnityEngine;

namespace PenaltyKing
{
    /// <summary>
    /// Async-safe audio preferences persistence with shutdown guarantee.
    /// Uses PlayerPrefs with explicit save calls and application lifecycle hooks.
    /// </summary>
    public static class AudioPreferences
    {
        public const string MusicKey = "PenaltyKing.Audio.MusicVolume.v1";
        public const string SfxKey = "PenaltyKing.Audio.SfxVolume.v1";

        private static bool savePending;
        private static float pendingMusic;
        private static float pendingSfx;
        private static readonly object lockObject = new object();

        public static void Load(out float music, out float sfx)
        {
            lock (lockObject)
            {
                music = Mathf.Clamp01(PlayerPrefs.GetFloat(MusicKey, 0.7f));
                sfx = Mathf.Clamp01(PlayerPrefs.GetFloat(SfxKey, 0.8f));
            }
        }

        public static void Save(float music, float sfx)
        {
            lock (lockObject)
            {
                PlayerPrefs.SetFloat(MusicKey, Mathf.Clamp01(music));
                PlayerPrefs.SetFloat(SfxKey, Mathf.Clamp01(sfx));
                PlayerPrefs.Save();
                savePending = false;
            }
        }

        public static void ScheduleSave(float music, float sfx)
        {
            lock (lockObject)
            {
                pendingMusic = Mathf.Clamp01(music);
                pendingSfx = Mathf.Clamp01(sfx);
                savePending = true;
            }
        }

        public static void Flush()
        {
            lock (lockObject)
            {
                if (savePending)
                {
                    PlayerPrefs.SetFloat(MusicKey, pendingMusic);
                    PlayerPrefs.SetFloat(SfxKey, pendingSfx);
                    PlayerPrefs.Save();
                    savePending = false;
                }
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeLifecycleHooks()
        {
            Application.quitting += OnApplicationQuitting;
            Application.pauseStateChanged += OnPauseStateChanged;
        }

        private static void OnApplicationQuitting()
        {
            Flush();
        }

        private static void OnPauseStateChanged(PauseState state)
        {
            if (state == PauseState.Paused)
            {
                Flush();
            }
        }
    }
}
