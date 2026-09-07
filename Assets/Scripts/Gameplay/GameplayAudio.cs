using UnityEngine;

namespace PenaltyKing
{
    [DisallowMultipleComponent, RequireComponent(typeof(GameplayController), typeof(ShotPresentation))]
    public sealed class GameplayAudio : MonoBehaviour
    {
        [SerializeField] private AudioClip ambience, kick, goal, save;
        private ShotPresentation presentation;
        private AudioManager audioManager;
        private bool started;
        public AudioClip Ambience => ambience;
        public AudioClip KickClip => kick;
        public AudioClip GoalClip => goal;
        public AudioClip SaveClip => save;

        public void Configure(AudioClip loop, AudioClip contact, AudioClip cheer, AudioClip disappointment)
        { ambience = loop; kick = contact; goal = cheer; save = disappointment; }

        private void OnEnable()
        {
            presentation = GetComponent<ShotPresentation>();
            presentation.Kick += OnKick;
            presentation.Impact += OnImpact;
            if (started) Begin();
        }

        private void Start() { started = true; Begin(); }

        private void Begin()
        {
            if (GetComponent<GameplayController>().State == PlayState.NeedsSelection) return;
            audioManager = AudioManager.Instance;
            audioManager.BeginStadium(this, ambience);
        }

        private void OnKick() { if (audioManager != null) audioManager.PlayStadiumCue(this, kick); }
        private void OnImpact(ShotResult result)
        { if (audioManager != null) audioManager.PlayStadiumCue(this, result.Outcome == ShotOutcome.Goal ? goal : save); }

        public void StopCue()
        {
            if (audioManager != null && audioManager.StadiumActive)
                audioManager.SfxSource.Stop();
        }

        public void StopStadium()
        {
            if (audioManager != null) audioManager.EndStadium(this);
            audioManager = null;
        }

        private void OnDisable()
        {
            if (presentation != null) { presentation.Kick -= OnKick; presentation.Impact -= OnImpact; }
            StopStadium();
        }
    }
}
