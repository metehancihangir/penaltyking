using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
namespace PenaltyKing.Tests
{
 public sealed class FirstPlayGuideTests
 {
  GameplayController game;FirstPlayGuide guide;
  [UnitySetUp] public IEnumerator Open(){PlayerPrefs.DeleteKey(FirstPlayGuide.CompletedKey);RuntimeBootstrap.EnsureServices();GameManager.Instance.SelectLocalMultiplayer();GameManager.Instance.SelectMode(GameMode.FixedRound);yield return SceneManager.LoadSceneAsync("Gameplay");yield return null;game=Object.FindFirstObjectByType<GameplayController>();guide=Object.FindFirstObjectByType<FirstPlayGuide>();}
  [UnityTearDown] public IEnumerator Close(){yield return SceneManager.LoadSceneAsync("MainMenu");PlayerPrefs.SetInt(FirstPlayGuide.CompletedKey,1);}
  [UnityTest] public IEnumerator GuideConsumesInputAndPersistsOnlyWhenCompleted()
  {
   Assert.That(guide.Visible && game.TutorialOpen,Is.True);
   game.Left.OnPointerDown(LocalTestInput.Pointer(game.Left));Assert.That(game.Round.HasShotSelection,Is.False);
   LocalTestInput.Press(guide.Next);guide.Next.onClick.Invoke();Assert.That(guide.Step,Is.EqualTo(1));Assert.That(PlayerPrefs.HasKey(FirstPlayGuide.CompletedKey),Is.False);
   yield return null;LocalTestInput.Press(guide.Next);yield return null;LocalTestInput.Press(guide.Next);
   Assert.That(guide.Visible || game.TutorialOpen,Is.False);Assert.That(game.Round.ShotsTaken,Is.Zero);Assert.That(game.Handoff.Visible,Is.True);
   yield return LocalTestInput.Continue(game);LocalTestInput.Press(game.Right);Assert.That(game.Round.HasShotSelection,Is.True);
   yield return SceneManager.LoadSceneAsync("Gameplay");yield return null;
   Assert.That(Object.FindFirstObjectByType<FirstPlayGuide>().Visible,Is.False);Assert.That(PlayerPrefs.GetInt(FirstPlayGuide.CompletedKey),Is.EqualTo(1));
  }
  [UnityTest] public IEnumerator PracticeTargetsNeverCommitAMatchChoice()
  {
   var practice=Object.FindFirstObjectByType<GuidePractice>();
   var buttons=practice.transform.Find("Directions").GetComponentsInChildren<UnityEngine.UI.Button>();
   Assert.That(buttons.Length,Is.EqualTo(3));
   LocalTestInput.Press(buttons[0]);
   Assert.That(game.Round.HasShotSelection,Is.False);Assert.That(game.Round.ShotsTaken,Is.Zero);
   Assert.That(practice.transform.Find("Description").GetComponent<UnityEngine.UI.Text>().text,Does.Contain("Harika"));
   var settings=Object.FindFirstObjectByType<InGameSettings>();LocalTestInput.Press(settings.OpenButton);yield return null;
   var detail=practice.transform.Find("Description").GetComponent<UnityEngine.UI.Text>().text;
   buttons[1].onClick.Invoke();Assert.That(practice.transform.Find("Description").GetComponent<UnityEngine.UI.Text>().text,Is.EqualTo(detail));
  }
  [UnityTest] public IEnumerator SettingsBlockGuideAndPartialGuideRestarts()
  {
   LocalTestInput.Press(guide.Next);yield return null;
   var settings=Object.FindFirstObjectByType<InGameSettings>();LocalTestInput.Press(settings.OpenButton);yield return null;
   guide.Next.onClick.Invoke();Assert.That(guide.Step,Is.EqualTo(1));Assert.That(game.TutorialOpen,Is.True);
   LocalTestInput.Press(settings.Options.Back);yield return null;Assert.That(guide.Visible,Is.True);
   yield return SceneManager.LoadSceneAsync("Gameplay");yield return null;
   guide=Object.FindFirstObjectByType<FirstPlayGuide>();Assert.That(guide.Visible,Is.True);Assert.That(guide.Step,Is.Zero);
  }
 }
}
