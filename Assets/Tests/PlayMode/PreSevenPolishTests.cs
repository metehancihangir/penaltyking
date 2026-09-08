using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
namespace PenaltyKing.Tests
{
 public sealed class PreSevenPolishTests
 {
  [UnityTearDown] public IEnumerator Close(){yield return SceneManager.LoadSceneAsync("MainMenu");}
  [UnityTest] public IEnumerator MenuThemeAndStadiumStayOnTheirOwnScreensAndHandoffKeepsAudio()
  {
   RuntimeBootstrap.EnsureServices();yield return SceneManager.LoadSceneAsync("MainMenu");yield return null;
   var audio=AudioManager.Instance;Assert.That(audio.MusicSource.isPlaying,Is.True);Assert.That(audio.MusicSource.clip.name,Is.EqualTo("MenuPixelTheme"));
   var theme=audio.MusicSource.clip;yield return SceneManager.LoadSceneAsync("Options");yield return null;Assert.That(audio.MusicSource.clip,Is.SameAs(theme));Assert.That(audio.MusicSource.isPlaying,Is.True);
   GameManager.Instance.SelectLocalMultiplayer();GameManager.Instance.SelectMode(GameMode.Endless);yield return SceneManager.LoadSceneAsync("Gameplay");yield return null;
   var game=Object.FindFirstObjectByType<GameplayController>();Assert.That(audio.MusicSource.isPlaying,Is.False);
   Assert.That(game.Handoff.GetComponent<Image>().color.a,Is.LessThan(.2f));Assert.That(game.Handoff.Title,Is.EqualTo("Şut Sırası\nPLAYER 1'de"));
   var clip=audio.CrowdSource.clip;var position=audio.CrowdSource.timeSamples;yield return new WaitForSecondsRealtime(.3f);Assert.That(audio.CrowdSource.timeSamples,Is.GreaterThan(position));
   yield return LocalTestInput.Continue(game);LocalTestInput.Press(game.Left);Assert.That(game.Handoff.Visible,Is.True);Assert.That(audio.CrowdSource.clip,Is.SameAs(clip));Assert.That(audio.CrowdSource.isPlaying,Is.True);
   position=audio.CrowdSource.timeSamples;yield return new WaitForSecondsRealtime(.3f);Assert.That(audio.CrowdSource.timeSamples,Is.GreaterThan(position));
   yield return SceneManager.LoadSceneAsync("MainMenu");yield return null;Assert.That(audio.MusicSource.isPlaying,Is.True);Assert.That(audio.CrowdSource.isPlaying,Is.False);
  }
  [UnityTest] public IEnumerator AllStandSectionsCelebrateOnlyOnGoalAndReset()
  {
   RuntimeBootstrap.EnsureServices();GameManager.Instance.SelectLocalMultiplayer();GameManager.Instance.SelectMode(GameMode.Endless);yield return SceneManager.LoadSceneAsync("Gameplay");yield return null;
   var game=Object.FindFirstObjectByType<GameplayController>();var fans=Object.FindFirstObjectByType<CrowdCelebration>();Assert.That(fans.Sections.Length,Is.EqualTo(12));
   game.Presentation.Sample(new ShotResult(ShotDirection.Left,ShotDirection.Right),1.03f);foreach(var section in fans.Sections)Assert.That(section.anchoredPosition.y,Is.GreaterThan(0));
   game.Presentation.Sample(new ShotResult(ShotDirection.Left,ShotDirection.Left),1.03f);foreach(var section in fans.Sections)Assert.That(section.anchoredPosition,Is.EqualTo(Vector2.zero));
   game.Presentation.ResetPose();foreach(var section in fans.Sections)Assert.That(section.anchoredPosition,Is.EqualTo(Vector2.zero));
  }
 }
}
