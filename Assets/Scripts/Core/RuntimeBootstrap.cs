using UnityEngine;

namespace PenaltyKing
{
    public static class RuntimeBootstrap
    {
        // Works when Play is pressed from any of the five scenes.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void EnsureServices()
        {
            if (GameManager.Instance == null)
                new GameObject("GameManager").AddComponent<GameManager>();
            if (AudioManager.Instance == null)
                new GameObject("AudioManager").AddComponent<AudioManager>();
        }
    }
}
