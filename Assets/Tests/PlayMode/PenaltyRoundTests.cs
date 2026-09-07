using System;
using NUnit.Framework;
namespace PenaltyKing.Tests
{
    public sealed class PenaltyRoundTests
    {
        [Test]
        public void EveryHumanDirectionPairResolvesWithoutRandomness()
        {
            foreach (ShotDirection shot in Enum.GetValues(typeof(ShotDirection)))
            foreach (ShotDirection keeper in Enum.GetValues(typeof(ShotDirection)))
            {
                var round = new PenaltyRound(GameMode.FixedRound, 5);
                round.ChooseShot(shot);
                Assert.That(round.ShotsTaken, Is.Zero);
                var result = round.ChooseKeeper(keeper);
                Assert.That(result.PlayerDirection, Is.EqualTo(shot));
                Assert.That(result.KeeperDirection, Is.EqualTo(keeper));
                Assert.That(result.Outcome, Is.EqualTo(shot == keeper ? ShotOutcome.Save : ShotOutcome.Goal));
                Assert.That(round.Player1.History[0], Is.EqualTo(result.Outcome));
                Assert.That(round.Player2.Shots, Is.Zero);
                Assert.That(round.ShooterPlayer, Is.EqualTo(2));
            }
        }
        [Test]
        public void RolesAlternateAndScoresBelongToTheShooter()
        {
            var round = new PenaltyRound(GameMode.FixedRound, 5);
            for (var i = 0; i < 10; i++)
            {
                Assert.That(round.ShooterPlayer, Is.EqualTo(i % 2 + 1));
                Assert.That(round.KeeperPlayer, Is.EqualTo(2 - i % 2));
                round.ChooseShot(ShotDirection.Left);
                round.ChooseKeeper(i % 2 == 0 ? ShotDirection.Right : ShotDirection.Left);
                Assert.That(round.IsOver, Is.EqualTo(i == 9));
            }
            Assert.That(round.Player1.Goals, Is.EqualTo(5));
            Assert.That(round.Player2.Goals, Is.Zero);
            Assert.That(round.Player1.Shots, Is.EqualTo(5));
            Assert.That(round.Player2.Shots, Is.EqualTo(5));
            Assert.Throws<InvalidOperationException>(() => round.ChooseShot(ShotDirection.Left));
        }
        [Test]
        public void InvalidOrderAndDuplicateChoicesCannotChangeScoreOrHiddenChoice()
        {
            var round = new PenaltyRound(GameMode.Endless, 5);
            Assert.Throws<InvalidOperationException>(() => round.ChooseKeeper(ShotDirection.Left));
            Assert.Throws<ArgumentOutOfRangeException>(() => round.ChooseShot((ShotDirection)7));
            round.ChooseShot(ShotDirection.Right);
            Assert.Throws<InvalidOperationException>(() => round.ChooseShot(ShotDirection.Left));
            Assert.Throws<ArgumentOutOfRangeException>(() => round.ChooseKeeper((ShotDirection)7));
            Assert.That(round.ChooseKeeper(ShotDirection.Right).Outcome, Is.EqualTo(ShotOutcome.Save));
            Assert.Throws<InvalidOperationException>(() => round.ChooseKeeper(ShotDirection.Left));
            Assert.That(round.ShotsTaken, Is.EqualTo(1));
        }
        [Test]
        public void EndlessKeepsPlayingAndHistoryStorageIsBounded()
        {
            var round = new PenaltyRound(GameMode.Endless, 5);
            for (var i = 0; i < 1000; i++) { round.ChooseShot(ShotDirection.Center); round.ChooseKeeper(ShotDirection.Center); }
            Assert.That(round.IsOver, Is.False);
            Assert.That(round.Player1.Shots, Is.EqualTo(500));
            Assert.That(round.Player2.Shots, Is.EqualTo(500));
            Assert.That(round.Player1.History.Count, Is.EqualTo(64));
            Assert.That(round.Player2.History.Count, Is.EqualTo(64));
        }
    }
}
