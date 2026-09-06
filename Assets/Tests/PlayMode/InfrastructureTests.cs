using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace PenaltyKing.Tests
{
    public sealed class InfrastructureTests
    {
        [Test]
        public void CrowdAmbienceFollowsSfxAndIsIndependentOfMusic()
        {
            RuntimeBootstrap.EnsureServices();
            var game = GameManager.Instance;
            var audio = AudioManager.Instance;
            var oldMusic = game.MusicVolume;
            var oldSfx = game.SfxVolume;
            try
            {
                game.SetMusicVolume(0.4f);
                game.SetSfxVolume(0f);
                Assert.That(audio.CrowdSource.volume, Is.Zero);
                Assert.That(audio.SfxSource.volume, Is.Zero);
                Assert.That(audio.MusicSource.volume, Is.EqualTo(0.4f).Within(0.001f));
                game.SetSfxVolume(0.6f);
                game.SetMusicVolume(0f);
                Assert.That(audio.MusicSource.volume, Is.Zero);
                Assert.That(audio.CrowdSource.volume, Is.EqualTo(0.6f).Within(0.001f));
                Assert.That(audio.SfxSource.volume, Is.EqualTo(0.6f).Within(0.001f));
            }
            finally
            {
                game.SetMusicVolume(oldMusic);
                game.SetSfxVolume(oldSfx);
            }
        }

        [UnityTest]
        public IEnumerator AllScenesPreserveSessionAndAudioServices()
        {
            RuntimeBootstrap.EnsureServices();
            var game = GameManager.Instance;
            var audio = AudioManager.Instance;
            var oldDifficulty = game.SelectedDifficulty;
            var oldMode = game.SelectedMode;
            var oldMusic = game.MusicVolume;
            var oldSfx = game.SfxVolume;
            try
            {
                game.SelectDifficulty(Difficulty.Hard);
                game.SelectMode(GameMode.Endless);
                game.SetMusicVolume(0.25f);
                game.SetSfxVolume(0.65f);
                yield return SceneManager.LoadSceneAsync("MainMenu");
                foreach (GameScene scene in System.Enum.GetValues(typeof(GameScene)))
                {
                    var navigator = Object.FindFirstObjectByType<SceneNavigator>();
                    Assert.That(navigator, Is.Not.Null);
                    navigator.Navigate(scene);
                    var deadline = Time.realtimeSinceStartup + 15f;
                    do
                    {
                        yield return null;
                        Assert.That(Time.realtimeSinceStartup, Is.LessThan(deadline), "Scene transition timed out");
                    } while (navigator != null && navigator.IsLoading);
                    Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo(scene.ToString()));
                    Assert.That(GameManager.Instance, Is.SameAs(game));
                    Assert.That(AudioManager.Instance, Is.SameAs(audio));
                    Assert.That(Object.FindObjectsByType<GameManager>(FindObjectsSortMode.None), Has.Length.EqualTo(1));
                    Assert.That(Object.FindObjectsByType<AudioManager>(FindObjectsSortMode.None), Has.Length.EqualTo(1));
                    Assert.That(game.SelectedDifficulty, Is.EqualTo(Difficulty.Hard));
                    Assert.That(game.SelectedMode, Is.EqualTo(GameMode.Endless));
                    Assert.That(audio.MusicSource.volume, Is.EqualTo(0.25f).Within(0.001f));
                    Assert.That(audio.SfxSource.volume, Is.EqualTo(0.65f).Within(0.001f));
                    Assert.That(audio.CrowdSource.volume, Is.EqualTo(0.65f).Within(0.001f));
                    Assert.That(Object.FindFirstObjectByType<TouchInputProbe>(), Is.Not.Null);
                }
            }
            finally
            {
                game.SelectDifficulty(oldDifficulty);
                game.SelectMode(oldMode);
                game.SetMusicVolume(oldMusic);
                game.SetSfxVolume(oldSfx);
            }
        }

        [UnityTest]
        public IEnumerator DuplicateManagersDoNotReplaceOrDuplicateServices()
        {
            RuntimeBootstrap.EnsureServices();
            var game = GameManager.Instance;
            var audio = AudioManager.Instance;
            new GameObject("Duplicate GameManager test").AddComponent<GameManager>();
            new GameObject("Duplicate AudioManager test").AddComponent<AudioManager>();
            yield return null;
            Assert.That(GameManager.Instance, Is.SameAs(game));
            Assert.That(AudioManager.Instance, Is.SameAs(audio));
            Assert.That(Object.FindObjectsByType<GameManager>(FindObjectsSortMode.None), Has.Length.EqualTo(1));
            Assert.That(Object.FindObjectsByType<AudioManager>(FindObjectsSortMode.None), Has.Length.EqualTo(1));
        }

        [UnityTest]
        public IEnumerator TouchBeginTriggersOnceAndReleaseDoesNotTrigger()
        {
            yield return SceneManager.LoadSceneAsync("Gameplay");
            var probe = Object.FindFirstObjectByType<TouchInputProbe>();
            var previousBackgroundBehavior = InputSystem.settings.backgroundBehavior;
#if UNITY_EDITOR
            var previousEditorBehavior = InputSystem.settings.editorInputBehaviorInPlayMode;
            // Batchmode has no focused Game View. Route virtual device events to play mode.
            InputSystem.settings.editorInputBehaviorInPlayMode =
                InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
#endif
            InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
            var screen = InputSystem.AddDevice<Touchscreen>();
            var initialCount = probe.PressCount;
            var position = new Vector2(160f, 240f);
            try
            {
                InputSystem.QueueStateEvent(screen, new TouchState
                {
                    touchId = 1, phase = UnityEngine.InputSystem.TouchPhase.Began, position = position
                });
                InputSystem.Update();
                yield return null;
                yield return null;
                Assert.That(probe.PressCount, Is.EqualTo(initialCount + 1));
                Assert.That(probe.LastPosition, Is.EqualTo(position));
                InputSystem.QueueStateEvent(screen, new TouchState
                {
                    touchId = 1, phase = UnityEngine.InputSystem.TouchPhase.Ended, position = position
                });
                InputSystem.Update();
                yield return null;
                yield return null;
                Assert.That(probe.PressCount, Is.EqualTo(initialCount + 1));
            }
            finally
            {
                InputSystem.RemoveDevice(screen);
                InputSystem.settings.backgroundBehavior = previousBackgroundBehavior;
#if UNITY_EDITOR
                InputSystem.settings.editorInputBehaviorInPlayMode = previousEditorBehavior;
#endif
            }
        }
    }
}
