using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PenaltyKing
{
    [DisallowMultipleComponent]
    public sealed class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }
        public AudioSource MusicSource { get; private set; }
        public AudioSource SfxSource { get; private set; }
        // Crowd ambience must remain on the SFX volume bus (user decision).
        public AudioSource CrowdSource { get; private set; }
        private GameManager state;
        private Object stadiumOwner;
        private AudioClip menuTheme;
        public bool StadiumActive => stadiumOwner != null;

        public void BeginStadium(Object owner, AudioClip ambience)
        {
            if (owner == null || ambience == null) return;
            if (stadiumOwner == owner && CrowdSource.isPlaying) return;
            SfxSource.Stop();
            stadiumOwner = owner;
            CrowdSource.clip = ambience;
            CrowdSource.Play();
#if DEVELOPMENT_BUILD
            StartCoroutine(MeasureOutput("crowd", CrowdSource));
#endif
        }

        public void PlayStadiumCue(Object owner, AudioClip clip)
        {
            if (owner != stadiumOwner || owner == null || clip == null) return;
            SfxSource.Stop();
            SfxSource.clip = clip;
            SfxSource.Play();
#if DEVELOPMENT_BUILD
            StartCoroutine(MeasureOutput(clip.name, SfxSource));
#endif
        }

        public void EndStadium(Object owner)
        {
            if (owner != stadiumOwner) return;
            CrowdSource.Stop(); SfxSource.Stop();
            CrowdSource.clip = null; SfxSource.clip = null;
            stadiumOwner = null;
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
            MusicSource = CreateSource("Music", true);
            SfxSource = CreateSource("SFX", false);
            CrowdSource = CreateSource("Crowd Ambience (SFX)", true);
            state = GameManager.Instance;
            state.VolumeChanged += ApplyVolumes;
            ApplyVolumes(state.MusicVolume, state.SfxVolume);
            menuTheme = Resources.Load<AudioClip>("Audio/MenuPixelTheme");
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        // BeforeSceneLoad creates services before the scene's AudioListener exists.
        // Start also covers pressing Play directly in MainMenu without navigation.
        private IEnumerator Start()
        {
            yield return null;
            OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "Gameplay") { MusicSource.Stop(); return; }
            if (menuTheme != null && !MusicSource.isPlaying)
            {
                MusicSource.clip = menuTheme; MusicSource.Play();
#if DEVELOPMENT_BUILD
                StartCoroutine(MeasureOutput("menu", MusicSource));
#endif
            }
        }

#if DEVELOPMENT_BUILD
        private IEnumerator MeasureOutput(string cue, AudioSource source)
        {
            yield return new WaitForSecondsRealtime(.1f);
            var samples = new float[1024];
            float peak = 0;
            for (var frame = 0; frame < 6; frame++)
            {
                source.GetOutputData(samples, 0);
                foreach (var sample in samples) peak = Mathf.Max(peak, Mathf.Abs(sample));
                yield return new WaitForSecondsRealtime(.025f);
            }
            Debug.Log(System.FormattableString.Invariant($"[MobileAudio] cue={cue} playing={source.isPlaying} volume={source.volume:F3} outputPeak={peak:F6}"));
        }
#endif

        private AudioSource CreateSource(string channelName, bool loop)
        {
            var child = new GameObject(channelName);
            child.transform.SetParent(transform, false);
            var source = child.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = 0f;
            source.loop = loop;
            return source;
        }

        private void ApplyVolumes(float music, float sfx)
        {
            MusicSource.volume = music;
            SfxSource.volume = sfx;
            CrowdSource.volume = sfx;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            if (state != null) state.VolumeChanged -= ApplyVolumes;
            if (Instance == this) Instance = null;
        }
    }
}
