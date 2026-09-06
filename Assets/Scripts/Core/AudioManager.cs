using UnityEngine;

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
        }

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
            if (state != null) state.VolumeChanged -= ApplyVolumes;
            if (Instance == this) Instance = null;
        }
    }
}
