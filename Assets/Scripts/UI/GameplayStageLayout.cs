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
            var scale = Mathf.Max(0.01f, Mathf.Min((size.x - 24) / 960f, (size.y - 140) / 640f));
            var height = Mathf.Clamp((size.y - 140) / scale, 640, 1500);
            stage.sizeDelta = new Vector2(960, height);
            stage.localScale = new Vector3(scale, scale, 1);
            if (controller == null || Mathf.Approximately(lastHeight, height)) return;
            lastHeight = height;
            var extra = (height - 640) * .5f;
            sky.anchoredPosition = new Vector2(0, 254.375f + extra);
            stands.anchoredPosition = new Vector2(0, 99.6875f + extra);
            crowd.anchoredPosition = new Vector2(0, 57 + extra);
            pitch.sizeDelta = new Vector2(960, 330.625f + height - 640);
            pitch.anchoredPosition = new Vector2(0, -154.6875f);
            goal.anchoredPosition = new Vector2(0, 94 + extra);
            zones.anchoredPosition = new Vector2(0, extra);
            shooter.anchoredPosition = new Vector2(-100, -224 - extra);
            controller.ConfigureComposition(new Vector2(0, -213 - extra), new Vector2(0, 37 + extra), 190, 58 + extra, .55f);
        }
        private void OnEnable() { lastHeight = -1; Fit(); }
        private void LateUpdate() => Fit();
    }
}
