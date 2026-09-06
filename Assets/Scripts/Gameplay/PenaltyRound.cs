using System;

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

    // Pure round logic: no timing, UI, animation or mutable difficulty configuration.
    public sealed class PenaltyRound
    {
        private readonly Random random;
        public GameMode Mode { get; }
        public int ShotLimit { get; }
        public float SaveProbability { get; }
        public int ShotsTaken { get; private set; }
        public int Goals { get; private set; }
        public bool IsOver { get; private set; }

        public PenaltyRound(GameMode mode, int shotLimit, float saveProbability, Random random = null)
        {
            if (!Enum.IsDefined(typeof(GameMode), mode)) throw new ArgumentOutOfRangeException(nameof(mode));
            if (shotLimit < 1) throw new ArgumentOutOfRangeException(nameof(shotLimit));
            if (float.IsNaN(saveProbability) || saveProbability < 0 || saveProbability > 1)
                throw new ArgumentOutOfRangeException(nameof(saveProbability));
            Mode = mode;
            ShotLimit = shotLimit;
            SaveProbability = saveProbability;
            this.random = random ?? new Random();
        }

        public ShotResult Shoot(ShotDirection direction)
        {
            if (IsOver) throw new InvalidOperationException("The round has ended.");
            if (!Enum.IsDefined(typeof(ShotDirection), direction)) throw new ArgumentOutOfRangeException(nameof(direction));
            // First decide whether the keeper guesses correctly. On a miss, choose
            // equally between the OTHER two directions, so configured odds are exact.
            var keeper = random.NextDouble() < SaveProbability ? direction
                : (ShotDirection)(((int)direction + random.Next(1, 3)) % 3);
            var result = new ShotResult(direction, keeper);
            ShotsTaken++;
            if (result.Outcome == ShotOutcome.Goal) Goals++;
            IsOver = Mode == GameMode.FixedRound ? ShotsTaken >= ShotLimit : result.Outcome == ShotOutcome.Save;
            return result;
        }
    }
}
