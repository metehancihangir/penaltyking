using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace PenaltyKing
{
    public enum PlayState { Ready, ShowingShot, Finished, NeedsSelection, PassingPhone, ChoosingKeeper, Leaving }

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
        [SerializeField] private PixelMenuButton exit;
        [SerializeField] private Text player1Summary, player2Summary;
        public PixelMenuButton Exit => exit;
        public Text ResultTitle => resultTitle;
        public void ConfigureMatchUi(PixelMenuButton exitButton, Text first, Text second)
        { exit = exitButton; player1Summary = first; player2Summary = second; }
        public PenaltyRound Round { get; private set; }
        public PlayState State { get; private set; }
        public ShotResult? LastShot { get; private set; }
        public Text ScoreLabel => score;
        public Text ShotCounter => shotCounter;
        public Text ResultScore => resultScore;
        public bool ResultVisible => resultPanel.activeSelf;
        [SerializeField] private TurnHandoff handoff;
        public TurnHandoff Handoff => handoff;
        public void ConfigureHandoff(TurnHandoff overlay) => handoff = overlay;
        private SceneNavigator navigator;
        private GameMode mode;
        private int shotLimit;

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
            if (exit != null) exit.onClick.AddListener(GoHome);
            var state = GameManager.Instance;
            if (!state.IsLocalMultiplayer || !state.HasModeSelection)
            {
                State = PlayState.NeedsSelection;
                EnableZones(false);
                resultPanel.SetActive(true);
                resultTitle.text = "SEÇİM GEREKLİ";
                resultScore.text = "Ana menüden oyuncu ve mod seç.";
                score.text = shotCounter.text = modeLabel.text = feedback.text = directions.text = "";
                replay.gameObject.SetActive(false);
                if (player1Summary != null) player1Summary.text = player2Summary.text = "";
                return;
            }
            mode = state.SelectedMode;
            shotLimit = state.SelectedRoundShots;
            modeLabel.text = mode == GameMode.FixedRound ? "SABİT ROUND" : "ENDLESS";
            StartRound();
        }

        private void ShootLeft() => Shoot(ShotDirection.Left);
        private void ShootCenter() => Shoot(ShotDirection.Center);
        private void ShootRight() => Shoot(ShotDirection.Right);
        private void Shoot(ShotDirection direction)
        {
            if (navigator.IsLoading) return;
            if (State == PlayState.Ready)
            {
                Round.ChooseShot(direction);
                EnableZones(false);
                feedback.text = directions.text = "";
                State = PlayState.PassingPhone;
                handoff.Show(Round.KeeperPlayer, true, () =>
                {
                    State = PlayState.ChoosingKeeper;
                    feedback.text = $"PLAYER {Round.KeeperPlayer} · KURTARIŞ";
                    directions.text = "Bir yöne dokun.";
                    EnableZones(true);
                });
                return;
            }
            if (State != PlayState.ChoosingKeeper) return;
            State = PlayState.ShowingShot;
            EnableZones(false);
            var shot = Round.ChooseKeeper(direction);
            LastShot = shot;
            if (presentation != null)
            {
                feedback.text = ""; directions.text = "";
                StartCoroutine(AnimateShot(shot));
                return;
            }
            ball.anchoredPosition = new Vector2(DirectionX(shot.PlayerDirection), ballTargetY);
            ball.localScale = Vector3.one * ballFlightScale;
            keeper.anchoredPosition = new Vector2(DirectionX(shot.KeeperDirection), keeperRestPosition.y);
            feedback.text = shot.Outcome == ShotOutcome.Goal ? "GOL!" : "KURTARIŞ!";
            feedback.color = shot.Outcome == ShotOutcome.Goal ? new Color32(186, 235, 113, 255) : new Color32(255, 195, 128, 255);
            directions.text = $"Şut: {Name(shot.PlayerDirection)}  ·  Kaleci: {Name(shot.KeeperDirection)}";
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
                resultTitle.text = Round.Player1.Goals == Round.Player2.Goals ? "BERABERE" : $"PLAYER {(Round.Player1.Goals > Round.Player2.Goals ? 1 : 2)} KAZANDI";
                resultScore.text = $"{Round.Player1.Goals}  -  {Round.Player2.Goals}";
                if (player1Summary != null)
                {
                    player1Summary.text = $"PLAYER 1\n{Round.Player1.Shots} / {Round.ShotLimit} şut";
                    player2Summary.text = $"PLAYER 2\n{Round.Player2.Shots} / {Round.ShotLimit} şut";
                }
                resultPanel.SetActive(true);
            }
            else ReadyForShot();
        }

        private void Restart()
        {
            if (navigator.IsLoading || State != PlayState.Finished) return;
            StartRound();
        }

        private void StartRound()
        {
            StopAllCoroutines();
            if (presentation != null) presentation.Cancel();
            GetComponent<GameplayAudio>()?.StopCue();
            Round = new PenaltyRound(mode, shotLimit);
            handoff.Hide();
            LastShot = null;
            resultPanel.SetActive(false);
            RefreshScore();
            ReadyForShot();
        }

        private void ReadyForShot()
        {
            State = PlayState.PassingPhone;
            LastShot = null;
            ball.anchoredPosition = ballRestPosition;
            ball.localScale = Vector3.one;
            keeper.anchoredPosition = keeperRestPosition;
            if (presentation != null) presentation.ResetPose();
            feedback.text = $"PLAYER {Round.ShooterPlayer} · ŞUT";
            feedback.color = new Color32(227, 238, 230, 255);
            directions.text = "Sol, orta veya sağ.";
            EnableZones(false);
            handoff.Show(Round.ShooterPlayer, false, () =>
            {
                State = PlayState.Ready;
                EnableZones(true);
            });
        }

        private void RefreshScore()
        {
            score.text = $"P1 {Round.Player1.Goals}  -  {Round.Player2.Goals} P2";
            shotCounter.text = mode == GameMode.FixedRound ? $"ŞUT  {Round.ShotsTaken} / {Round.ShotLimit * 2}" : $"ŞUT  {Round.ShotsTaken}";
        }
        private void EnableZones(bool enabled) { left.interactable = center.interactable = right.interactable = enabled; }
        private float DirectionX(ShotDirection direction) => ((int)direction - 1) * targetSpacing;
        private static string Name(ShotDirection direction) => direction == ShotDirection.Left ? "Sol" : direction == ShotDirection.Center ? "Orta" : "Sağ";
        private void GoHome()
        {
            if (navigator.IsLoading || State == PlayState.Leaving) return;
            State = PlayState.Leaving;
            EnableZones(false);
            StopAllCoroutines();
            if (presentation != null) presentation.Cancel();
            handoff.Hide();
            GetComponent<GameplayAudio>()?.StopStadium();
            Round = null;
            LastShot = null;
            GameManager.Instance.BeginSelection();
            navigator.Navigate(GameScene.MainMenu);
        }
        private void OnDestroy()
        {
            if (presentation != null) presentation.Impact -= PresentImpact;
            left.onClick.RemoveListener(ShootLeft); center.onClick.RemoveListener(ShootCenter); right.onClick.RemoveListener(ShootRight);
            replay.onClick.RemoveListener(Restart); home.onClick.RemoveListener(GoHome);
            if (exit != null) exit.onClick.RemoveListener(GoHome);
        }
    }
}
