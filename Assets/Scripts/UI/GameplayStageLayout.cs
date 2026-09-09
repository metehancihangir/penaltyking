using UnityEngine;

namespace PenaltyKing
{
    // Keep the penalty action compact; extra portrait space belongs to the background.
    [ExecuteAlways, RequireComponent(typeof(RectTransform))]
    public sealed class GameplayStageLayout : MonoBehaviour
    {
        [SerializeField] private RectTransform sky, stands, crowd, pitch, goal, shooter, zones;
        [SerializeField] private GameplayController controller;
        private float lastHeight = -1, lastWidth = -1;
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
            var scale = Mathf.Max(.01f, Mathf.Min(size.x / 960f, size.y / 540f));
            var height = size.y / scale;
            var width = size.x / scale;
            stage.sizeDelta = new Vector2(width, height);
            stage.localScale = new Vector3(scale, scale, 1);
            if (controller == null || (Mathf.Approximately(lastHeight, height) && Mathf.Approximately(lastWidth,width))) return;
            lastHeight = height; lastWidth = width;
            var extra = (height - 640) * .5f;
            var headerSpace = Mathf.Max(0, (height / 960f - .65f) * 70);
            sky.sizeDelta = new Vector2(width, 131.25f + headerSpace);
            stands.sizeDelta = new Vector2(width, stands.sizeDelta.y);
            var sections=stands.Find("Animated Sections");
            if(sections!=null) sections.localScale=new Vector3(width/960f,1,1);
            sky.anchoredPosition = new Vector2(0, 254.375f + extra - headerSpace * .5f);
            stands.anchoredPosition = new Vector2(0, 99.6875f + extra - headerSpace);
            crowd.anchoredPosition = new Vector2(0, 57 + extra - headerSpace);
            pitch.sizeDelta = new Vector2(width, 330.625f + height - 640 - headerSpace);
            pitch.anchoredPosition = new Vector2(0, -154.6875f - headerSpace * .5f);
            goal.anchoredPosition = new Vector2(0, 72.4f + extra - headerSpace);
            zones.anchoredPosition = new Vector2(0, extra - headerSpace);
            var ballY = -height * .5f + Mathf.Lerp(137, 240, Mathf.Clamp01((height - 640) / 1100));
            var actorHeight = Mathf.Lerp(210, 240, Mathf.Clamp01((height - 640) / 1100));
            if (controller.Presentation != null) controller.Presentation.ConfigureActorScale(actorHeight, .94f);
            pitch.GetComponent<PixelPitch>()?.SetSpot(ballY - 12 - pitch.anchoredPosition.y);
            shooter.anchoredPosition = new Vector2(-100 * actorHeight / 160, ballY - 11 * actorHeight / 160);
            controller.ConfigureComposition(new Vector2(0, ballY), new Vector2(0, 33.58f + extra - headerSpace), 152, 43.6f + extra - headerSpace, .55f);
            var portrait = Mathf.Clamp01((height - 960) / 480);
            goal.localScale = Vector3.one * .95f;
            goal.anchoredPosition += new Vector2(0, -86.4f * .05f);
            zones.localScale = Vector3.one * .95f;
            zones.anchoredPosition += new Vector2(0, -3);
            controller.ConfigureComposition(new Vector2(0, ballY), new Vector2(0, 33.58f + extra - headerSpace), 152 * .95f, 43.6f + extra - headerSpace - 3, .55f);
            if (portrait > 0)
            {
                var zoom = Mathf.Lerp(1, 1.45f, portrait);
                var goalY = Mathf.Lerp(goal.anchoredPosition.y, height * .20f, portrait);
                var groundY = goalY - 86.4f * zoom;
                ballY = Mathf.Lerp(ballY, groundY - 250, portrait);
                actorHeight = Mathf.Lerp(actorHeight, 310, portrait);
                goal.anchoredPosition = new Vector2(0, goalY);
                goal.localScale = Vector3.one * zoom;
                zones.anchoredPosition = new Vector2(0, goalY - 72.4f * zoom);
                zones.localScale = Vector3.one * zoom;
                stands.anchoredPosition = new Vector2(0, groundY + 113.6875f);
                var skyBottom = stands.anchoredPosition.y + stands.rect.height * .5f;
                sky.sizeDelta = new Vector2(960, height * .5f - skyBottom);
                sky.anchoredPosition = new Vector2(0, (height * .5f + skyBottom) * .5f);
                var pitchTop = groundY + 24.625f;
                pitch.sizeDelta = new Vector2(960, pitchTop + height * .5f);
                pitch.anchoredPosition = new Vector2(0, (pitchTop - height * .5f) * .5f);
                pitch.GetComponent<PixelPitch>()?.SetSpot(ballY - 12 - pitch.anchoredPosition.y);
                controller.Presentation?.ConfigureActorScale(actorHeight, .94f * zoom);
                controller.ConfigureComposition(new Vector2(0, ballY), new Vector2(0, groundY + 47.58f * zoom), 152 * zoom, groundY + 57.6f * zoom, .55f);
            }
            pitch.GetComponent<PixelPitch>()?.SetGoalScale(goal.localScale.x);
        }
        private void OnEnable() { lastHeight = -1; Fit(); }
        private void LateUpdate() => Fit();
    }
}
