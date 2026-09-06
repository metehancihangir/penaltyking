using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace PenaltyKing.Tests
{
    public sealed class SelectionTests
    {
        private Difficulty originalDifficulty;
        private GameMode originalMode;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            RuntimeBootstrap.EnsureServices();
            originalDifficulty = GameManager.Instance.SelectedDifficulty;
            originalMode = GameManager.Instance.SelectedMode;
            GameManager.Instance.BeginSelection();
            yield return SceneManager.LoadSceneAsync("MainMenu");
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            yield return SceneManager.LoadSceneAsync("MainMenu");
            GameManager.Instance.SelectDifficulty(originalDifficulty);
            GameManager.Instance.SelectMode(originalMode);
            GameManager.Instance.BeginSelection();
        }

        private static void Click(PixelMenuButton button)
        {
            Canvas.ForceUpdateCanvases();
            var canvas = button.GetComponentInParent<Canvas>();
            var pointer = new PointerEventData(EventSystem.current)
            {
                position = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, button.transform.position),
                button = PointerEventData.InputButton.Left
            };
            var hits = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer, hits);
            Assert.That(hits, Is.Not.Empty);
            Assert.That(hits[0].gameObject.GetComponentInParent<PixelMenuButton>(), Is.SameAs(button));
            ExecuteEvents.ExecuteHierarchy(hits[0].gameObject, pointer, ExecuteEvents.pointerClickHandler);
        }

        private static IEnumerator WaitForScene(string scene)
        {
            var deadline = Time.realtimeSinceStartup + 10f;
            while (SceneManager.GetActiveScene().name != scene)
            {
                Assert.That(Time.realtimeSinceStartup, Is.LessThan(deadline), "Scene transition timed out");
                yield return null;
            }
            yield return null;
        }

        [UnityTest]
        public IEnumerator AllSixDifficultyModePairsReachGameplayWithTheirSelections()
        {
            foreach (Difficulty difficulty in System.Enum.GetValues(typeof(Difficulty)))
            foreach (GameMode mode in System.Enum.GetValues(typeof(GameMode)))
            {
                yield return SceneManager.LoadSceneAsync("MainMenu");
                yield return null;
                Click(Object.FindFirstObjectByType<MainMenuController>().Singleplayer);
                yield return WaitForScene("DifficultySelect");
                var screen = Object.FindFirstObjectByType<DifficultySelectController>();
                Assert.That(Object.FindFirstObjectByType<PhaseZeroDiagnostics>(), Is.Null);
                Click(difficulty == Difficulty.Easy ? screen.Easy : difficulty == Difficulty.Medium ? screen.Medium : screen.Hard);
                yield return WaitForScene("ModeSelect");
                var modes = Object.FindFirstObjectByType<ModeSelectController>();
                var state = GameManager.Instance;
                Assert.That(state.SelectedDifficulty, Is.EqualTo(difficulty));
                Assert.That(modes.DifficultyLabel.text, Does.Contain(ModeSelectController.DifficultyName(difficulty)));
                Assert.That(state.HasModeSelection, Is.False);
                Click(mode == GameMode.FixedRound ? modes.FixedRound : modes.Endless);
                yield return WaitForScene("Gameplay");
                Assert.That(GameManager.Instance, Is.SameAs(state));
                Assert.That(state.SelectedDifficulty, Is.EqualTo(difficulty));
                Assert.That(state.SelectedMode, Is.EqualTo(mode));
                Assert.That(state.HasDifficultySelection && state.HasModeSelection, Is.True);
                Assert.That(state.SelectedSaveProbability, Is.EqualTo(state.Rules.SaveProbability(difficulty)));
                Assert.That(state.SelectedRoundShots, Is.EqualTo(state.Rules.FixedRoundShots));
            }
        }

        [UnityTest]
        public IEnumerator BackAllowsChangingDifficultyAndFreshEntryRequiresSelection()
        {
            Click(Object.FindFirstObjectByType<MainMenuController>().Singleplayer);
            yield return WaitForScene("DifficultySelect");
            Click(Object.FindFirstObjectByType<DifficultySelectController>().Hard);
            yield return WaitForScene("ModeSelect");
            Click(Object.FindFirstObjectByType<ModeSelectController>().Back);
            yield return WaitForScene("DifficultySelect");
            Assert.That(GameManager.Instance.HasDifficultySelection, Is.False);
            Click(Object.FindFirstObjectByType<DifficultySelectController>().Easy);
            yield return WaitForScene("ModeSelect");
            Assert.That(Object.FindFirstObjectByType<ModeSelectController>().DifficultyLabel.text, Does.Contain("Kolay"));
            Click(Object.FindFirstObjectByType<ModeSelectController>().Back);
            yield return WaitForScene("DifficultySelect");
            Click(Object.FindFirstObjectByType<DifficultySelectController>().Back);
            yield return WaitForScene("MainMenu");
            Click(Object.FindFirstObjectByType<MainMenuController>().Singleplayer);
            yield return WaitForScene("DifficultySelect");
            Assert.That(GameManager.Instance.HasDifficultySelection, Is.False);
            Assert.That(GameManager.Instance.HasModeSelection, Is.False);
        }

        [UnityTest]
        public IEnumerator DirectModeEntryCannotSkipDifficultyAndRapidTapsKeepFirstChoice()
        {
            yield return SceneManager.LoadSceneAsync("ModeSelect");
            yield return null;
            var mode = Object.FindFirstObjectByType<ModeSelectController>();
            Assert.That(mode.FixedRound.interactable || mode.Endless.interactable, Is.False);
            Click(mode.FixedRound);
            yield return null;
            Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo("ModeSelect"));
            Click(mode.Back);
            yield return WaitForScene("DifficultySelect");
            var choices = Object.FindFirstObjectByType<DifficultySelectController>();
            Click(choices.Easy);
            Click(choices.Hard);
            yield return WaitForScene("ModeSelect");
            Assert.That(GameManager.Instance.SelectedDifficulty, Is.EqualTo(Difficulty.Easy));
            mode = Object.FindFirstObjectByType<ModeSelectController>();
            Click(mode.Endless);
            Click(mode.FixedRound);
            yield return WaitForScene("Gameplay");
            Assert.That(GameManager.Instance.SelectedMode, Is.EqualTo(GameMode.Endless));
        }

        [Test]
        public void DefaultRulesAndSelectedProbabilityStayFixedUntilReselection()
        {
            var defaults = ScriptableObject.CreateInstance<GameRules>();
            Assert.That(defaults.SaveProbability(Difficulty.Easy), Is.EqualTo(0.20f));
            Assert.That(defaults.SaveProbability(Difficulty.Medium), Is.EqualTo(0.35f));
            Assert.That(defaults.SaveProbability(Difficulty.Hard), Is.EqualTo(0.50f));
            Assert.That(defaults.FixedRoundShots, Is.EqualTo(5));
            Object.DestroyImmediate(defaults);
            var state = GameManager.Instance;
            var backup = JsonUtility.ToJson(state.Rules);
            try
            {
                state.SelectDifficulty(Difficulty.Easy);
                var selected = state.SelectedSaveProbability;
                JsonUtility.FromJsonOverwrite("{\"easySaveProbability\":0.77}", state.Rules);
                Assert.That(state.Rules.SaveProbability(Difficulty.Easy), Is.EqualTo(0.77f));
                Assert.That(state.SelectedSaveProbability, Is.EqualTo(selected));
                state.SelectDifficulty(Difficulty.Easy);
                Assert.That(state.SelectedSaveProbability, Is.EqualTo(0.77f));
            }
            finally { JsonUtility.FromJsonOverwrite(backup, state.Rules); }
        }
    }
}
