#if DEVELOPMENT_BUILD
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PenaltyKing
{
    // Development-only log telemetry; never adds controls or overlays to the product.
    public sealed class MobilePerformanceProbe : MonoBehaviour
    {
        private readonly List<float> frames = new List<float>(600);
        private float windowStart, sceneStart;
        private bool lastSummary;
        private GameplayController game;
        private void Awake()
        {
            SceneManager.sceneLoaded += Loaded;
            Debug.Log($"[MobileDevice] model={SystemInfo.deviceModel} gpu={SystemInfo.graphicsDeviceName} memoryMB={SystemInfo.systemMemorySize} resolution={Screen.width}x{Screen.height}");
        }
        private void Loaded(Scene scene, LoadSceneMode mode)
        { frames.Clear(); sceneStart = windowStart = Time.realtimeSinceStartup; lastSummary = false; game = FindFirstObjectByType<GameplayController>(); StartCoroutine(Targets()); }
        private IEnumerator Targets()
        {
            yield return new WaitForSecondsRealtime(.7f);
            Debug.Log("[MobileScene] " + SceneManager.GetActiveScene().name);
            foreach (var button in FindObjectsByType<Button>(FindObjectsSortMode.None))
            {
                if (!button.isActiveAndEnabled || !button.interactable) continue;
                var canvas = button.GetComponentInParent<Canvas>();
                var point = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, button.transform.position);
                Debug.Log($"[MobileTarget] {button.name}|{Mathf.RoundToInt(point.x)}|{Mathf.RoundToInt(Screen.height - point.y)}");
            }
        }
        private void Update()
        {
            if (SceneManager.GetActiveScene().name != "Gameplay") return;
            var now = Time.realtimeSinceStartup;
            if (now - sceneStart < 2) { windowStart = now; return; }
            frames.Add(Time.unscaledDeltaTime * 1000);
            if (now - windowStart >= 5 && frames.Count > 0)
            {
                var total = 0f; foreach (var frame in frames) total += frame;
                frames.Sort();
                Debug.Log(string.Format(CultureInfo.InvariantCulture, "[MobilePerformance] frames={0} fps={1:F2} p95Ms={2:F2} maxMs={3:F2}",
                    frames.Count, 1000 * frames.Count / total, frames[(int)((frames.Count - 1) * .95f)], frames[frames.Count - 1]));
                frames.Clear(); windowStart = now;
            }
            if (game != null && game.ResultVisible && !lastSummary) { lastSummary = true; StartCoroutine(Targets()); }
            else if (game != null && !game.ResultVisible) lastSummary = false;
        }
        private void OnDestroy() => SceneManager.sceneLoaded -= Loaded;
    }
}
#endif
