using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PenaltyKing
{
    // Survives scene unload, so fade-in also shields the new screen from queued taps.
    public sealed class SceneTransition : MonoBehaviour
    {
        public static SceneTransition Instance { get; private set; }
        public static bool IsBusy => Instance != null && Instance.busy;
        public float Opacity => overlay.alpha;
        private CanvasGroup overlay;
        private bool busy;
        private string expectedScene;
        private const float FadeSeconds = .16f;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics() => Instance = null;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this; DontDestroyOnLoad(gameObject);
            var canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay; canvas.sortingOrder = 32767;
            gameObject.AddComponent<GraphicRaycaster>();
            overlay = gameObject.AddComponent<CanvasGroup>();
            var image = new GameObject("Transition Shade", typeof(RectTransform), typeof(Image)).GetComponent<Image>();
            image.transform.SetParent(transform, false);
            image.rectTransform.anchorMin = Vector2.zero; image.rectTransform.anchorMax = Vector2.one;
            image.rectTransform.offsetMin = image.rectTransform.offsetMax = Vector2.zero;
            image.color = new Color32(7, 18, 34, 255);
            Clear(); SceneManager.sceneLoaded += SceneLoaded;
        }

        public void Navigate(GameScene destination)
        {
            if (busy) return;
            busy = true; overlay.blocksRaycasts = true;
            expectedScene = destination.ToString();
            StartCoroutine(Transition());
        }

        private IEnumerator Transition()
        {
            try
            {
                yield return Fade(0, 1);
                yield return SceneManager.LoadSceneAsync(expectedScene);
                yield return null; // Let the new layout and input module initialize while covered.
                yield return Fade(1, 0);
            }
            finally { Clear(); }
        }

        private IEnumerator Fade(float from, float to)
        {
            for (var elapsed = 0f; elapsed < FadeSeconds; elapsed += Time.unscaledDeltaTime)
            { overlay.alpha = Mathf.Lerp(from, to, Mathf.SmoothStep(0, 1, elapsed / FadeSeconds)); yield return null; }
            overlay.alpha = to;
        }

        private void SceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Direct editor/test loads can interrupt navigation. Never strand a blocking overlay.
            if (busy && mode == LoadSceneMode.Single && scene.name != expectedScene)
            { StopAllCoroutines(); Clear(); }
        }

        private void Clear() { busy = false; expectedScene = null; overlay.alpha = 0; overlay.blocksRaycasts = false; }
        private void OnDestroy()
        { SceneManager.sceneLoaded -= SceneLoaded; if (Instance == this) Instance = null; }
    }
}
