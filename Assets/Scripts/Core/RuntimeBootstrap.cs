using UnityEngine;

namespace PenaltyKing
{
    public static class RuntimeBootstrap
    {
        // Works when Play is pressed from any of the five scenes.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void EnsureServices()
        {
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
            if (GameManager.Instance == null)
                new GameObject("GameManager").AddComponent<GameManager>();
            if (AudioManager.Instance == null)
                new GameObject("AudioManager").AddComponent<AudioManager>();
            if (SceneTransition.Instance == null)
                new GameObject("Scene Transition").AddComponent<SceneTransition>();
#if DEVELOPMENT_BUILD
            if (Object.FindFirstObjectByType<MobilePerformanceProbe>() == null)
                GameManager.Instance.gameObject.AddComponent<MobilePerformanceProbe>();
#endif
        }
    }
}
