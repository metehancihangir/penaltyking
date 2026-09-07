using System;
using System.Collections.Generic;

namespace PenaltyKing
{
    public enum ShotDirection { Left, Center, Right }
    public enum ShotOutcome { Goal, Save }

    public readonly struct ShotResult
    {
        public ShotDirection PlayerDirection { get; }
        public ShotDirection KeeperDirection { get; }
        public ShotOutcome Outcome { get; }
        public ShotResult(ShotDirection player, ShotDirection keeper)
        {
            PlayerDirection = player;
            KeeperDirection = keeper;
            Outcome = player == keeper ? ShotOutcome.Save : ShotOutcome.Goal;
        }
    }

    public sealed class PlayerRoundStats
    {
        private readonly List<ShotOutcome> history = new List<ShotOutcome>();
        public IReadOnlyList<ShotOutcome> History { get; }
        public int Goals { get; private set; }
        public int Shots { get; private set; }
        public PlayerRoundStats() => History = history.AsReadOnly();
        internal void Record(ShotOutcome outcome)
        {
            Shots++;
            if (outcome == ShotOutcome.Goal) Goals++;
            // Endless sessions keep bounded presentation history, independent of totals.
            if (history.Count == 64) history.RemoveAt(0);
            history.Add(outcome);
        }
    }

    // Both directions are supplied by people. The first choice remains private until resolution.
    public sealed class PenaltyRound
    {
        private ShotDirection? pendingShot;
        public GameMode Mode { get; }
        public int ShotLimit { get; }
        public PlayerRoundStats Player1 { get; } = new PlayerRoundStats();
        public PlayerRoundStats Player2 { get; } = new PlayerRoundStats();
        public int ShotsTaken => Player1.Shots + Player2.Shots;
        public int ShooterPlayer => ShotsTaken % 2 + 1;
        public int KeeperPlayer => 3 - ShooterPlayer;
        public bool HasShotSelection => pendingShot.HasValue;
        public bool IsOver => Mode == GameMode.FixedRound && Player1.Shots >= ShotLimit && Player2.Shots >= ShotLimit;

        public PenaltyRound(GameMode mode, int shotLimit)
        {
            if (!Enum.IsDefined(typeof(GameMode), mode)) throw new ArgumentOutOfRangeException(nameof(mode));
            if (shotLimit < 1) throw new ArgumentOutOfRangeException(nameof(shotLimit));
            Mode = mode; ShotLimit = shotLimit;
        }

        public void ChooseShot(ShotDirection direction)
        {
            Validate(direction);
            if (IsOver || pendingShot.HasValue) throw new InvalidOperationException("Not awaiting a shot.");
            pendingShot = direction;
        }

        public ShotResult ChooseKeeper(ShotDirection direction)
        {
            Validate(direction);
            if (IsOver || !pendingShot.HasValue) throw new InvalidOperationException("Not awaiting a keeper.");
            var result = new ShotResult(pendingShot.Value, direction);
            pendingShot = null;
            (ShooterPlayer == 1 ? Player1 : Player2).Record(result.Outcome);
            return result;
        }

        private static void Validate(ShotDirection direction)
        {
            if (!Enum.IsDefined(typeof(ShotDirection), direction)) throw new ArgumentOutOfRangeException(nameof(direction));
        }
    }
}
