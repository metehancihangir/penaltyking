using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace PenaltyKing.Tests
{
    public sealed class GameplayAudioTests
    {
        private float music, sfx;
        [SetUp]
        public void RememberVolumes()
        { RuntimeBootstrap.EnsureServices(); music = GameManager.Instance.MusicVolume; sfx = GameManager.Instance.SfxVolume; }
        [UnityTearDown]
        public IEnumerator Restore()
        {
            yield return SceneManager.LoadSceneAsync("MainMenu");
            GameManager.Instance.SetMusicVolume(music); GameManager.Instance.SetSfxVolume(sfx);
            GameManager.Instance.BeginSelection();
        }

        private static IEnumerator Open(float probability)
        {
            var state = GameManager.Instance; var backup = JsonUtility.ToJson(state.Rules);
            try
            {
                JsonUtility.FromJsonOverwrite("{\"mediumSaveProbability\":" + probability.ToString(System.Globalization.CultureInfo.InvariantCulture) + "}", state.Rules);
                state.SelectDifficulty(Difficulty.Medium); state.SelectMode(GameMode.FixedRound);
            }
            finally { JsonUtility.FromJsonOverwrite(backup, state.Rules); }
            yield return SceneManager.LoadSceneAsync("Gameplay"); yield return null;
        }

        [UnityTest]
        public IEnumerator RealClipsPlayAtContactAndCorrectOutcomeThenStopOnExit()
        {
            foreach (var probability in new[] { 0f, 1f })
            {
                yield return Open(probability);
                var game = Object.FindFirstObjectByType<GameplayController>();
                var clips = game.GetComponent<GameplayAudio>(); var bus = AudioManager.Instance;
                Assert.That(bus.CrowdSource.clip, Is.SameAs(clips.Ambience));
                Assert.That(bus.CrowdSource.loop && bus.CrowdSource.isPlaying, Is.True);
                Assert.That(bus.SfxSource.isPlaying, Is.False);
                var kicks = 0; var impacts = 0;
                game.Presentation.Kick += () => { kicks++; Assert.That(bus.SfxSource.clip, Is.SameAs(clips.KickClip)); Assert.That(bus.SfxSource.isPlaying, Is.True); };
                game.Presentation.Impact += shot =>
                {
                    impacts++; Assert.That(bus.SfxSource.clip, Is.SameAs(probability == 0 ? clips.GoalClip : clips.SaveClip));
                    Assert.That(bus.SfxSource.isPlaying, Is.True);
                };
                game.Left.OnPointerDown(new PointerEventData(EventSystem.current));
                Assert.That(bus.SfxSource.isPlaying, Is.False, "No kick sound before foot contact");
                var deadline = Time.realtimeSinceStartup + 4;
                while (game.State == PlayState.ShowingShot)
                { Assert.That(Time.realtimeSinceStartup, Is.LessThan(deadline)); yield return null; }
                Assert.That(kicks, Is.EqualTo(1)); Assert.That(impacts, Is.EqualTo(1));
                Assert.That(bus.CrowdSource.isPlaying, Is.True);
                yield return SceneManager.LoadSceneAsync("MainMenu");
                Assert.That(bus.CrowdSource.isPlaying || bus.SfxSource.isPlaying, Is.False);
                Assert.That(bus.StadiumActive, Is.False);
            }
        }

        [UnityTest]
        public IEnumerator SfxControlsPlayingAmbienceAndReactionIndependentlyOfMusic()
        {
            yield return Open(0);
            var game = Object.FindFirstObjectByType<GameplayController>(); var bus = AudioManager.Instance;
            game.Center.OnPointerDown(new PointerEventData(EventSystem.current));
            yield return new WaitForSecondsRealtime(1.0f);
            Assert.That(bus.SfxSource.isPlaying && bus.CrowdSource.isPlaying, Is.True);
            GameManager.Instance.SetMusicVolume(.17f); GameManager.Instance.SetSfxVolume(.36f);
            Assert.That(bus.CrowdSource.volume, Is.EqualTo(.36f).Within(.001f));
            Assert.That(bus.SfxSource.volume, Is.EqualTo(.36f).Within(.001f));
            Assert.That(bus.MusicSource.volume, Is.EqualTo(.17f).Within(.001f));
            GameManager.Instance.SetSfxVolume(0);
            Assert.That(bus.CrowdSource.volume + bus.SfxSource.volume, Is.Zero);
            Assert.That(bus.MusicSource.volume, Is.EqualTo(.17f).Within(.001f));
            GameManager.Instance.SetSfxVolume(1);
            Assert.That(bus.CrowdSource.volume, Is.EqualTo(1));
            Assert.That(bus.CrowdSource.isPlaying, Is.True, "Muting must not restart the loop");
        }

        [UnityTest]
        public IEnumerator ClipsHaveSignalHeadroomAndSeamlessLoopBoundary()
        {
            yield return Open(0);
            var clips = Object.FindFirstObjectByType<GameplayAudio>();
            foreach (var clip in new[] { clips.Ambience, clips.KickClip, clips.GoalClip, clips.SaveClip })
            {
                Assert.That(clip.loadState, Is.EqualTo(AudioDataLoadState.Loaded));
                var data = new float[clip.samples * clip.channels]; Assert.That(clip.GetData(data, 0), Is.True);
                double energy = 0; float peak = 0;
                foreach (var sample in data) { energy += sample * sample; peak = Mathf.Max(peak, Mathf.Abs(sample)); }
                Assert.That(peak, Is.LessThan(.82f)); Assert.That(energy / data.Length, Is.GreaterThan(.00001));
                if (clip == clips.Ambience)
                {
                    Assert.That(clip.length, Is.GreaterThan(20));
                    for (var channel = 0; channel < clip.channels; channel++)
                        Assert.That(Mathf.Abs(data[channel] - data[data.Length - clip.channels + channel]), Is.LessThan(.015f));
                }
                else Assert.That(clip.length, Is.LessThan(ShotPresentation.Duration - ShotPresentation.ImpactTime));
            }
        }

        [UnityTest]
        public IEnumerator LeavingBeforeContactAndOpeningWithoutSelectionsAreSilent()
        {
            yield return Open(0);
            var game = Object.FindFirstObjectByType<GameplayController>();
            game.Right.OnPointerDown(new PointerEventData(EventSystem.current));
            yield return SceneManager.LoadSceneAsync("MainMenu");
            yield return new WaitForSecondsRealtime(1);
            Assert.That(AudioManager.Instance.SfxSource.isPlaying || AudioManager.Instance.CrowdSource.isPlaying, Is.False);
            GameManager.Instance.BeginSelection();
            yield return SceneManager.LoadSceneAsync("Gameplay"); yield return null;
            Assert.That(AudioManager.Instance.StadiumActive, Is.False);
        }
    }
}
