using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace PenaltyKing
{
    public enum ShotAnimationPhase { Idle, RunUp, Flight, Impact, Landing, Recovery }

    // One unscaled timeline owns poses, trajectory and sound hooks. Sampling is deterministic
    // so previews and tests see the same frames as the running game.
    public sealed class ShotPresentation : MonoBehaviour
    {
        public const float ContactTime = .24f;
        public const float ImpactTime = .9f;
        public const float Duration = 2.2f;
        public const float GoalCelebrationDuration = 5f;
        public static float DurationFor(ShotResult shot) => shot.Outcome == ShotOutcome.Goal ? ImpactTime + GoalCelebrationDuration : Duration;
        private GoalCelebration goalCelebration;
        public bool GoalCelebrating => goalCelebration != null && goalCelebration.Visible;
        [SerializeField] private Image ball, keeper, shooter, shadow;
        [SerializeField] private RectTransform stage, crowd;
        [SerializeField] private Image[] trail, dust, confetti;
        [SerializeField] private Sprite keeperIdle, shooterIdle;
        [SerializeField] private Sprite[] shooterFrames, sideFrames, centerFrames;
        private Vector2 ballRest, keeperRest, shooterRest, crowdRest, stageRest;
        private CrowdCelebration allFans;
        private GoalNetRipple net;
        private float spacing = 190, targetY = 58;
        [SerializeField] private float shooterHeight = 160, keeperSize = 1;
        public float ShooterHeight => shooterHeight;
        public Vector2 ShooterPosition => shooter.rectTransform.anchoredPosition;
        public Vector2 KeeperPosition => keeper.rectTransform.anchoredPosition;
        public void ConfigureActorScale(float height, float keeperScale)
        { shooterHeight = height; keeperSize = keeperScale; }
        private ShotResult current;
        private bool initialized;
        private int generation;
        public bool IsPlaying { get; private set; }
        public bool IsPaused { get; private set; }
        public void SetPaused(bool paused) => IsPaused = paused;
        public ShotAnimationPhase Phase { get; private set; }
        public float Elapsed { get; private set; }
        public Vector2 BallPosition => ball.rectTransform.anchoredPosition;
        public Vector2 BallRestPosition => ballRest;
        public Sprite KeeperSprite => keeper.sprite;
        public Vector3 KeeperScale => keeper.rectTransform.localScale;
        public bool CrowdCelebrating { get; private set; }
        public event Action Kick;
        public event Action<ShotResult> Impact;
        public event Action Completed;

        public void Configure(Image football, Image goalkeeper, Image player, RectTransform stadium, RectTransform fans,
            Image ballShadow, Image[] ballTrail, Image[] kickDust, Image[] celebration, Sprite[] playerPoses, Sprite[] sidePoses, Sprite[] centralPoses)
        {
            ball = football; keeper = goalkeeper; shooter = player; stage = stadium; crowd = fans;
            shadow = ballShadow; trail = ballTrail; dust = kickDust; confetti = celebration;
            keeperIdle = keeper.sprite; shooterIdle = shooter.sprite;
            shooterFrames = playerPoses; sideFrames = sidePoses; centerFrames = centralPoses;
        }

        public void SetComposition(Vector2 ballStart, Vector2 keeperStart, float distance, float goalY)
        {
            if (allFans == null) allFans = stage.GetComponentInChildren<CrowdCelebration>();
            if (net == null) net = stage.GetComponentInChildren<GoalNetRipple>();
            if (goalCelebration == null)
            {
                goalCelebration = stage.GetComponentInChildren<GoalCelebration>();
                if (goalCelebration == null)
                {
                    var overlay = new GameObject("GOAL Celebration", typeof(RectTransform), typeof(GoalCelebration));
                    overlay.transform.SetParent(stage, false);
                    goalCelebration = overlay.GetComponent<GoalCelebration>();
                    goalCelebration.rectTransform.sizeDelta = new Vector2(550, 160);
                }
            }
            goalCelebration.rectTransform.anchoredPosition = new Vector2(0, goalY + 70);
            ballRest = ballStart; keeperRest = keeperStart; spacing = distance; targetY = goalY;
            shooterRest = new Vector2(-100 * shooterHeight / 160, ballStart.y - 11 * shooterHeight / 160);
            crowdRest = new Vector2(0, keeperStart.y + 20);
            if (!initialized) { stageRest = stage.anchoredPosition; initialized = true; }
            if (IsPlaying) Sample(current, Elapsed); else ResetPose();
        }

        public IEnumerator Play(ShotResult shot)
        {
            ResetPose(); current = shot; IsPlaying = true;
            var runGeneration = ++generation;
            var kicked = false; var impacted = false;
            var duration = DurationFor(shot);
            for (var time = 0f; time < duration; time += IsPaused ? 0 : Time.unscaledDeltaTime)
            {
                if (runGeneration != generation) yield break;
                if (IsPaused) { yield return null; continue; }
                Sample(shot, time);
                if (!kicked && time >= ContactTime) { kicked = true; Kick?.Invoke(); }
                if (!impacted && time >= ImpactTime) { impacted = true; Impact?.Invoke(shot); }
                yield return null;
            }
            if (runGeneration != generation) yield break;
            // A long frame can cross both event boundaries. Never lose or duplicate either cue.
            if (!kicked) Kick?.Invoke();
            if (!impacted) Impact?.Invoke(shot);
            Sample(shot, duration);
            IsPlaying = false; Phase = ShotAnimationPhase.Idle;
            ResetPose(); Completed?.Invoke();
        }

        public void Cancel() { IsPaused = false; generation++; IsPlaying = false; ResetPose(); }

        public void ResetPose()
        {
            if (ball == null) return;
            Phase = ShotAnimationPhase.Idle; Elapsed = 0; CrowdCelebrating = false;
            ball.rectTransform.anchoredPosition = ballRest;
            ball.rectTransform.localScale = Vector3.one * 1.2f; ball.rectTransform.localRotation = Quaternion.identity;
            ball.color = Color.white;
            Pose(keeper, keeperIdle, 114 * keeperSize, false, keeperRest, 1);
            Pose(shooter, shooterIdle, shooterHeight, false, shooterRest, 1);
            crowd.anchoredPosition = crowdRest; stage.anchoredPosition = stageRest;
            allFans?.Sample(0, false);
            net?.Sample(-1, Vector2.zero);
            goalCelebration?.Sample(-1);
            foreach (var image in trail) image.enabled = false;
            foreach (var image in dust) image.enabled = false;
            foreach (var image in confetti) image.enabled = false;
            shadow.rectTransform.anchoredPosition = ballRest + new Vector2(0, -12);
            shadow.rectTransform.localScale = Vector3.one; shadow.enabled = true;
        }

        public void Sample(ShotResult shot, float time)
        {
            Elapsed = time;
            var sign = (int)shot.KeeperDirection - 1;
            var target = new Vector2(((int)shot.PlayerDirection - 1) * spacing, targetY);
            var flight = Mathf.Clamp01((time - ContactTime) / (ImpactTime - ContactTime));
            Phase = time < ContactTime ? ShotAnimationPhase.RunUp : time < ImpactTime ? ShotAnimationPhase.Flight : time < 1.06f ? ShotAnimationPhase.Impact : time < 1.55f ? ShotAnimationPhase.Landing : ShotAnimationPhase.Recovery;

            // Contact frame has its striking boot on the ball, then a brief follow-through.
            var run = Mathf.Clamp01(time / ContactTime);
            var recover = Mathf.Clamp01((time - 1.6f) / .6f);
            var playerFrame = time < .18f ? 0 : time < .32f ? 1 : time < .62f ? 2 : 3;
            var actorScale = shooterHeight / 160;
            var foot = shooterRest + new Vector2(0, -80 * actorScale);
            var contactFoot = ballRest + new Vector2(-69, -92) * actorScale;
            var playerFoot = Vector2.Lerp(Vector2.Lerp(foot, contactFoot, run), foot, recover);
            Pose(shooter, shooterFrames[playerFrame], shooterHeight, false, playerFoot + new Vector2(0, 80 * actorScale), 1);
            var celebrationAge = time - ImpactTime;
            var celebrating = shot.Outcome == ShotOutcome.Goal && celebrationAge >= 0 && celebrationAge < GoalCelebrationDuration;
            goalCelebration?.Sample(celebrating ? celebrationAge : -1);
            if (celebrating)
            {
                // Small victory hops with a side-to-side body lean. Keep body size and kit.
                var envelope = Mathf.Clamp01(celebrationAge / .3f) * Mathf.Clamp01((GoalCelebrationDuration - celebrationAge) / .35f);
                var hop = Mathf.Max(0, Mathf.Sin(celebrationAge * 6)) * 17 * actorScale * envelope;
                var sway = Mathf.Sin(celebrationAge * 3) * 7 * envelope;
                Pose(shooter, shooterIdle, shooterHeight, false, shooterRest + new Vector2(sway, hop), 1);
                shooter.rectTransform.localRotation = Quaternion.Euler(0, 0, -sway * .55f);
            }

            var dive = Mathf.SmoothStep(0, 1, Mathf.Clamp01((time - .16f) / (ImpactTime - .16f)));
            var landing = Mathf.Clamp01((time - 1.02f) / .35f);
            var keeperFrame = time < .38f ? 0 : time < 1.05f ? 1 : time < 1.5f ? 2 : 3;
            var position = keeperRest;
            if (sign == 0)
            {
                position.y += Mathf.Sin(dive * Mathf.PI) * 14 * keeperSize;
                position.y -= landing * 17 * keeperSize * (1 - recover);
                var height = keeperFrame == 2 ? 90 : keeperFrame == 0 ? 106 : 114;
                Pose(keeper, centerFrames[keeperFrame], height * keeperSize, false, position, 1);
            }
            else
            {
                var airborne = new Vector2(sign * (spacing - 65 * keeperSize), targetY - 16 * keeperSize);
                position = Vector2.Lerp(keeperRest, airborne, dive);
                position.y += Mathf.Sin(dive * Mathf.PI) * 20 * keeperSize;
                position.y = Mathf.Lerp(position.y, keeperRest.y - 34 * keeperSize, landing);
                position = Vector2.Lerp(position, keeperRest, recover);
                // Atlas frames share a pixel scale. Normalizing each pose's height made
                // the crouched recovery body inflate compared with the horizontal dive.
                var height = sideFrames[keeperFrame].rect.height * 154f / sideFrames[1].rect.width;
                if (keeperFrame == 3) position.y = keeperRest.y + (height - 114) * keeperSize * .5f;
                Pose(keeper, sideFrames[keeperFrame], height * keeperSize, false, position, sign);
            }
            if (time > 2.02f) Pose(keeper, keeperIdle, 114 * keeperSize, false, keeperRest, 1);

            var ballPosition = Trajectory(target, flight);
            if (time < ContactTime) ballPosition = ballRest;
            if (time >= ImpactTime)
            {
                var settle = Mathf.Clamp01((time - ImpactTime) / .5f);
                if (shot.Outcome == ShotOutcome.Goal)
                    ballPosition = Vector2.Lerp(target, target + new Vector2(0, -48), settle) + new Vector2(0, Mathf.Abs(Mathf.Sin(settle * Mathf.PI * 2)) * 8 * (1 - settle));
                else
                    ballPosition = Vector2.Lerp(target, new Vector2(target.x - sign * 28, keeperRest.y - 57 * keeperSize), settle) + new Vector2(0, Mathf.Sin(settle * Mathf.PI) * 17);
            }
            ball.rectTransform.anchoredPosition = ballPosition;
            if (net != null)
            {
                var hit = (Vector2)net.transform.InverseTransformPoint(stage.TransformPoint(target));
                net.Sample(shot.Outcome == ShotOutcome.Goal ? time - ImpactTime : -1, hit);
            }
            ball.rectTransform.localScale = Vector3.one * 1.2f * Mathf.Lerp(1, .55f, flight);
            ball.rectTransform.localRotation = Quaternion.Euler(0, 0, time < ContactTime ? 0 : (time - ContactTime) * -780);
            shadow.rectTransform.anchoredPosition = new Vector2(ballPosition.x, Mathf.Lerp(ballRest.y - 12, keeperRest.y - 57 * keeperSize, flight));
            shadow.rectTransform.localScale = Vector3.one * Mathf.Lerp(1, .5f, flight);
            for (var i = 0; i < trail.Length; i++)
            {
                var image = trail[i]; image.enabled = time > ContactTime + .04f && time < ImpactTime;
                image.rectTransform.anchoredPosition = Trajectory(target, Mathf.Clamp01(flight - (i + 1) * .035f));
                image.rectTransform.localScale = ball.rectTransform.localScale;
                image.color = new Color(1, 1, 1, .14f / (i + 1));
            }
            var kickAge = time - ContactTime;
            stage.anchoredPosition = stageRest + (kickAge >= 0 && kickAge < .13f ? new Vector2(Mathf.Sin(kickAge * 190) * 2, Mathf.Cos(kickAge * 140)) * (1 - kickAge / .13f) : Vector2.zero);
            for (var i = 0; i < dust.Length; i++)
            {
                dust[i].enabled = kickAge >= 0 && kickAge < .24f;
                dust[i].rectTransform.anchoredPosition = ballRest + new Vector2((i - 2) * kickAge * 65, -12 + kickAge * 25);
                dust[i].color = new Color(.76f, .78f, .47f, Mathf.Clamp01(1 - kickAge / .24f));
            }
            CrowdCelebrating = celebrating;
            crowd.anchoredPosition = crowdRest + new Vector2(0, CrowdCelebrating ? Mathf.Abs(Mathf.Sin((time - ImpactTime) * 7)) * 9 : 0);
            allFans?.Sample(time - ImpactTime, CrowdCelebrating);
            for (var i = 0; i < confetti.Length; i++)
            {
                var particle = confetti[i]; particle.enabled = CrowdCelebrating;
                var age = Mathf.Repeat(time - ImpactTime + i * .037f, .8f);
                particle.rectTransform.anchoredPosition = new Vector2(-435 + i * 46 + Mathf.Sin(age * 12 + i) * 8, crowdRest.y + 15 + 100 * age - 110 * age * age);
                particle.rectTransform.localRotation = Quaternion.Euler(0, 0, age * 260 + i * 35);
            }
        }

        private Vector2 Trajectory(Vector2 target, float t) => Vector2.Lerp(ballRest, target, t) + new Vector2(0, Mathf.Sin(t * Mathf.PI) * 40);

        private static void Pose(Image image, Sprite sprite, float extent, bool width, Vector2 position, int sign)
        {
            image.sprite = sprite;
            var ratio = sprite.rect.width / sprite.rect.height;
            image.rectTransform.sizeDelta = width ? new Vector2(extent, extent / ratio) : new Vector2(extent * ratio, extent);
            image.rectTransform.anchoredPosition = position;
            image.rectTransform.localScale = new Vector3(sign, 1, 1);
            image.rectTransform.localRotation = Quaternion.identity;
        }
    }
}
