using UnityEngine;

namespace PenaltyKing
{
    // Portrait extends the pitch; goal and character sprites keep their proportions.
    [ExecuteAlways, RequireComponent(typeof(RectTransform))]
    public sealed class GameplayStageLayout : MonoBehaviour
    {
        [SerializeField] private RectTransform sky, stands, crowd, pitch, goal, shooter, zones;
        [SerializeField] private GameplayController controller;
        private float lastHeight = -1;
        public void Configure(RectTransform skyLayer, RectTransform standLayer, RectTransform crowdLayer,
            RectTransform pitchLayer, RectTransform goalLayer, RectTransform shooterLayer, RectTransform touchLayer, GameplayController gameplay)
        {
            sky = skyLayer; stands = standLayer; crowd = crowdLayer; pitch = pitchLayer;
            goal = goalLayer; shooter = shooterLayer; zones = touchLayer; controller = gameplay;
            lastHeight = -1;
        }
        public void Fit()
        {
            var stage = (RectTransform)transform;
            var parent = transform.parent as RectTransform;
            if (parent == null) return;
            var size = parent.rect.size;
            var scale = Mathf.Max(.01f, size.x / 960f);
            var height = size.y / scale;
            stage.sizeDelta = new Vector2(960, height);
            stage.localScale = new Vector3(scale, scale, 1);
            if (controller == null || Mathf.Approximately(lastHeight, height)) return;
            lastHeight = height;
            var extra = (height - 640) * .5f;
            var headerSpace = Mathf.Max(0, (height / 960f - .65f) * 70);
            sky.sizeDelta = new Vector2(960, 131.25f + headerSpace);
            sky.anchoredPosition = new Vector2(0, 254.375f + extra - headerSpace * .5f);
            stands.anchoredPosition = new Vector2(0, 99.6875f + extra - headerSpace);
            crowd.anchoredPosition = new Vector2(0, 57 + extra - headerSpace);
            pitch.sizeDelta = new Vector2(960, 330.625f + height - 640 - headerSpace);
            pitch.anchoredPosition = new Vector2(0, -154.6875f - headerSpace * .5f);
            goal.anchoredPosition = new Vector2(0, 94 + extra - headerSpace);
            zones.anchoredPosition = new Vector2(0, extra - headerSpace);
            var ballY = -height * .5f + Mathf.Lerp(110, 180, Mathf.Clamp01((height - 640) / 1100));
            shooter.anchoredPosition = new Vector2(-100, ballY - 11);
            controller.ConfigureComposition(new Vector2(0, ballY), new Vector2(0, 37 + extra - headerSpace), 190, 58 + extra - headerSpace, .55f);
        }
        private void OnEnable() { lastHeight = -1; Fit(); }
        private void LateUpdate() => Fit();
    }
}
