using UnityEngine;

namespace PenaltyKing
{
    // Continuous curves; no animation frame quantisation. One pause-aware game clock.
    public static class RigMotion
    {
        private static float Ease(float t) => Mathf.SmoothStep(0, 1, Mathf.Clamp01(t));
        private static float Window(float t, float a, float b) => Ease((t-a)/(b-a));

        public static void Striker(FootballRig rig,float time,float celebration,Vector2 rest,Vector2 ball,float height)
        {
            var scale=height/210; var t=time/ShotPresentation.ContactTime;
            var start=rest+new Vector2(0,-9)*scale;
            var hips=Vector2.Lerp(start,ball+new Vector2(-101,73)*scale,Window(t,.12f,1));
            var load=Window(t,.22f,.70f)*(1-Window(t,.70f,1.16f));
            // Pelvis leads the knee, followed by ankle extension at contact.
            var hip=-2-7*Window(t,.22f,.60f)+17*Window(t,.64f,.94f);
            var lean=2-10*Window(t,.20f,.65f)+24*Window(t,.78f,1.25f);
            var ankle=-20*Window(t,.34f,.72f)*(1-Window(t,.86f,1));
            ankle+=17*Window(t,1,1.30f)*(1-Window(t,1.30f,1.85f));
            var lf=new Vector2(-19,-100); var rf=new Vector2(24,-98);
            if(t<.72f)rf=Vector2.Lerp(rf,new Vector2(-32,-52),Window(t,.25f,.72f));
            else if(t<=1)rf=Vector2.Lerp(new Vector2(-32,-52),new Vector2(82,-73),Window(t,.72f,1));
            else if(t<1.3f)rf=Vector2.Lerp(new Vector2(82,-73),new Vector2(78,-35),Window(t,1,1.3f));
            else rf=Vector2.Lerp(new Vector2(78,-35),rf,Window(t,1.3f,1.85f));
            lf.y+=Mathf.Sin(Window(t,.12f,.62f)*Mathf.PI)*9;
            hips.y+=Mathf.Sin(Window(t,1.04f,1.58f)*Mathf.PI)*3*scale;
            var counter=Window(t,.32f,.85f)*(1-Window(t,1.15f,1.8f));
            var lh=Vector2.Lerp(new Vector2(-31,-9),new Vector2(-62,25),counter);
            var rh=Vector2.Lerp(new Vector2(36,-12),new Vector2(42,-17),counter);
            var chest=60-2*Mathf.Sin(Window(t,.88f,1.18f)*Mathf.PI);
            var home=Window(time,1.6f,ShotPresentation.Duration-.08f);
            if(celebration>=0)
            {
                var cheer=Window(celebration,0,.28f)*(1-Window(celebration,2.35f,2.92f));
                var secondArm=Window(celebration,.14f,.47f)*(1-Window(celebration,2.45f,2.92f));
                lh=Vector2.Lerp(lh,new Vector2(-42,111),cheer);
                rh=Vector2.Lerp(rh,new Vector2(44,104),secondArm);
                lean=Mathf.Lerp(lean,Mathf.Sin(celebration*3)*3,cheer);
                hips.y+=Mathf.Max(0,Mathf.Sin(celebration*4))*3*scale*cheer;
                home=Window(celebration,2.25f,2.92f);
            }
            hips=Vector2.Lerp(hips,start,home);lean=Mathf.Lerp(lean,2,home);
            hip=Mathf.Lerp(hip,-2,Window(t,1.15f,1.9f));chest=Mathf.Lerp(chest,60,home);
            lf=Vector2.Lerp(lf,new Vector2(-19,-100),home);rf=Vector2.Lerp(rf,new Vector2(24,-98),home);
            lh=Vector2.Lerp(lh,new Vector2(-31,-9),home);rh=Vector2.Lerp(rh,new Vector2(36,-12),home);
            rig.MotionBlend=Window(t,.05f,.38f)*(1-home);rig.KeepHandsClosed=false;
            rig.SetRoot(hips,scale,0);rig.Pose(lean,lf,rf,lh,rh,chest,hip,0,ankle);
        }

        public static void Keeper(FootballRig rig,ShotResult shot,float time,Vector2 rest,Vector2 target,float keeperSize)
        {
            var scale=114*keeperSize/210;var start=rest-Vector2.up*9*keeperSize;
            var after=time-ShotPresentation.ImpactTime;var age=time-ShotPresentation.ContactTime;
            var dive=Window(time,ShotPresentation.ContactTime+.12f,ShotPresentation.ImpactTime);
            var pose=KeeperPoses.Contact(shot.KeeperDirection);
            rig.MotionBlend=1;rig.KeepHandsClosed=false;rig.SetRoot(Vector2.zero,scale,0);pose.Apply(rig);
            var catchRoot=target-rig.GripPosition;
            var hips=Vector2.Lerp(start,catchRoot,dive);
            var sign=ShotTargets.Column(shot.KeeperDirection)-1;
            var anticipate=Mathf.Sin(Window(age,.01f,.18f)*Mathf.PI);
            hips+=new Vector2(-sign*5,-3)*anticipate*scale;
            hips.y+=Mathf.Sin(dive*Mathf.PI)*(ShotTargets.IsHigh(shot.KeeperDirection)?13:3)*scale;
            var active=KeeperPose.Blend(KeeperPoses.Ready,pose,dive);
            var recover=Window(after,.67f,1.23f);
            if(after>=0)
            {
                var landing=Window(after,.08f,.54f);
                active=KeeperPose.Blend(active,KeeperPoses.Landing(shot.KeeperDirection),landing);
                var ground=rest.y-57*keeperSize+20*scale;
                hips.y=Mathf.Lerp(hips.y,sign==0?start.y-14*scale:ground,landing);
                hips.y+=Mathf.Sin(Window(after,.44f,.67f)*Mathf.PI)*1.5f*scale;
                var finish=KeeperPoses.Ready;
                if(shot.Outcome==ShotOutcome.Save){finish.leftHand=new Vector2(-12,23);finish.rightHand=new Vector2(12,23);}
                active=KeeperPose.Blend(active,finish,recover);
                hips=Vector2.Lerp(hips,start,recover);
            }
            rig.MotionBlend=Window(age,.01f,.20f)*(1-recover);
            rig.KeepHandsClosed=shot.Outcome==ShotOutcome.Save&&after>=0;
            rig.SetRoot(hips,scale,0);active.Apply(rig);
        }
    }
}
