using System;
using UnityEngine;

namespace PenaltyKing
{
    [CreateAssetMenu(menuName = "Penalty King/Game Rules", fileName = "GameRules")]
    public sealed class GameRules : ScriptableObject
    {
        [SerializeField, Range(0f, 1f)] private float easySaveProbability = 0.20f;
        [SerializeField, Range(0f, 1f)] private float mediumSaveProbability = 0.35f;
        [SerializeField, Range(0f, 1f)] private float hardSaveProbability = 0.50f;
        [SerializeField, Min(1)] private int fixedRoundShots = 5;
        public int FixedRoundShots => Mathf.Max(1, fixedRoundShots);

        public float SaveProbability(Difficulty difficulty)
        {
            switch (difficulty)
            {
                case Difficulty.Easy: return Mathf.Clamp01(easySaveProbability);
                case Difficulty.Medium: return Mathf.Clamp01(mediumSaveProbability);
                case Difficulty.Hard: return Mathf.Clamp01(hardSaveProbability);
                default: throw new ArgumentOutOfRangeException(nameof(difficulty));
            }
        }
    }
}
