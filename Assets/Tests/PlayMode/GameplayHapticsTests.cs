using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
namespace PenaltyKing.Tests
{
 public sealed class GameplayHapticsTests
 {
  bool previous,existed;int stored;float sfx;
  [UnitySetUp] public IEnumerator Open()
  {
   RuntimeBootstrap.EnsureServices();var state=GameManager.Instance;previous=state.VibrationEnabled;sfx=state.SfxVolume;existed=PlayerPrefs.HasKey(GameManager.VibrationKey);stored=PlayerPrefs.GetInt(GameManager.VibrationKey);
   state.SelectLocalMultiplayer();state.SelectMode(GameMode.Endless);yield return SceneManager.LoadSceneAsync("Gameplay");yield return null;
  }
  [UnityTearDown] public IEnumerator Restore()
  {
   yield return SceneManager.LoadSceneAsync("MainMenu");GameManager.Instance.SetVibrationEnabled(previous);GameManager.Instance.SetSfxVolume(sfx);
   if(existed)PlayerPrefs.SetInt(GameManager.VibrationKey,stored);else PlayerPrefs.DeleteKey(GameManager.VibrationKey);PlayerPrefs.Save();
  }
  [UnityTest] public IEnumerator OnlyGoalsAtImpactPulseOnceEvenWhenSilentAndPaused()
  {
   var game=Object.FindFirstObjectByType<GameplayController>();var settings=Object.FindFirstObjectByType<InGameSettings>();var pulses=0;float impactAt=-1;
   game.GetComponent<GameplayHaptics>().PulseRequested+=duration=>{pulses++;impactAt=game.Presentation.Elapsed;Assert.That(duration,Is.EqualTo(65));};
   GameManager.Instance.SetVibrationEnabled(true);GameManager.Instance.SetSfxVolume(0);
   yield return LocalTestInput.Choose(game,game.Left,game.Right);Assert.That(pulses,Is.Zero);
   LocalTestInput.Press(settings.OpenButton);yield return new WaitForSecondsRealtime(1.1f);Assert.That(pulses,Is.Zero);
   LocalTestInput.Press(settings.Options.Back);yield return new WaitForSecondsRealtime(1.02f);Assert.That(pulses,Is.EqualTo(1));Assert.That(impactAt,Is.GreaterThanOrEqualTo(ShotPresentation.ImpactTime));
   LocalTestInput.Press(settings.OpenButton);yield return new WaitForSecondsRealtime(.3f);LocalTestInput.Press(settings.Options.Back);yield return LocalTestInput.Finish(game);Assert.That(pulses,Is.EqualTo(1));
   yield return LocalTestInput.Choose(game,game.Center,game.Center);yield return LocalTestInput.Finish(game);Assert.That(pulses,Is.EqualTo(1));
   GameManager.Instance.SetVibrationEnabled(false);yield return LocalTestInput.Choose(game,game.Left,game.Right);yield return LocalTestInput.Finish(game);Assert.That(pulses,Is.EqualTo(1));
   GameManager.Instance.SetVibrationEnabled(true);yield return LocalTestInput.Choose(game,game.Right,game.Left);yield return LocalTestInput.Finish(game);Assert.That(pulses,Is.EqualTo(2));
  }
 }
}
