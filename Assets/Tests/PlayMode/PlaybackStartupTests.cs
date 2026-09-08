using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
namespace PenaltyKing.Tests
{
    public sealed class PlaybackStartupTests
    {
        [UnityTest]
        public IEnumerator CreatingAudioInAnAlreadyOpenMenuStartsWithoutNavigation()
        {
            RuntimeBootstrap.EnsureServices();
            yield return SceneManager.LoadSceneAsync("MainMenu");
            Object.Destroy(AudioManager.Instance.gameObject);
            yield return null;
            RuntimeBootstrap.EnsureServices();
            yield return new WaitForSecondsRealtime(.3f);
            var bus = AudioManager.Instance;
            Assert.That(bus.MusicSource.isPlaying, Is.True);
            var position = bus.MusicSource.timeSamples;
            yield return new WaitForSecondsRealtime(.2f);
            Assert.That(bus.MusicSource.timeSamples, Is.GreaterThan(position));
            Assert.That(Application.runInBackground, Is.True);
            Assert.That(Object.FindFirstObjectByType<AudioListener>().isActiveAndEnabled, Is.True);
        }
    }
}
