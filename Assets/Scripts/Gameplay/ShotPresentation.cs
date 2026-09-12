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
        public const float WindupDelay = .46f;
        public const float ContactTime = .24f + WindupDelay;
        public const float ImpactTime = .9f + WindupDelay;
        public const float Duration = 2.2f + WindupDelay;
        public const float GoalCelebrationDuration = 3f;
        public static float DurationFor(ShotResult shot) => shot.Outcome == ShotOutcome.Goal ? ImpactTime + GoalCelebrationDuration : Duration;
        private GoalCelebration goalCelebration;
        [SerializeField] private FootballRig strikerRig, goalieRig;
        public bool UsesBoneRigs => strikerRig!=null && goalieRig!=null;
        public Vector2 GripPosition => goalieRig!=null?goalieRig.GripPosition:KeeperPosition;
        public void ConfigureRigs(FootballRig player,FootballRig goalkeeper){strikerRig=player;goalieRig=goalkeeper;}
        private void SampleRigKeeper(ShotResult shot,float time)
        { RigMotion.Keeper(goalieRig,shot,time,keeperRest,TargetPoint(shot.KeeperDirection),keeperSize); }
        public bool GoalCelebrating => goalCelebration != null && goalCelebration.Visible;
        [SerializeField] private Image ball, keeper, shooter, shadow;
        [SerializeField] private RectTransform stage, crowd;
        [SerializeField] private Image[] trail, dust, confetti;
        [SerializeField] private Sprite keeperIdle, shooterIdle;
        [SerializeField] private Sprite[] shooterFrames, sideFrames, centerFrames;
        private Vector2 ballRest, keeperRest, shooterRest, crowdRest, stageRest;
        private CrowdCelebration allFans;
        private GoalNetRipple net;
        private int originalBallOrder = -1, originalEffectsOrder;
        private int ballDepthLayer;
        [SerializeField] private Sprite[] performanceFrames;
        [SerializeField] private Vector2 contactFootPixels;
        [SerializeField] private Sprite[] shotFrames, keeperSix;
        [SerializeField] private Vector2 shotContactPixels;
        public void ConfigureSixAnimation(Sprite[] shots,Vector2 toe,Sprite[] saves)
        { shotFrames=shots;shotContactPixels=toe;keeperSix=saves;shooterIdle=shots[0];keeperIdle=saves[7]; }
        public Vector2 TargetPoint(ShotDirection direction)
        {
            if(net==null)return new Vector2((ShotTargets.Column(direction)-1)*spacing,targetY+(ShotTargets.IsHigh(direction)?65:0));
            var rect=((RectTransform)net.transform).rect;
            var point=new Vector2((ShotTargets.Column(direction)-1)*rect.width*.32f,rect.height*(ShotTargets.IsHigh(direction)?.40f:-.36f));
            return stage.InverseTransformPoint(net.transform.TransformPoint(point));
        }
        public void ConfigurePerformance(Sprite[] frames, Vector2 contact)
        { performanceFrames = frames; contactFootPixels = contact; shooterIdle = frames[0]; }
        private float spacing = 190, targetY = 58;
        [SerializeField] private float shooterHeight = 160, keeperSize = 1;
        public float ShooterHeight => shooterHeight;
        public Vector2 ShooterPosition => shooter.rectTransform.anchoredPosition;
        public Vector2 ContactBootPosition => strikerRig!=null ? strikerRig.ToePosition : ShooterPosition + ((shotFrames!=null&&shotFrames.Length==12?shotContactPixels:contactFootPixels) + shooter.sprite.pivot - shooter.sprite.rect.size * .5f) * (shooterHeight / (shotFrames!=null&&shotFrames.Length==12?shotFrames[0]:performanceFrames[0]).rect.height);
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
                    var overlay = new GameObject("GOAL Celebration", typeof(RectTransform), typeof(Canvas), typeof(GoalCelebration));
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
            SortBallDepth(0);
            Phase = ShotAnimationPhase.Idle; Elapsed = 0; CrowdCelebrating = false;
            ball.rectTransform.anchoredPosition = ballRest;
            ball.rectTransform.localScale = Vector3.one * 1.2f; ball.rectTransform.localRotation = Quaternion.identity;
            ball.color = Color.white;
            Pose(keeper, keeperIdle, 114 * keeperSize, false, keeperRest, 1);
            Pose(shooter, shooterIdle, shooterHeight, false, shooterRest, 1);
            SamplePlayer(0, -1);
            if(goalieRig!=null){SampleRigKeeper(new ShotResult(ShotDirection.Center,ShotDirection.Center),0);goalieRig.HoldBall(false,ball.sprite,Vector2.zero);}
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
            var keeperTime = Mathf.Max(0, time - WindupDelay);
            var sign = ShotTargets.Column(shot.KeeperDirection) - 1;
            var target = TargetPoint(shot.PlayerDirection);
            var flight = Mathf.Clamp01((time - ContactTime) / (ImpactTime - ContactTime));
            Phase = time < ContactTime ? ShotAnimationPhase.RunUp : time < ImpactTime ? ShotAnimationPhase.Flight : keeperTime < 1.06f ? ShotAnimationPhase.Impact : keeperTime < 1.55f ? ShotAnimationPhase.Landing : ShotAnimationPhase.Recovery;

            // Contact frame has its striking boot on the ball, then a brief follow-through.
            var run = Mathf.Clamp01(time / ContactTime);
            var recover = Mathf.Clamp01((keeperTime - 1.6f) / .6f);
            var playerFrame = time < .18f ? 0 : time < .32f ? 1 : time < .62f ? 2 : 3;
            var actorScale = shooterHeight / 160;
            var foot = shooterRest + new Vector2(0, -80 * actorScale);
            var contactFoot = ballRest + new Vector2(-69, -92) * actorScale;
            var playerFoot = Vector2.Lerp(Vector2.Lerp(foot, contactFoot, run), foot, recover);
            if (performanceFrames == null || performanceFrames.Length < 8)
                Pose(shooter, shooterFrames[playerFrame], shooterHeight, false, playerFoot + new Vector2(0, 80 * actorScale), 1);
            var celebrationAge = time - ImpactTime;
            var celebrating = shot.Outcome == ShotOutcome.Goal && celebrationAge >= 0 && celebrationAge < GoalCelebrationDuration;
            goalCelebration?.Sample(celebrating ? celebrationAge : -1);
            SamplePlayer(time, celebrating ? celebrationAge : -1);

            if(goalieRig==null)
            {
            var dive = Mathf.SmoothStep(0, 1, Mathf.Clamp01((keeperTime - .16f) / (.9f - .16f)));
            var landing = Mathf.Clamp01((keeperTime - 1.02f) / .35f);
            var keeperFrame = keeperTime < .38f ? 0 : keeperTime < 1.05f ? 1 : keeperTime < 1.5f ? 2 : 3;
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
            if (keeperTime > 2.02f) Pose(keeper, keeperIdle, 114 * keeperSize, false, keeperRest, 1);

            }
            if(goalieRig!=null)SampleRigKeeper(shot,time);
            else if(keeperSix!=null&&keeperSix.Length==8)SampleSixKeeper(shot.KeeperDirection,time);
            var ballPosition = Trajectory(target, flight);
            if (time < ContactTime) ballPosition = ballRest;
            if (time >= ImpactTime)
            {
                var settle = Mathf.Clamp01((time - ImpactTime) / .5f);
                if (shot.Outcome == ShotOutcome.Goal)
                    ballPosition = Vector2.Lerp(target, NetFloor(target), settle) + new Vector2(0, Mathf.Sin(settle * Mathf.PI) * 5 * (1 - settle));
                else
                    ballPosition = Vector2.Lerp(target, new Vector2(target.x - sign * 28, keeperRest.y - 57 * keeperSize), settle) + new Vector2(0, Mathf.Sin(settle * Mathf.PI) * 17);
            }
            var held=goalieRig!=null && shot.Outcome==ShotOutcome.Save && time>=ImpactTime;
            if(held)ballPosition=goalieRig.GripPosition;
            ball.color=held?new Color(1,1,1,0):Color.white;
            ball.rectTransform.anchoredPosition = ballPosition;
            SortBallDepth(time >= ImpactTime ? 2 : flight > .12f ? 1 : 0);
            if (net != null)
            {
                var hit = (Vector2)net.transform.InverseTransformPoint(stage.TransformPoint(target));
                net.Sample(shot.Outcome == ShotOutcome.Goal ? time - ImpactTime : -1, hit);
            }
            ball.rectTransform.localScale = Vector3.one * 1.2f * Mathf.Lerp(1, .55f, flight);
            var spinTime = Mathf.Clamp(time - ContactTime, 0, ImpactTime - ContactTime);
            var roll = Mathf.Clamp01((time - ImpactTime) / .5f);
            var spin = spinTime * -780 - (time >= ImpactTime ? 100 * (2 * roll - roll * roll) : 0);
            ball.rectTransform.localRotation = held?Quaternion.identity:Quaternion.Euler(0, 0, spin);
            goalieRig?.HoldBall(held,ball.sprite,ball.rectTransform.rect.size*1.2f*.55f);
            shadow.enabled = !held;
            shadow.rectTransform.anchoredPosition = new Vector2(ballPosition.x, Mathf.Lerp(ballRest.y - 12, keeperRest.y - 57 * keeperSize, flight));
            shadow.rectTransform.localScale = Vector3.one * Mathf.Lerp(1, .5f, flight);
            if (shot.Outcome == ShotOutcome.Goal && time >= ImpactTime)
            {
                var floor = NetFloor(target);
                shadow.rectTransform.anchoredPosition = floor - Vector2.up * ball.rectTransform.rect.height * .5f * 1.2f * .55f;
                shadow.rectTransform.localScale = Vector3.one * .65f;
            }
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
                dust[i].enabled = false; // Avoid detached debris at the striking foot.
                dust[i].rectTransform.anchoredPosition = ballRest + new Vector2((i - 2) * kickAge * 65, -12 + kickAge * 25);
                dust[i].color = new Color(.76f, .78f, .47f, Mathf.Clamp01(1 - kickAge / .24f));
            }
            CrowdCelebrating = celebrating;
            crowd.anchoredPosition = crowdRest;
            allFans?.Sample(time - ImpactTime, CrowdCelebrating);
        }

        private Vector2 Trajectory(Vector2 target, float t) => Vector2.Lerp(ballRest, target, t) + new Vector2(0, Mathf.Sin(t * Mathf.PI) * 40);

        private Vector2 NetFloor(Vector2 target)
        {
            if (net == null) return target + new Vector2(0, -25);
            var rect = ((RectTransform)net.transform).rect;
            var local = (Vector2)net.transform.InverseTransformPoint(stage.TransformPoint(target));
            // Rear ground seam in the trimmed Goal sprite is at 9% of its height.
            // Add the actual ball radius so its bottom, not its centre, rests on it.
            var radius = ball.rectTransform.rect.height * .5f * 1.2f * .55f;
            var localRadius = net.transform.InverseTransformVector(stage.TransformVector(Vector3.up * radius)).y;
            local.x *= .94f; local.y = rect.yMin + rect.height * .09f + localRadius;
            // A centre goal rolls a little to the side of the keeper after hitting the net.
            if (Mathf.Abs(local.x) < 1) local.x = rect.width * .11f;
            return stage.InverseTransformPoint(net.transform.TransformPoint(local));
        }

        private void SamplePlayer(float time, float celebrationAge)
        {
            if(strikerRig!=null){RigMotion.Striker(strikerRig,time,celebrationAge,shooterRest,ballRest,shooterHeight);return;}
            if (performanceFrames == null || performanceFrames.Length < 8) return;
            if(shotFrames!=null&&shotFrames.Length==12 && celebrationAge<0)
            { SampleShotPlayer(time);return; }
            var scale = shooterHeight / 160;
            var frame = time < .2f ? 0 : time < ContactTime ? 1 : time < ContactTime + .16f ? 2 : 3;
            if (time > ContactTime + .4f) frame = 0;
            if (performanceFrames.Length == 12)
            {
                frame = time < .10f ? 0 : time < .23f ? 1 : time < .36f ? 2 : time < .49f ? 3 : time < ContactTime ? 4 : time < ContactTime + .10f ? 5 : time < ContactTime + .22f ? 6 : time < ContactTime + .4f ? 7 : 0;
            }
            var approach = Mathf.SmoothStep(0, 1, Mathf.Clamp01((time - .25f) / (ContactTime - .25f)));
            var foot = shooterRest + new Vector2(0, -shooterHeight * .5f);
            var plantedFoot = ballRest - (shotFrames!=null&&shotFrames.Length==12 ? shotContactPixels*(shooterHeight/shotFrames[0].rect.height) : contactFootPixels * (shooterHeight / performanceFrames[0].rect.height));
            foot = Vector2.Lerp(foot + new Vector2(-8, -3) * Mathf.Sin(Mathf.Clamp01(time / ContactTime) * Mathf.PI), plantedFoot, approach);
            if (celebrationAge >= 0)
            {
                frame = celebrationAge < .2f ? 4 : celebrationAge < .65f ? 5 : celebrationAge < 1.0f ? 6 : celebrationAge < 1.45f ? 5 : celebrationAge < 1.8f ? 7 : 5;
                if (performanceFrames.Length == 12)
                    frame = celebrationAge < .2f ? 8 : celebrationAge < .7f ? 9 : celebrationAge < 1.05f ? 10 : celebrationAge < 1.6f ? 9 : celebrationAge < 2.1f ? 10 : celebrationAge < 2.65f ? 9 : 11;
            }
            // Sprite pivots mark the planted foot; one pixels-to-world scale for all poses.
            var sprite = performanceFrames[frame];
            var pixelScale = shooterHeight / performanceFrames[0].rect.height;
            var size = sprite.rect.size * pixelScale;
            var pivotOffset = (sprite.rect.size * .5f - sprite.pivot) * pixelScale;
            Pose(shooter, sprite, size.y, false, foot + pivotOffset, 1);
        }

        private void SampleShotPlayer(float time)
        {
            // Load, approach, plant, backswing, contact and follow-through have distinct drawings.
            var frame=time<ContactTime ? Mathf.Clamp(Mathf.FloorToInt(time/ContactTime*8),0,7)
                : time<ContactTime+.09f?8:time<ContactTime+.20f?9:time<ContactTime+.33f?10:11;
            if(time<=0)frame=0;
            var pixelScale=shooterHeight/shotFrames[0].rect.height;
            var foot=shooterRest-Vector2.up*shooterHeight*.5f;
            var planted=ballRest-shotContactPixels*pixelScale;
            var approach=Mathf.SmoothStep(0,1,Mathf.Clamp01((time-.16f)/(ContactTime-.16f)));
            foot=Vector2.Lerp(foot,planted,approach);
            var sprite=shotFrames[frame];
            Pose(shooter,sprite,sprite.rect.height*pixelScale,false,foot+(sprite.rect.size*.5f-sprite.pivot)*pixelScale,1);
        }

        private void SampleSixKeeper(ShotDirection direction,float time)
        {
            var column=ShotTargets.Column(direction)-1;var high=ShotTargets.IsHigh(direction);
            var scale=114*keeperSize/keeperSix[7].rect.height;
            var age=time-ContactTime;var flight=ImpactTime-ContactTime;
            if(age<.08f || time>Duration-.1f){Pose(keeper,keeperSix[7],keeperSix[7].rect.height*scale,false,keeperRest,1);return;}
            var dive=Mathf.SmoothStep(0,1,Mathf.Clamp01((age-.08f)/(flight-.08f)));
            var target=TargetPoint(direction);var sign=column==0?1:column;
            var frame=column==0?(high?2:3):(high?0:1);
            var sprite=keeperSix[frame];
            // Save-pose pivot is the glove contact point. Absolute pixel scale stays fixed.
            var offset=(sprite.rect.size*.5f-sprite.pivot)*scale;offset.x*=sign;
            var position=Vector2.Lerp(keeperRest,target+offset,dive);
            position.y+=Mathf.Sin(dive*Mathf.PI)*(high?13:4);
            var after=time-ImpactTime;
            if(after>.16f)
            {
                var groundY=keeperRest.y-57*keeperSize;
                var landing=Mathf.SmoothStep(0,1,Mathf.Clamp01((after-.16f)/.22f));
                var landingFeet=position-Vector2.up*(sprite.rect.height*scale*.5f);
                frame=column==0?6:after<.5f?4:5;sprite=keeperSix[frame];
                var feet=new Vector2(target.x-column*42*keeperSize,groundY);
                var footOffset=(sprite.rect.size*.5f-sprite.pivot)*scale;footOffset.x*=sign;
                position=Vector2.Lerp(landingFeet,feet,landing)+footOffset;
                var recover=Mathf.SmoothStep(0,1,Mathf.Clamp01((after-.65f)/.5f));
                if(recover>0){sprite=keeperSix[7];sign=1;position=Vector2.Lerp(feet+Vector2.up*57*keeperSize,keeperRest,recover);}
            }
            Pose(keeper,sprite,sprite.rect.height*scale,false,position,sign);
        }

        private static void Pose(Image image, Sprite sprite, float extent, bool width, Vector2 position, int sign)
        {
            image.sprite = sprite;
            var ratio = sprite.rect.width / sprite.rect.height;
            image.rectTransform.sizeDelta = width ? new Vector2(extent, extent / ratio) : new Vector2(extent * ratio, extent);
            image.rectTransform.anchoredPosition = position;
            image.rectTransform.localScale = new Vector3(sign, 1, 1);
            image.rectTransform.localRotation = Quaternion.identity;
        }

        private void SortBallDepth(int layer)
        {
            var effects = shadow.transform.parent;
            if (originalBallOrder < 0) { originalBallOrder = ball.transform.GetSiblingIndex(); originalEffectsOrder = effects.GetSiblingIndex(); }
            if (ballDepthLayer == layer) return;
            ballDepthLayer = layer;
            if (layer > 0)
            {
                var foreground = layer == 2 ? keeper.transform : shooter.transform;
                effects.SetSiblingIndex(foreground.GetSiblingIndex());
                ball.transform.SetSiblingIndex(foreground.GetSiblingIndex());
            }
            else
            {
                ball.transform.SetSiblingIndex(originalBallOrder);
                effects.SetSiblingIndex(originalEffectsOrder);
            }
        }
    }
}
