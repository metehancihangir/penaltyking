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
        public Slider Music => music;
        public Slider Sfx => sfx;
        public PixelMenuButton Back => back;
        private GameManager state;
        private bool pendingSave;
        private float saveAt;

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
        }

        private void ChangeMusic(float value) { state.SetMusicVolume(value); ScheduleSave(); }
        private void ChangeSfx(float value) { state.SetSfxVolume(value); ScheduleSave(); }
        private void ScheduleSave() { pendingSave = true; saveAt = Time.unscaledTime + 0.3f; }
        private void Update() { if (pendingSave && Time.unscaledTime >= saveAt) Flush(); }

        private void Refresh(float musicVolume, float sfxVolume)
        {
            music.SetValueWithoutNotify(musicVolume);
            sfx.SetValueWithoutNotify(sfxVolume);
            musicPercent.text = $"{Mathf.RoundToInt(musicVolume * 100)}%";
            sfxPercent.text = $"{Mathf.RoundToInt(sfxVolume * 100)}%";
        }

        public void Flush()
        {
            if (!pendingSave || state == null) return;
            AudioPreferences.Save(state.MusicVolume, state.SfxVolume);
            pendingSave = false;
        }

        private void GoBack() { Flush(); GetComponent<SceneNavigator>().Navigate(GameScene.MainMenu); }
        private void OnApplicationPause(bool paused) { if (paused) Flush(); }
        private void OnApplicationFocus(bool focused) { if (!focused) Flush(); }
        private void OnApplicationQuit() => Flush();
        private void OnDisable()
        {
            Flush();
            if (state != null) state.VolumeChanged -= Refresh;
            music.onValueChanged.RemoveListener(ChangeMusic);
            sfx.onValueChanged.RemoveListener(ChangeSfx);
            back.onClick.RemoveListener(GoBack);
        }
    }
}
