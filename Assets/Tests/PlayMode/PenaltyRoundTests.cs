using System;
using NUnit.Framework;

namespace PenaltyKing.Tests
{
    public sealed class PenaltyRoundTests
    {
        private sealed class SequenceRandom : Random
        {
            private readonly double[] values;
            private int index;
            public SequenceRandom(params double[] values) { this.values = values; }
            public override double NextDouble() => values[index++];
            public override int Next(int minValue, int maxValue) => minValue;
        }

        [Test]
        public void FixedRoundCountsMixedOutcomesAndRejectsShotsAfterItsLimit()
        {
            var round = new PenaltyRound(GameMode.FixedRound, 5, .35f, new SequenceRandom(.9, .1, .9, .9, .1));
            var expected = new[] { ShotOutcome.Goal, ShotOutcome.Save, ShotOutcome.Goal, ShotOutcome.Goal, ShotOutcome.Save };
            for (var i = 0; i < 5; i++)
            {
                var shot = round.Shoot((ShotDirection)(i % 3));
                Assert.That(shot.Outcome, Is.EqualTo(expected[i]));
                Assert.That(round.ShotsTaken, Is.EqualTo(i + 1));
                Assert.That(round.IsOver, Is.EqualTo(i == 4));
            }
            Assert.That(round.Goals, Is.EqualTo(3));
            Assert.Throws<InvalidOperationException>(() => round.Shoot(ShotDirection.Left));
            Assert.That(round.ShotsTaken, Is.EqualTo(5));
        }

        [Test]
        public void EndlessIgnoresRoundLimitAndStopsOnFirstSaveAfterStreak()
        {
            var round = new PenaltyRound(GameMode.Endless, 2, .35f, new SequenceRandom(.9, .9, .9, .9, .1));
            for (var i = 0; i < 4; i++)
            {
                round.Shoot((ShotDirection)(i % 3));
                Assert.That(round.IsOver, Is.False);
            }
            Assert.That(round.Shoot(ShotDirection.Center).Outcome, Is.EqualTo(ShotOutcome.Save));
            Assert.That(round.Goals, Is.EqualTo(4));
            Assert.That(round.ShotsTaken, Is.EqualTo(5));
            Assert.That(round.IsOver, Is.True);
            Assert.Throws<InvalidOperationException>(() => round.Shoot(ShotDirection.Right));
        }

        [Test]
        public void ConfiguredOddsApplyToEveryDirectionAndOtherDirectionsAreBalanced()
        {
            const int samples = 10000;
            foreach (var probability in new[] { .2f, .35f, .5f })
            foreach (ShotDirection direction in Enum.GetValues(typeof(ShotDirection)))
            {
                var round = new PenaltyRound(GameMode.FixedRound, samples, probability, new Random(421));
                var counts = new int[3];
                for (var i = 0; i < samples; i++)
                {
                    var shot = round.Shoot(direction);
                    counts[(int)shot.KeeperDirection]++;
                    Assert.That(shot.Outcome == ShotOutcome.Save, Is.EqualTo(shot.PlayerDirection == shot.KeeperDirection));
                }
                Assert.That(counts[(int)direction] / (float)samples, Is.EqualTo(probability).Within(.02f));
                for (var i = 1; i <= 2; i++)
                    Assert.That(counts[((int)direction + i) % 3] / (float)samples, Is.EqualTo((1 - probability) / 2).Within(.02f));
                Assert.That(round.SaveProbability, Is.EqualTo(probability));
            }
        }

        [Test]
        public void ProbabilityExtremesCustomLimitAndInvalidDirectionAreHandled()
        {
            foreach (var probability in new[] { 0f, 1f })
            {
                var round = new PenaltyRound(GameMode.FixedRound, 3, probability, new Random(17));
                Assert.Throws<ArgumentOutOfRangeException>(() => round.Shoot((ShotDirection)10));
                Assert.That(round.ShotsTaken, Is.Zero);
                foreach (ShotDirection direction in Enum.GetValues(typeof(ShotDirection)))
                    Assert.That(round.Shoot(direction).Outcome, Is.EqualTo(probability == 0 ? ShotOutcome.Goal : ShotOutcome.Save));
                Assert.That(round.Goals, Is.EqualTo(probability == 0 ? 3 : 0));
                Assert.That(round.IsOver, Is.True);
            }
        }
    }
}
