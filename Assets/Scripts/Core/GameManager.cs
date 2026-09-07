using System;
using UnityEngine;

namespace PenaltyKing
{
    public enum Difficulty { Easy, Medium, Hard }
    public enum GameMode { FixedRound, Endless }

    // Session selections plus volume preferences loaded before audio services start.
    [DisallowMultipleComponent]
    public sealed class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public Difficulty SelectedDifficulty { get; private set; } = Difficulty.Medium;
        public GameMode SelectedMode { get; private set; } = GameMode.FixedRound;
        public GameRules Rules { get; private set; }
        public bool HasDifficultySelection { get; private set; }
        public bool IsLocalMultiplayer { get; private set; }
        public bool HasModeSelection { get; private set; }
        public float SelectedSaveProbability { get; private set; }
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
        private static void ResetStatics() => Instance = null;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Rules = Resources.Load<GameRules>("GameRules");
            if (Rules == null) throw new InvalidOperationException("Missing Resources/GameRules asset.");
            SelectedSaveProbability = Rules.SaveProbability(SelectedDifficulty);
            SelectedRoundShots = Rules.FixedRoundShots;
            AudioPreferences.Load(out var music, out var sfx);
            MusicVolume = music;
            SfxVolume = sfx;
            VibrationEnabled = PlayerPrefs.GetInt(VibrationKey, 1) != 0;
        }

        public void BeginSelection()
        {
            IsLocalMultiplayer = false;
            HasDifficultySelection = false;
            HasModeSelection = false;
        }

        public void SelectLocalMultiplayer()
        {
            BeginSelection();
            IsLocalMultiplayer = true;
        }

        public void SelectDifficulty(Difficulty difficulty)
        {
            IsLocalMultiplayer = false;
            // Snapshot the configured probability. It does not change with score or time.
            var probability = Rules.SaveProbability(difficulty);
            SelectedDifficulty = difficulty;
            SelectedSaveProbability = probability;
            HasDifficultySelection = true;
            HasModeSelection = false;
        }

        public void SelectMode(GameMode mode)
        {
            if (!HasDifficultySelection && !IsLocalMultiplayer) throw new InvalidOperationException("Choose difficulty before mode.");
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
            if (Instance == this) Instance = null;
        }
    }
}
