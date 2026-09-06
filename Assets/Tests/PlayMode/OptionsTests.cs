using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace PenaltyKing.Tests
{
    public sealed class OptionsTests
    {
        private bool hadMusic, hadSfx;
        private float storedMusic, storedSfx, sessionMusic, sessionSfx;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            RuntimeBootstrap.EnsureServices();
            hadMusic = PlayerPrefs.HasKey(AudioPreferences.MusicKey);
            hadSfx = PlayerPrefs.HasKey(AudioPreferences.SfxKey);
            storedMusic = PlayerPrefs.GetFloat(AudioPreferences.MusicKey);
            storedSfx = PlayerPrefs.GetFloat(AudioPreferences.SfxKey);
            sessionMusic = GameManager.Instance.MusicVolume;
            sessionSfx = GameManager.Instance.SfxVolume;
            yield return SceneManager.LoadSceneAsync("Options");
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            // Leave the screen before restoring preferences so OnDisable cannot overwrite them.
            yield return SceneManager.LoadSceneAsync("MainMenu");
            if (hadMusic) PlayerPrefs.SetFloat(AudioPreferences.MusicKey, storedMusic);
            else PlayerPrefs.DeleteKey(AudioPreferences.MusicKey);
            if (hadSfx) PlayerPrefs.SetFloat(AudioPreferences.SfxKey, storedSfx);
            else PlayerPrefs.DeleteKey(AudioPreferences.SfxKey);
            PlayerPrefs.Save();
            GameManager.Instance.SetMusicVolume(sessionMusic);
            GameManager.Instance.SetSfxVolume(sessionSfx);
        }

        private static void Drag(Slider slider, float fraction)
        {
            Canvas.ForceUpdateCanvases();
            var camera = slider.GetComponentInParent<Canvas>().worldCamera;
            var rect = (RectTransform)slider.handleRect.parent;
            var world = rect.TransformPoint(new Vector3(Mathf.Lerp(rect.rect.xMin, rect.rect.xMax, fraction), 0, 0));
            var pointer = new PointerEventData(EventSystem.current)
            {
                position = RectTransformUtility.WorldToScreenPoint(camera, world),
                button = PointerEventData.InputButton.Left
            };
            var hits = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer, hits);
            Assert.That(hits, Is.Not.Empty);
            Assert.That(hits[0].gameObject.GetComponentInParent<Slider>(), Is.SameAs(slider));
            pointer.pointerPressRaycast = hits[0];
            ExecuteEvents.ExecuteHierarchy(hits[0].gameObject, pointer, ExecuteEvents.pointerDownHandler);
            slider.OnDrag(pointer);
            slider.OnPointerUp(pointer);
        }

        [UnityTest]
        public IEnumerator ExactlyTwoSlidersDragAndUpdateIndependentChannelsImmediately()
        {
            var options = Object.FindFirstObjectByType<OptionsController>();
            Assert.That(Object.FindObjectsByType<Slider>(FindObjectsSortMode.None), Has.Length.EqualTo(2));
            Assert.That(Object.FindObjectsByType<Button>(FindObjectsSortMode.None), Has.Length.EqualTo(1));
            Assert.That(Object.FindFirstObjectByType<PhaseZeroDiagnostics>(), Is.Null);
            Drag(options.Music, 0.25f);
            Assert.That(AudioManager.Instance.MusicSource.volume, Is.EqualTo(0.25f).Within(0.015f));
            Drag(options.Sfx, 0f);
            Assert.That(AudioManager.Instance.SfxSource.volume, Is.Zero.Within(0.015f));
            Assert.That(AudioManager.Instance.CrowdSource.volume, Is.Zero.Within(0.015f));
            Assert.That(AudioManager.Instance.MusicSource.volume, Is.EqualTo(0.25f).Within(0.015f));
            Drag(options.Sfx, 1f);
            Assert.That(AudioManager.Instance.SfxSource.volume, Is.EqualTo(1f).Within(0.015f));
            Assert.That(AudioManager.Instance.CrowdSource.volume, Is.EqualTo(1f).Within(0.015f));
            yield return new WaitForSecondsRealtime(0.4f);
            Assert.That(PlayerPrefs.GetFloat(AudioPreferences.SfxKey), Is.EqualTo(1f).Within(0.015f));
        }

        [UnityTest]
        public IEnumerator BackSavesImmediatelyAndReturnsToMainMenu()
        {
            var options = Object.FindFirstObjectByType<OptionsController>();
            options.Music.value = 0.31f; options.Sfx.value = 0.62f;
            options.Back.OnPointerClick(new PointerEventData(EventSystem.current) { button = PointerEventData.InputButton.Left });
            var deadline = Time.realtimeSinceStartup + 10f;
            while (SceneManager.GetActiveScene().name != "MainMenu")
            {
                Assert.That(Time.realtimeSinceStartup, Is.LessThan(deadline));
                yield return null;
            }
            Assert.That(PlayerPrefs.GetFloat(AudioPreferences.MusicKey), Is.EqualTo(0.31f).Within(0.001f));
            Assert.That(PlayerPrefs.GetFloat(AudioPreferences.SfxKey), Is.EqualTo(0.62f).Within(0.001f));
        }

        [UnityTest]
        public IEnumerator RecreatedServicesAndScreenRestoreSavedVolumes()
        {
            var options = Object.FindFirstObjectByType<OptionsController>();
            options.Music.value = 0.19f; options.Sfx.value = 0.83f;
            options.Flush();
            yield return SceneManager.LoadSceneAsync("MainMenu");
            Object.Destroy(AudioManager.Instance.gameObject);
            Object.Destroy(GameManager.Instance.gameObject);
            yield return null;
            RuntimeBootstrap.EnsureServices();
            yield return SceneManager.LoadSceneAsync("Options");
            options = Object.FindFirstObjectByType<OptionsController>();
            Assert.That(options.Music.value, Is.EqualTo(0.19f).Within(0.001f));
            Assert.That(options.Sfx.value, Is.EqualTo(0.83f).Within(0.001f));
            Assert.That(AudioManager.Instance.MusicSource.volume, Is.EqualTo(0.19f).Within(0.001f));
            Assert.That(AudioManager.Instance.CrowdSource.volume, Is.EqualTo(0.83f).Within(0.001f));
        }
    }
}
