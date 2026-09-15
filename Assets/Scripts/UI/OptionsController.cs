using UnityEngine;
using UnityEngine.UI;

namespace PenaltyKing
{
    [RequireComponent(typeof(SceneNavigator))]
    public sealed class OptionsController : MonoBehaviour
    {
        [SerializeField] private Slider music;
        [SerializeField] private Slider sfx;
        [SerializeField] private Text musicPercent;
        [SerializeField] private Text sfxPercent;
        [SerializeField] private PixelMenuButton back;
        [SerializeField] private Toggle vibration;
        public Toggle Vibration => vibration;
        public void ConfigureVibration(Toggle control) => vibration = control;
        public Slider Music => music;
        public Slider Sfx => sfx;
        public PixelMenuButton Back => back;
        [SerializeField] private bool embedded;
        public event System.Action Closed;
        public void ConfigureEmbedded() => embedded = true;
        private GameManager state;

        public void Configure(Slider musicSlider, Slider sfxSlider, Text musicLabel, Text sfxLabel, PixelMenuButton backButton)
        {
            music = musicSlider; sfx = sfxSlider;
            musicPercent = musicLabel; sfxPercent = sfxLabel; back = backButton;
        }

        private void OnEnable()
        {
            state = GameManager.Instance;
            Refresh(state.MusicVolume, state.SfxVolume);
            music.onValueChanged.AddListener(ChangeMusic);
            sfx.onValueChanged.AddListener(ChangeSfx);
            back.onClick.AddListener(GoBack);
            state.VolumeChanged += Refresh;
            if (vibration != null)
            {
                vibration.SetIsOnWithoutNotify(state.VibrationEnabled);
                vibration.onValueChanged.AddListener(ChangeVibration);
                state.VibrationChanged += RefreshVibration;
            }
        }

        private void ChangeVibration(bool enabled) => state.SetVibrationEnabled(enabled);
        private void RefreshVibration(bool enabled) => vibration.SetIsOnWithoutNotify(enabled);
        private void ChangeMusic(float value) { state.SetMusicVolume(value); AudioPreferences.ScheduleSave(state.MusicVolume, state.SfxVolume); }
        private void ChangeSfx(float value) { state.SetSfxVolume(value); AudioPreferences.ScheduleSave(state.MusicVolume, state.SfxVolume); }

        private void Refresh(float musicVolume, float sfxVolume)
        {
            music.SetValueWithoutNotify(musicVolume);
            sfx.SetValueWithoutNotify(sfxVolume);
            musicPercent.text = $"{Mathf.RoundToInt(musicVolume * 100)}%";
            sfxPercent.text = $"{Mathf.RoundToInt(sfxVolume * 100)}%";
        }

        public void Flush()
        {
            AudioPreferences.Flush();
        }

        private void GoBack() { Flush(); if (embedded) Closed?.Invoke(); else GetComponent<SceneNavigator>().Navigate(GameScene.MainMenu); }
        private void OnApplicationPause(bool paused) { if (paused) AudioPreferences.Flush(); }
        private void OnApplicationFocus(bool focused) { if (!focused) AudioPreferences.Flush(); }
        private void OnApplicationQuit() => AudioPreferences.Flush();
        private void OnDisable()
        {
            Flush();
            if (state != null)
            {
                state.VolumeChanged -= Refresh;
                state.VibrationChanged -= RefreshVibration;
            }
            if (vibration != null) vibration.onValueChanged.RemoveListener(ChangeVibration);
            music.onValueChanged.RemoveListener(ChangeMusic);
            sfx.onValueChanged.RemoveListener(ChangeSfx);
            back.onClick.RemoveListener(GoBack);
        }
    }
}
