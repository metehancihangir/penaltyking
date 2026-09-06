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
        public float MusicVolume { get; private set; } = 0.7f;
        public float SfxVolume { get; private set; } = 0.8f;
        public event Action<float, float> VolumeChanged;

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
            AudioPreferences.Load(out var music, out var sfx);
            MusicVolume = music;
            SfxVolume = sfx;
        }

        public void SelectDifficulty(Difficulty difficulty) => SelectedDifficulty = difficulty;
        public void SelectMode(GameMode mode) => SelectedMode = mode;

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
