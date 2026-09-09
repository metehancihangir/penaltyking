using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
namespace PenaltyKing.Tests
{
 public sealed class MatchPolishTests
 {
  [UnityTearDown] public IEnumerator Close(){yield return SceneManager.LoadSceneAsync("MainMenu");}
  [UnityTest] public IEnumerator ExitPromptPausesCancelsWithoutLosingMatchAndConfirmsExplicitly()
  {
   RuntimeBootstrap.EnsureServices();GameManager.Instance.SelectLocalMultiplayer();GameManager.Instance.SelectMode(GameMode.Endless);
   yield return SceneManager.LoadSceneAsync("Gameplay");yield return null;
   var game=Object.FindFirstObjectByType<GameplayController>();var dialog=game.GetComponent<ExitConfirmation>();var kits=game.GetComponent<PlayerKitColours>();
   Assert.That(kits.ShooterPlayer,Is.EqualTo(1));
   yield return LocalTestInput.Choose(game,game.Left,game.Right);yield return new WaitForSecondsRealtime(.12f);
   var round=game.Round;LocalTestInput.Press(game.Exit);var elapsed=game.Presentation.Elapsed;
   Assert.That(dialog.Visible && game.SettingsOpen,Is.True);
   yield return new WaitForSecondsRealtime(.4f);
   Assert.That(game.Presentation.Elapsed,Is.EqualTo(elapsed));Assert.That(game.Round,Is.SameAs(round));Assert.That(SceneManager.GetActiveScene().name,Is.EqualTo("Gameplay"));
   Assert.That(AudioManager.Instance.CrowdSource.isPlaying,Is.True);
   LocalTestInput.Press(dialog.CancelButton);Assert.That(game.SettingsOpen || dialog.Visible,Is.False);
   yield return LocalTestInput.Finish(game);Assert.That(round.ShotsTaken,Is.EqualTo(1));Assert.That(kits.ShooterPlayer,Is.EqualTo(2));
   var stage=Object.FindFirstObjectByType<GameplayStageLayout>().transform;
   Assert.That(stage.Find("07 Shooter").GetComponent<Image>().material.GetFloat("_Kit"),Is.EqualTo(1));
   Assert.That(stage.Find("06 Keeper").GetComponent<Image>().material.GetFloat("_Kit"),Is.EqualTo(2));
   LocalTestInput.Press(game.Exit);yield return null;LocalTestInput.Press(dialog.ConfirmButton);
   var deadline=Time.realtimeSinceStartup+10;
   while(SceneManager.GetActiveScene().name!="MainMenu"){Assert.That(Time.realtimeSinceStartup,Is.LessThan(deadline));yield return null;}
   Assert.That(AudioManager.Instance.StadiumActive,Is.False);
  }
 }
}
