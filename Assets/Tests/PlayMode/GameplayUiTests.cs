using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace PenaltyKing.Tests
{
    public sealed class GameplayUiTests
    {
        private float music,sfx; private bool vibration,hadVibration,hadMusic,hadSfx; private int savedVibration; private float savedMusic,savedSfx;
        [UnitySetUp]
        public IEnumerator Open()
        {
            RuntimeBootstrap.EnsureServices();
            var state=GameManager.Instance;music=state.MusicVolume;sfx=state.SfxVolume;vibration=state.VibrationEnabled;
            hadVibration=PlayerPrefs.HasKey(GameManager.VibrationKey);savedVibration=PlayerPrefs.GetInt(GameManager.VibrationKey);
            hadMusic=PlayerPrefs.HasKey(AudioPreferences.MusicKey);savedMusic=PlayerPrefs.GetFloat(AudioPreferences.MusicKey);
            hadSfx=PlayerPrefs.HasKey(AudioPreferences.SfxKey);savedSfx=PlayerPrefs.GetFloat(AudioPreferences.SfxKey);
            state.SelectLocalMultiplayer();state.SelectMode(GameMode.Endless);
            yield return SceneManager.LoadSceneAsync("Gameplay");yield return null;
        }
        [UnityTearDown]
        public IEnumerator Close()
        {
            yield return SceneManager.LoadSceneAsync("MainMenu");
            var state=GameManager.Instance;state.SetMusicVolume(music);state.SetSfxVolume(sfx);state.SetVibrationEnabled(vibration);state.BeginSelection();
            if(hadVibration)PlayerPrefs.SetInt(GameManager.VibrationKey,savedVibration);else PlayerPrefs.DeleteKey(GameManager.VibrationKey);
            if(hadMusic)PlayerPrefs.SetFloat(AudioPreferences.MusicKey,savedMusic);else PlayerPrefs.DeleteKey(AudioPreferences.MusicKey);
            if(hadSfx)PlayerPrefs.SetFloat(AudioPreferences.SfxKey,savedSfx);else PlayerPrefs.DeleteKey(AudioPreferences.SfxKey);
            PlayerPrefs.Save();
        }
        [UnityTest]
        public IEnumerator SettingsPreserveEverySelectionStageAndSharePreferences()
        {
            var game=Object.FindFirstObjectByType<GameplayController>();var settings=Object.FindFirstObjectByType<InGameSettings>();
            for(var stage=0;stage<4;stage++)
            {
                if(stage==1)yield return LocalTestInput.Continue(game);
                if(stage==2)LocalTestInput.Press(game.Right);
                if(stage==3)yield return LocalTestInput.Continue(game);
                var round=game.Round;var state=game.State;var pending=round.HasShotSelection;
                LocalTestInput.Press(settings.OpenButton);
                yield return null;
                Assert.That(settings.Visible && game.SettingsOpen,Is.True);
                game.Left.OnPointerDown(new PointerEventData(EventSystem.current));
                game.Handoff.OnPointerDown(new PointerEventData(EventSystem.current));game.Handoff.OnPointerClick(new PointerEventData(EventSystem.current));
                Assert.That(game.State,Is.EqualTo(state));Assert.That(round.HasShotSelection,Is.EqualTo(pending));Assert.That(round.ShotsTaken,Is.Zero);
                settings.Options.Music.value=.21f;settings.Options.Sfx.value=.34f;settings.Options.Vibration.isOn=false;
                Assert.That(AudioManager.Instance.CrowdSource.volume,Is.EqualTo(.34f).Within(.001f));
                Assert.That(AudioManager.Instance.CrowdSource.isPlaying,Is.True);
                Assert.That(GameManager.Instance.VibrationEnabled,Is.False);
                LocalTestInput.Press(settings.Options.Back);yield return null;
                Assert.That(settings.Visible || game.SettingsOpen,Is.False);Assert.That(game.Round,Is.SameAs(round));Assert.That(game.State,Is.EqualTo(state));
            }
            LocalTestInput.Press(game.Left);
            Assert.That(game.LastShot.Value.PlayerDirection,Is.EqualTo(ShotDirection.Right));
            Assert.That(game.LastShot.Value.KeeperDirection,Is.EqualTo(ShotDirection.Left));
            yield return LocalTestInput.Finish(game);
            LocalTestInput.Press(game.Exit);
            Assert.That(game.GetComponent<ExitConfirmation>().Visible, Is.True);
            yield return null; // A second gesture occurs after the modal is rendered.
            LocalTestInput.Press(game.GetComponent<ExitConfirmation>().ConfirmButton);
            while(SceneTransition.IsBusy)yield return null;
            yield return SceneManager.LoadSceneAsync("Options");yield return null;
            var options=Object.FindFirstObjectByType<OptionsController>();Assert.That(options.Sfx.value,Is.EqualTo(.34f).Within(.001f));Assert.That(options.Vibration.isOn,Is.False);
        }
        [UnityTest]
        public IEnumerator PausedShotFreezesAndResumesWithoutDuplicatingAudioEvents()
        {
            var game=Object.FindFirstObjectByType<GameplayController>();var settings=Object.FindFirstObjectByType<InGameSettings>();
            var kicks=0;var impacts=0;game.Presentation.Kick+=()=>kicks++;game.Presentation.Impact+=shot=>impacts++;
            yield return LocalTestInput.Choose(game,game.Left,game.Right);
            LocalTestInput.Press(settings.OpenButton);yield return null;
            var time=game.Presentation.Elapsed;var position=game.Presentation.BallPosition;
            yield return new WaitForSecondsRealtime(.65f);
            Assert.That(game.Presentation.Elapsed,Is.EqualTo(time));Assert.That(game.Presentation.BallPosition,Is.EqualTo(position));Assert.That(kicks+impacts,Is.Zero);
            LocalTestInput.Press(settings.Options.Back);
            yield return new WaitForSecondsRealtime(.4f);
            Assert.That(kicks,Is.EqualTo(1));Assert.That(impacts,Is.Zero);
            LocalTestInput.Press(settings.OpenButton);yield return null;
            time=game.Presentation.Elapsed;yield return new WaitForSecondsRealtime(.7f);
            Assert.That(game.Presentation.Elapsed,Is.EqualTo(time));Assert.That(impacts,Is.Zero);
            LocalTestInput.Press(settings.Options.Back);yield return LocalTestInput.Finish(game);
            Assert.That(kicks,Is.EqualTo(1));Assert.That(impacts,Is.EqualTo(1));Assert.That(game.Round.Player1.Goals,Is.EqualTo(1));
        }
        [UnityTest]
        public IEnumerator ScoreboardShowsBoundedHumanResultsAndOldLabelsAreHidden()
        {
            var board=Object.FindFirstObjectByType<MatchScoreboard>();var round=new PenaltyRound(GameMode.Endless,5);
            for(var i=0;i<140;i++){round.ChooseShot(ShotDirection.Left);round.ChooseKeeper(i%2==0?ShotDirection.Right:ShotDirection.Left);}
            board.Render(round,PlayState.Ready);
            Assert.That(board.Score.text,Is.EqualTo("70 - 0"));Assert.That(board.FirstShots.Length,Is.EqualTo(5));
            foreach(var mark in board.FirstMarks)Assert.That(mark.text,Is.EqualTo("+"));foreach(var mark in board.SecondMarks)Assert.That(mark.text,Is.EqualTo("×"));
            foreach(var text in Object.FindObjectsByType<Text>(FindObjectsSortMode.None))
                Assert.That(text.text,Is.Not.EqualTo("BİR YÖNE DOKUN").And.Not.EqualTo("Sol, orta veya sağ.").And.Not.EqualTo("SOL").And.Not.EqualTo("ORTA").And.Not.EqualTo("SAĞ").And.Not.EqualTo("ENDLESS"));
            yield return null;
        }
        [UnityTest]
        public IEnumerator ScoreboardAndSettingsStayInsideReducedSafeAreas()
        {
            var board=Object.FindFirstObjectByType<MatchScoreboard>();
            var safe=(RectTransform)board.transform.parent;safe.GetComponent<SafeAreaPanel>().enabled=false;
            safe.anchorMin=new Vector2(.08f,.06f);safe.anchorMax=new Vector2(.92f,.94f);
            Canvas.ForceUpdateCanvases();board.Fit();Canvas.ForceUpdateCanvases();
            AssertInside((RectTransform)board.transform,safe);
            var settings=Object.FindFirstObjectByType<InGameSettings>();LocalTestInput.Press(settings.OpenButton);yield return null;
            var panel=settings.Options.GetComponent<RectTransform>();var panelSafe=(RectTransform)panel.parent;
            panelSafe.GetComponent<SafeAreaPanel>().enabled=false;panelSafe.anchorMin=new Vector2(.08f,.12f);panelSafe.anchorMax=new Vector2(.92f,.88f);
            Canvas.ForceUpdateCanvases();panel.GetComponent<FitPanelToSafeArea>().Fit();Canvas.ForceUpdateCanvases();
            AssertInside(panel,panelSafe);
        }
        private static void AssertInside(RectTransform child,RectTransform parent)
        {
            var corners=new Vector3[4];child.GetWorldCorners(corners);
            foreach(var corner in corners)
            {
                var local=parent.InverseTransformPoint(corner);
                Assert.That(local.x,Is.InRange(parent.rect.xMin-.1f,parent.rect.xMax+.1f));
                Assert.That(local.y,Is.InRange(parent.rect.yMin-.1f,parent.rect.yMax+.1f));
            }
        }

        [UnityTest]
        public IEnumerator PitchFillsViewportAtBothAspectRatiosWithoutScalingActorsUnevenly()
        {
            var layout=Object.FindFirstObjectByType<GameplayStageLayout>();
            var viewport=new GameObject("Viewport",typeof(RectTransform)).GetComponent<RectTransform>();viewport.SetParent(layout.transform.parent,false);layout.transform.SetParent(viewport,false);
            foreach(var size in new[]{new Vector2(960,540),new Vector2(390,844),new Vector2(360,800)})
            {
                viewport.sizeDelta=size;Canvas.ForceUpdateCanvases();layout.Fit();
                var stage=(RectTransform)layout.transform;Assert.That(stage.rect.width*stage.localScale.x,Is.EqualTo(size.x).Within(.01f));Assert.That(stage.rect.height*stage.localScale.y,Is.EqualTo(size.y).Within(.01f));
                Assert.That(stage.localScale.x,Is.EqualTo(stage.localScale.y));
            }
            yield return null;
        }
    }
}
