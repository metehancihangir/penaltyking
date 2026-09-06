using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace PenaltyKing
{
    public enum PlayState { Ready, ShowingShot, Finished, NeedsSelection }

    [RequireComponent(typeof(SceneNavigator))]
    public sealed class GameplayController : MonoBehaviour
    {
        [SerializeField] private ShotZone left, center, right;
        [SerializeField] private Text score, shotCounter, modeLabel, feedback, directions;
        [SerializeField] private RectTransform ball, keeper;
        [SerializeField] private GameObject resultPanel;
        [SerializeField] private Text resultTitle, resultScore;
        [SerializeField] private PixelMenuButton replay, home;
        [SerializeField, Min(0.1f)] private float resultHoldSeconds = 0.8f;
        [SerializeField] private Vector2 ballRestPosition = new Vector2(0, -58);
        [SerializeField] private Vector2 keeperRestPosition = new Vector2(0, 80);
        [SerializeField] private float targetSpacing = 128;
        [SerializeField] private float ballTargetY = 35;
        [SerializeField] private float ballFlightScale = 1;
        [SerializeField] private ShotPresentation presentation;
        public ShotPresentation Presentation => presentation;
        public void ConfigurePresentation(ShotPresentation value) => presentation = value;
        public ShotZone Left => left;
        public ShotZone Center => center;
        public ShotZone Right => right;
        public PixelMenuButton Replay => replay;
        public PixelMenuButton Home => home;
        public PenaltyRound Round { get; private set; }
        public PlayState State { get; private set; }
        public ShotResult? LastShot { get; private set; }
        public Text ScoreLabel => score;
        public Text ShotCounter => shotCounter;
        public Text ResultScore => resultScore;
        public bool ResultVisible => resultPanel.activeSelf;
        private readonly System.Random random = new System.Random();
        private SceneNavigator navigator;
        private GameMode mode;
        private int shotLimit;
        private float saveProbability;

        public void ConfigureComposition(Vector2 ballRest, Vector2 keeperRest, float spacing, float targetY, float flightScale)
        {
            ballRestPosition = ballRest; keeperRestPosition = keeperRest;
            targetSpacing = spacing; ballTargetY = targetY; ballFlightScale = flightScale;
            if (presentation != null) { presentation.SetComposition(ballRest, keeperRest, spacing, targetY); return; }
            if (ball == null || keeper == null) return;
            var showing = LastShot.HasValue && State != PlayState.Ready;
            ball.anchoredPosition = showing ? new Vector2(DirectionX(LastShot.Value.PlayerDirection), ballTargetY) : ballRestPosition;
            keeper.anchoredPosition = new Vector2(showing ? DirectionX(LastShot.Value.KeeperDirection) : 0, keeperRestPosition.y);
        }

        public void Configure(ShotZone l, ShotZone c, ShotZone r, Text scoreText, Text shotText, Text modeText,
            Text feedbackText, Text directionsText, RectTransform ballMarker, RectTransform keeperMarker,
            GameObject summary, Text summaryTitle, Text summaryScore, PixelMenuButton retry, PixelMenuButton mainMenu)
        {
            left = l; center = c; right = r; score = scoreText; shotCounter = shotText; modeLabel = modeText;
            feedback = feedbackText; directions = directionsText; ball = ballMarker; keeper = keeperMarker;
            resultPanel = summary; resultTitle = summaryTitle; resultScore = summaryScore; replay = retry; home = mainMenu;
        }

        private void Awake()
        {
            navigator = GetComponent<SceneNavigator>();
            if (presentation != null)
            {
                presentation.SetComposition(ballRestPosition, keeperRestPosition, targetSpacing, ballTargetY);
                presentation.Impact += PresentImpact;
            }
            left.onClick.AddListener(ShootLeft); center.onClick.AddListener(ShootCenter); right.onClick.AddListener(ShootRight);
            replay.onClick.AddListener(Restart); home.onClick.AddListener(GoHome);
            var state = GameManager.Instance;
            if (!state.HasDifficultySelection || !state.HasModeSelection)
            {
                State = PlayState.NeedsSelection;
                EnableZones(false);
                resultPanel.SetActive(true);
                resultTitle.text = "SEÇİM GEREKLİ";
                resultScore.text = "Ana menüden zorluk ve mod seç.";
                replay.gameObject.SetActive(false);
                return;
            }
            mode = state.SelectedMode;
            shotLimit = state.SelectedRoundShots;
            saveProbability = state.SelectedSaveProbability;
            modeLabel.text = $"{(mode == GameMode.FixedRound ? "SABİT ROUND" : "ENDLESS")} · {ModeSelectController.DifficultyName(state.SelectedDifficulty)}";
            Restart();
        }

        private void ShootLeft() => Shoot(ShotDirection.Left);
        private void ShootCenter() => Shoot(ShotDirection.Center);
        private void ShootRight() => Shoot(ShotDirection.Right);
        private void Shoot(ShotDirection direction)
        {
            if (State != PlayState.Ready || navigator.IsLoading) return;
            State = PlayState.ShowingShot;
            EnableZones(false);
            // Both directions are committed in the same input callback, before visual feedback.
            var shot = Round.Shoot(direction);
            LastShot = shot;
            if (presentation != null)
            {
                feedback.text = ""; directions.text = "";
                StartCoroutine(AnimateShot(shot));
                return;
            }
            ball.anchoredPosition = new Vector2(DirectionX(direction), ballTargetY);
            ball.localScale = Vector3.one * ballFlightScale;
            keeper.anchoredPosition = new Vector2(DirectionX(shot.KeeperDirection), keeperRestPosition.y);
            feedback.text = shot.Outcome == ShotOutcome.Goal ? "GOL!" : "KURTARIŞ!";
            feedback.color = shot.Outcome == ShotOutcome.Goal ? new Color32(186, 235, 113, 255) : new Color32(255, 195, 128, 255);
            directions.text = $"Şut: {Name(direction)}  ·  Kaleci: {Name(shot.KeeperDirection)}";
            RefreshScore();
            StartCoroutine(ShowResult());
        }

        private void PresentImpact(ShotResult shot)
        {
            feedback.text = shot.Outcome == ShotOutcome.Goal ? "GOL!" : "KURTARIŞ!";
            feedback.color = shot.Outcome == ShotOutcome.Goal ? new Color32(186, 235, 113, 255) : new Color32(255, 195, 128, 255);
            directions.text = $"Şut: {Name(shot.PlayerDirection)}  ·  Kaleci: {Name(shot.KeeperDirection)}";
            RefreshScore();
        }

        private IEnumerator AnimateShot(ShotResult shot)
        {
            yield return presentation.Play(shot);
            FinishShot();
        }

        private IEnumerator ShowResult()
        {
            // Readability hold only. Final ball, player and keeper animations belong to Phase 6.
            yield return new WaitForSecondsRealtime(resultHoldSeconds);
            FinishShot();
        }

        private void FinishShot()
        {
            if (Round.IsOver)
            {
                State = PlayState.Finished;
                resultTitle.text = mode == GameMode.FixedRound ? "ROUND TAMAMLANDI" : "SERİ SONA ERDİ";
                resultScore.text = mode == GameMode.FixedRound ? $"{Round.Goals} / {Round.ShotLimit} GOL" : $"{Round.Goals} GOL";
                resultPanel.SetActive(true);
            }
            else ReadyForShot();
        }

        private void Restart()
        {
            if (navigator.IsLoading || State == PlayState.NeedsSelection) return;
            StopAllCoroutines();
            if (presentation != null) presentation.Cancel();
            Round = new PenaltyRound(mode, shotLimit, saveProbability, random);
            LastShot = null;
            resultPanel.SetActive(false);
            RefreshScore();
            ReadyForShot();
        }

        private void ReadyForShot()
        {
            State = PlayState.Ready;
            ball.anchoredPosition = ballRestPosition;
            ball.localScale = Vector3.one;
            keeper.anchoredPosition = keeperRestPosition;
            if (presentation != null) presentation.ResetPose();
            feedback.text = "BİR YÖNE DOKUN";
            feedback.color = new Color32(227, 238, 230, 255);
            directions.text = "Sol, orta veya sağ.";
            EnableZones(true);
        }

        private void RefreshScore()
        {
            score.text = $"GOL  {Round.Goals}";
            shotCounter.text = mode == GameMode.FixedRound ? $"ŞUT  {Round.ShotsTaken} / {Round.ShotLimit}" : $"ŞUT  {Round.ShotsTaken}";
        }
        private void EnableZones(bool enabled) { left.interactable = center.interactable = right.interactable = enabled; }
        private float DirectionX(ShotDirection direction) => ((int)direction - 1) * targetSpacing;
        private static string Name(ShotDirection direction) => direction == ShotDirection.Left ? "Sol" : direction == ShotDirection.Center ? "Orta" : "Sağ";
        private void GoHome() { if (!navigator.IsLoading) { EnableZones(false); navigator.Navigate(GameScene.MainMenu); } }
        private void OnDestroy()
        {
            if (presentation != null) presentation.Impact -= PresentImpact;
            left.onClick.RemoveListener(ShootLeft); center.onClick.RemoveListener(ShootCenter); right.onClick.RemoveListener(ShootRight);
            replay.onClick.RemoveListener(Restart); home.onClick.RemoveListener(GoHome);
        }
    }
}
