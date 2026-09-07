using UnityEngine;
namespace PenaltyKing
{
    [CreateAssetMenu(menuName = "Penalty King/Game Rules", fileName = "GameRules")]
    public sealed class GameRules : ScriptableObject
    {
        [SerializeField, Min(1)] private int fixedRoundShots = 5;
        // Number of attempts per player.
        public int FixedRoundShots => Mathf.Max(1, fixedRoundShots);
    }
}
