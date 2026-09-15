using System;
using System.Threading;
using UnityEngine;

namespace PenaltyKing
{
    public enum GameMode { FixedRound, Endless }

    // Session selections plus volume preferences loaded before audio services start.
    [DisallowMultipleComponent]
    public sealed class GameManager : MonoBehaviour
    {
        private static readonly object lockObject = new object();
        private static GameManager instance;
        private static int isDestroyed = 0;

        public static GameManager Instance
        {
            get
            {
                if (Volatile.Read(ref isDestroyed) != 0)
                    return null;

                if (instance == null)
                {
                    lock (lockObject)
                    {
                        if (instance != null)
                            return instance;
                        // Instance will be set by Awake - return null if not yet initialized
                        return null;
                    }
                }

                return instance;
            }
            private set
            {
                if (Volatile.Read(ref isDestroyed) != 0)
                    return;

                lock (lockObject)
                {
                    if (instance != null && instance != value)
                    {
                        Destroy(value);
                        return;
                    }
                    instance = value;
                }
            }
        }

        public static bool IsCreated => instance != null && Volatile.Read(ref isDestroyed) == 0;
        public GameMode SelectedMode { get; private set; } = GameMode.FixedRound;
        public GameRules Rules { get; private set; }
        public bool IsLocalMultiplayer { get; private set; }
        public bool HasModeSelection { get; private set; }
        public int SelectedRoundShots { get; private set; }
        public float MusicVolume { get; private set; } = 0.7f;
        public float SfxVolume { get; private set; } = 0.8f;
        public event Action<float, float> VolumeChanged;
        public bool VibrationEnabled { get; private set; }
        public event Action<bool> VibrationChanged;
        public const string VibrationKey = "PenaltyKing.Vibration.Enabled.v1";

        public void SetVibrationEnabled(bool enabled)
        {
            VibrationEnabled = enabled;
            PlayerPrefs.SetInt(VibrationKey, enabled ? 1 : 0);
            PlayerPrefs.Save();
            VibrationChanged?.Invoke(enabled);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            lock (lockObject)
            {
                instance = null;
                Volatile.Write(ref isDestroyed, 0);
            }
        }

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Rules = Resources.Load<GameRules>("GameRules");
            if (Rules == null) throw new InvalidOperationException("Missing Resources/GameRules asset.");
            SelectedRoundShots = Rules.FixedRoundShots;
            AudioPreferences.Load(out var music, out var sfx);
            MusicVolume = music;
            SfxVolume = sfx;
            VibrationEnabled = PlayerPrefs.GetInt(VibrationKey, 1) != 0;
        }

        public void BeginSelection()
        {
            IsLocalMultiplayer = false;
            HasModeSelection = false;
        }

        public void SelectLocalMultiplayer()
        {
            BeginSelection();
            IsLocalMultiplayer = true;
        }

        public void SelectMode(GameMode mode)
        {
            if (!IsLocalMultiplayer) throw new InvalidOperationException("Choose players before mode.");
            if (!Enum.IsDefined(typeof(GameMode), mode)) throw new ArgumentOutOfRangeException(nameof(mode));
            SelectedMode = mode;
            SelectedRoundShots = Rules.FixedRoundShots;
            HasModeSelection = true;
        }

        public void SetMusicVolume(float value)
        {
            MusicVolume = Mathf.Clamp01(value);
            VolumeChanged?.Invoke(MusicVolume, SfxVolume);
        }

        public void SetSfxVolume(float value)
        {
            SfxVolume = Mathf.Clamp01(value);
            VolumeChanged?.Invoke(MusicVolume, SfxVolume);
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                Volatile.Write(ref isDestroyed, 1);
                instance = null;
            }
        }
    }
}
