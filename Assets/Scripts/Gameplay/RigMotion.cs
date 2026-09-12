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
            var lh=Vector2.Lerp(new Vector2(-31,-9),new Vector2(-48,22),load);
            var rh=Vector2.Lerp(new Vector2(36,-12),new Vector2(47,-1),load);
            var chest=60-2*Mathf.Sin(Window(t,.88f,1.18f)*Mathf.PI);
            var home=Window(time,1.6f,ShotPresentation.Duration-.08f);
            if(celebration>=0)
            {
                var cheer=Window(celebration,0,.35f)*(1-Window(celebration,2.35f,2.92f));
                lh=Vector2.Lerp(lh,new Vector2(-42,111),cheer);
                rh=Vector2.Lerp(rh,new Vector2(44,108),cheer);
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
            var scale=114*keeperSize/210;var sign=ShotTargets.Column(shot.KeeperDirection)-1;
            var high=ShotTargets.IsHigh(shot.KeeperDirection);var start=rest-Vector2.up*9*keeperSize;
            var age=time-ShotPresentation.ContactTime;var after=time-ShotPresentation.ImpactTime;
            var dive=Window(time,ShotPresentation.ContactTime+.13f,ShotPresentation.ImpactTime);
            var anticipation=Mathf.Sin(Window(age,.01f,.20f)*Mathf.PI);
            var angle=sign==0?0f:-sign*(high?60:80);
            var catchGrip=new Vector2(0,sign==0&&!high?-10:100);
            var grip=Vector2.Lerp(new Vector2(0,17),catchGrip,dive);
            var hips=Vector2.Lerp(start,target-FootballRig.Rotate(catchGrip*scale,angle),dive);
            hips+=new Vector2(-sign*6,-4)*anticipation*scale;
            hips.y+=Mathf.Sin(dive*Mathf.PI)*(high?12:3)*scale;angle*=dive;
            var lean=Mathf.Lerp(3,-sign*7,dive);
            var chest=Mathf.Lerp(55,sign==0&&!high?40:59,dive);
            var lf=new Vector2(-34,-96);var rf=new Vector2(35,-95);
            if(sign==0&&!high)
            {lf=Vector2.Lerp(lf,new Vector2(-34,-44),dive);rf=Vector2.Lerp(rf,new Vector2(34,-43),dive);}
            else if(sign!=0)
            {
                lf=Vector2.Lerp(lf,new Vector2(-27,sign<0?-60:-91),dive);
                rf=Vector2.Lerp(rf,new Vector2(30,sign>0?-60:-91),dive);
            }
            var spread=Mathf.Lerp(42,13,dive);
            var balance=Mathf.Sin(dive*Mathf.PI)*12;
            if(after>=0)
            {
                var land=Window(after,.05f,.48f);
                var settle=Mathf.Sin(Window(after,.38f,.68f)*Mathf.PI)*2;
                if(sign!=0)
                {
                    angle=Mathf.Lerp(angle,-sign*86,land);
                    hips.y=Mathf.Lerp(hips.y,rest.y-57*keeperSize+19*scale,land)+settle*scale;
                }
                else if(high)hips=Vector2.Lerp(hips,start-Vector2.up*5*scale,land);
                spread=Mathf.Lerp(13,12,Window(after,0,.12f));
                // Land, absorb impact, roll onto a knee and stand while still holding the ball.
                var recover=Window(after,.62f,1.22f);
                hips=Vector2.Lerp(hips,start,recover);angle=Mathf.Lerp(angle,0,recover);
                lean=Mathf.Lerp(lean,3,recover);chest=Mathf.Lerp(chest,55,recover);
                lf=Vector2.Lerp(lf,new Vector2(-34,-96),recover);
                rf=Vector2.Lerp(rf,new Vector2(35,-95),recover);
                grip=Vector2.Lerp(grip,new Vector2(0,shot.Outcome==ShotOutcome.Save?23:17),recover);
                if(shot.Outcome==ShotOutcome.Goal)spread=Mathf.Lerp(spread,42,recover);
            }
            rig.MotionBlend=Window(age,.01f,.20f)*(1-Window(after,.62f,1.22f));
            rig.KeepHandsClosed=shot.Outcome==ShotOutcome.Save&&after>=0;
            rig.SetRoot(hips,scale,angle);
            rig.Pose(lean,lf,rf,grip+new Vector2(-spread,balance),grip+new Vector2(spread,-balance),chest);
        }
    }
}

