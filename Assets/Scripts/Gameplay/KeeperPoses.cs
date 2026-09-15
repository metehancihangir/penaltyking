using UnityEngine;

namespace PenaltyKing
{
    // Authored independently in rig space. The root stays unrotated: the spine,
    // shoulders, limbs and ankles establish each silhouette.
    public struct KeeperPose
    {
        public float lower, upper, chest, hip, height, leftAnkle, rightAnkle;
        public Vector2 leftFoot, rightFoot, leftHand, rightHand;
        public float leftReach,rightReach;
        public void Apply(FootballRig rig) => rig.Pose(chest,leftFoot,rightFoot,leftHand,rightHand,height,hip,leftAnkle,rightAnkle,lower,upper,leftReach,rightReach);
        public static KeeperPose Blend(KeeperPose a,KeeperPose b,float t)=>new KeeperPose
        {
            leftReach=Mathf.Lerp(a.leftReach,b.leftReach,t),rightReach=Mathf.Lerp(a.rightReach,b.rightReach,t),lower=Mathf.Lerp(a.lower,b.lower,t),upper=Mathf.Lerp(a.upper,b.upper,t),chest=Mathf.Lerp(a.chest,b.chest,t),hip=Mathf.Lerp(a.hip,b.hip,t),height=Mathf.Lerp(a.height,b.height,t),
            leftAnkle=Mathf.Lerp(a.leftAnkle,b.leftAnkle,t),rightAnkle=Mathf.Lerp(a.rightAnkle,b.rightAnkle,t),
            leftFoot=Vector2.Lerp(a.leftFoot,b.leftFoot,t),rightFoot=Vector2.Lerp(a.rightFoot,b.rightFoot,t),leftHand=Vector2.Lerp(a.leftHand,b.leftHand,t),rightHand=Vector2.Lerp(a.rightHand,b.rightHand,t)
        };
    }

    public static class KeeperPoses
    {
        public static KeeperPose Ready=>new KeeperPose{lower=2,upper=1,chest=3,height=55,leftFoot=new Vector2(-34,-96),rightFoot=new Vector2(35,-95),leftHand=new Vector2(-42,17),rightHand=new Vector2(42,17)};
        public static KeeperPose Contact(ShotDirection direction)
        {
            switch(direction)
            {
                case ShotDirection.LeftHigh:
                    return new KeeperPose{rightReach=1,lower=32,upper=44,chest=47,hip=38,height=60,leftAnkle=22,rightAnkle=26,
                        leftFoot=new Vector2(57,-72),rightFoot=new Vector2(52,-83),leftHand=new Vector2(-83,79),rightHand=new Vector2(-76,93)};
                case ShotDirection.RightHigh:
                    return new KeeperPose{leftReach=1,lower=-30,upper=-44,chest=-48,hip=-40,height=60,leftAnkle=-26,rightAnkle=-20,
                        leftFoot=new Vector2(-50,-81),rightFoot=new Vector2(-59,-68),leftHand=new Vector2(79,90),rightHand=new Vector2(82,78)};
                case ShotDirection.Left:
                    return new KeeperPose{rightReach=1,lower=61,upper=74,chest=78,hip=62,height=60,leftAnkle=27,rightAnkle=24,
                        leftFoot=new Vector2(72,-52),rightFoot=new Vector2(88,-38),leftHand=new Vector2(-115,30),rightHand=new Vector2(-110,40)};
                case ShotDirection.Right:
                    return new KeeperPose{leftReach=1,lower=-55,upper=-70,chest=-77,hip=-60,height=60,leftAnkle=-25,rightAnkle=-28,
                        leftFoot=new Vector2(-86,-42),rightFoot=new Vector2(-78,-47),leftHand=new Vector2(109,45),rightHand=new Vector2(112,35)};
                case ShotDirection.CenterHigh:
                    return new KeeperPose{leftReach=1,rightReach=1,lower=2,upper=-3,chest=-3,height=60,leftAnkle=-6,rightAnkle=8,
                        leftFoot=new Vector2(-23,-98),rightFoot=new Vector2(20,-96),leftHand=new Vector2(-14,121),rightHand=new Vector2(13,123)};
                default:
                    return new KeeperPose{lower=7,upper=-2,chest=5,height=45,hip=-3,leftAnkle=5,rightAnkle=-8,
                        leftFoot=new Vector2(-52,-71),rightFoot=new Vector2(46,-68),leftHand=new Vector2(-13,9),rightHand=new Vector2(13,9)};
            }
        }
        public static KeeperPose Landing(ShotDirection direction)
        {
            if(direction==ShotDirection.Center||direction==ShotDirection.CenterHigh)
            {
                var center=Contact(ShotDirection.Center);center.leftHand=new Vector2(-12,22);center.rightHand=new Vector2(12,22);return center;
            }
            var left=ShotTargets.Column(direction)==0;
            return left?new KeeperPose{lower=69,upper=79,chest=83,hip=70,height=60,leftAnkle=25,rightAnkle=22,
                leftFoot=new Vector2(71,-50),rightFoot=new Vector2(91,-27),leftHand=new Vector2(-112,19),rightHand=new Vector2(-110,31)}
                :new KeeperPose{lower=-67,upper=-80,chest=-84,hip=-68,height=60,leftAnkle=-22,rightAnkle=-26,
                leftFoot=new Vector2(-91,-25),rightFoot=new Vector2(-73,-48),leftHand=new Vector2(109,30),rightHand=new Vector2(114,18)};
        }
    }
}
