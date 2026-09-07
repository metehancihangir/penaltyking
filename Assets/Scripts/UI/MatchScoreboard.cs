using UnityEngine;
using UnityEngine.UI;
namespace PenaltyKing
{
    [ExecuteAlways]
    public sealed class MatchScoreboard : MonoBehaviour
    {
        [SerializeField] private GameplayController game;
        [SerializeField] private Text first, second, score, role;
        [SerializeField] private Image firstAccent, secondAccent;
        [SerializeField] private Image[] firstShots, secondShots;
        [SerializeField] private Text[] firstMarks, secondMarks;
        private int lastTotal = -1;
        private PlayState lastState;
        public Text Score => score;
        public Text Role => role;
        public Image[] FirstShots => firstShots;
        public Image[] SecondShots => secondShots;
        public Text[] FirstMarks => firstMarks;
        public Text[] SecondMarks => secondMarks;
        private static readonly Color Lime = new Color32(186,235,113,255), Coral = new Color32(240,131,111,255), Idle = new Color32(48,73,87,255);
        public void Configure(GameplayController controller, Text p1, Text p2, Text scores, Text activeRole, Image a1, Image a2, Image[] shots1, Image[] shots2, Text[] marks1, Text[] marks2)
        { game = controller; first = p1; second = p2; score = scores; role = activeRole; firstAccent = a1; secondAccent = a2; firstShots = shots1; secondShots = shots2; firstMarks = marks1; secondMarks = marks2; }
        public void Fit()
        {
            var rect = (RectTransform)transform;
            var parent = (RectTransform)rect.parent;
            var scale = Mathf.Min(1, Mathf.Max(.1f, (parent.rect.width - 20) / 760));
            rect.localScale = new Vector3(scale,scale,1);
        }
        private void LateUpdate()
        {
            Fit();
            if (!Application.isPlaying || game.Round == null) return;
            if (game.State == PlayState.ShowingShot && game.Presentation.Elapsed < ShotPresentation.ImpactTime) return;
            if (lastTotal == game.Round.ShotsTaken && lastState == game.State) return;
            Render(game.Round, game.State);
        }
        public void Render(PenaltyRound round, PlayState state)
        {
            lastTotal = round.ShotsTaken; lastState = state;
            score.text = $"{round.Player1.Goals} - {round.Player2.Goals}";
            var active = state == PlayState.ChoosingKeeper || (state == PlayState.PassingPhone && round.HasShotSelection) ? round.KeeperPlayer : round.ShooterPlayer;
            if (state == PlayState.ShowingShot) active = round.KeeperPlayer; // Shooter of the just-resolved attempt.
            var finished = state == PlayState.Finished;
            first.color = !finished && active == 1 ? Lime : Color.white;
            second.color = !finished && active == 2 ? Lime : Color.white;
            firstAccent.color = !finished && active == 1 ? Lime : Idle;
            secondAccent.color = !finished && active == 2 ? Lime : Idle;
            role.text = finished ? "MAÇ SONU" : state == PlayState.ShowingShot ? "" : round.HasShotSelection ? "KURTARIŞ" : "ŞUT";
            Draw(round.Player1, round.Mode, firstShots, firstMarks);
            Draw(round.Player2, round.Mode, secondShots, secondMarks);
        }
        private static void Draw(PlayerRoundStats stats, GameMode mode, Image[] boxes, Text[] marks)
        {
            var start = mode == GameMode.Endless ? Mathf.Max(0, stats.History.Count - boxes.Length) : 0;
            for (var i = 0; i < boxes.Length; i++)
            {
                var index = start + i;
                var exists = index < stats.History.Count;
                boxes[i].color = !exists ? Idle : stats.History[index] == ShotOutcome.Goal ? Lime : Coral;
                marks[i].text = !exists ? "" : stats.History[index] == ShotOutcome.Goal ? "+" : "×";
            }
        }
    }
}
