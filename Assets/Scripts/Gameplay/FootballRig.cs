using UnityEngine;
using UnityEngine.UI;
namespace PenaltyKing
{
    // Art is attached to joint transforms. A two-bone IK solve drives continuous limbs;
    // no pose sprites are swapped during the animation.
    public sealed class FootballRig : MonoBehaviour
    {
        [SerializeField] private Image[] art;
        [SerializeField] private RectTransform[] bones;
        [SerializeField] private bool goalkeeper;
        [SerializeField] private Image heldBall;
        [SerializeField] private Image waist;
        [SerializeField] private FootballSkin skin, frontHands;
        private readonly Vector2[] posePoints=new Vector2[17];
        private readonly float[] poseAngles=new float[17];
        public float MotionBlend {get;set;}=1;
        public bool KeepHandsClosed {get;set;}
        public void ConfigureSkin(FootballSkin body,FootballSkin hands){skin=body;frontHands=hands;}
        public const float UpperLeg = 48, LowerLeg = 48;
        public const float UpperArm = 35, LowerArm = 34;
        public Vector2 GripPosition => (HandPosition(false)+HandPosition(true))*.5f;
        public Vector2 ToePosition => ParentPoint(bones[14].TransformPoint(new Vector2(19,0)));
        public bool Goalkeeper => goalkeeper;
        public void Configure(Image[] images,RectTransform[] joints,bool goalie){art=images;bones=joints;goalkeeper=goalie;}
        public void SetMaterial(Material material){foreach(var image in art)image.material=material;if(waist!=null)waist.material=material;if(skin!=null)skin.material=material;if(frontHands!=null)frontHands.material=material;}
        public void ConfigureWaist(Image image)=>waist=image;
        public void ConfigureHeldBall(Image image)=>heldBall=image;
        public void HoldBall(bool hold,Sprite sprite,Vector2 size)
        {
            if(heldBall==null)return;heldBall.gameObject.SetActive(hold);if(!hold)return;
            heldBall.sprite=sprite;heldBall.rectTransform.sizeDelta=size/transform.localScale.x;
            heldBall.rectTransform.position=transform.parent.TransformPoint(GripPosition);
            heldBall.rectTransform.rotation=transform.parent.rotation;
        }
        private Vector2 ParentPoint(Vector3 world)=>transform.parent.InverseTransformPoint(world);
        private Vector2 HandPosition(bool right)=>ParentPoint(bones[right?8:5].position);
        public void SetRoot(Vector2 hips,float scale,float angle)
        {
            var rt=(RectTransform)transform;rt.anchoredPosition=hips;rt.localScale=Vector3.one*scale;rt.localRotation=Quaternion.Euler(0,0,angle);
        }
        public void Pose(float lean,Vector2 leftFoot,Vector2 rightFoot,Vector2 leftHand,Vector2 rightHand,float chestHeight=60,
            float hipAngle=0,float leftAnkle=0,float rightAnkle=0,float lowerSpine=float.NaN,float upperSpine=float.NaN,float leftReach=0,float rightReach=0)
        {
            var low=float.IsNaN(lowerSpine)?lean*.45f:lowerSpine;
            var high=float.IsNaN(upperSpine)?lean*.8f:upperSpine;
            var lowPoint=Rotate(new Vector2(0,chestHeight/3),low);
            var highPoint=lowPoint+Rotate(new Vector2(0,chestHeight/3),high);
            var chest=highPoint+Rotate(new Vector2(0,chestHeight/3),lean);
            Joint(0,Vector2.zero,hipAngle);
            if(bones.Length>15){Joint(15,lowPoint,low);Joint(16,highPoint,high);}
            Joint(1,chest,lean);Joint(2,chest+Rotate(new Vector2(0,24),lean*.9f),lean*.9f);
            Place(art[0],bones[0],new Vector2(0,-2),new Vector2(54,33));
            // The shirt extends below the waist; the middle strip also covers rotation gaps.
            if(waist!=null)Place(waist,bones[0],new Vector2(0,10),new Vector2(43,29));
            Place(art[1],bones[1],new Vector2(0,-chestHeight*.47f),new Vector2(goalkeeper?68:60,chestHeight+21));
            Place(art[2],bones[2],new Vector2(0,0),goalkeeper?new Vector2(36,40):new Vector2(31,36));
            var leftShoulder=chest+Rotate(new Vector2(-26,-5),lean);
            var rightShoulder=chest+Rotate(new Vector2(26,-5),lean);
            leftHand=Vector2.Lerp(leftHand,leftShoulder+(leftHand-leftShoulder).normalized*(UpperArm+LowerArm-.002f),leftReach);
            rightHand=Vector2.Lerp(rightHand,rightShoulder+(rightHand-rightShoulder).normalized*(UpperArm+LowerArm-.002f),rightReach);
            if(leftReach>0&&rightReach==0)rightHand=Vector2.Lerp(rightHand,leftHand+Rotate(new Vector2(8,-8),lean),leftReach);
            if(rightReach>0&&leftReach==0)leftHand=Vector2.Lerp(leftHand,rightHand+Rotate(new Vector2(-8,-8),lean),rightReach);
            Arm(false,leftShoulder,leftHand);
            Arm(true,rightShoulder,rightHand);
            Leg(false,Rotate(new Vector2(-17.5f,-5),hipAngle),leftFoot,leftAnkle);
            Leg(true,Rotate(new Vector2(17.5f,-5),hipAngle),rightFoot,rightAnkle);
            if(skin!=null)
            {
                // Blend the complete skeleton from the anatomically drawn bind pose.
                for(var i=0;i<bones.Length;i++)
                {
                    posePoints[i]=transform.InverseTransformPoint(bones[i].position);
                    poseAngles[i]=(Quaternion.Inverse(transform.rotation)*bones[i].rotation).eulerAngles.z;
                }
                for(var i=0;i<bones.Length;i++)
                {
                    var mix=KeepHandsClosed&&i>=3&&i<=8?1:MotionBlend;
                    Joint(i,Vector2.Lerp(skin.BindPoints[i],posePoints[i],mix),Mathf.LerpAngle(skin.BindAngles[i],poseAngles[i],mix));
                }
                skin.RefreshPose();frontHands?.RefreshPose();
            }
        }
        private void Arm(bool right,Vector2 shoulder,Vector2 hand)
        {
            var b=right?6:3;var a=right?6:3;
            hand=ConstrainTarget(shoulder,hand,UpperArm,LowerArm,goalkeeper?0:8,145);
            var elbow=Elbow(shoulder,hand,UpperArm,LowerArm,goalkeeper?(right?-1:1):(right?1:-1));
            Limb(b,shoulder,elbow);Limb(b+1,elbow,hand);
            var wristAngle=Mathf.Atan2((hand-elbow).y,(hand-elbow).x)*Mathf.Rad2Deg+90;
            Joint(b+2,hand,wristAngle+(goalkeeper?180:0));
            Place(art[a],bones[b],new Vector2(0,-17.5f),new Vector2(18,40));
            Place(art[a+1],bones[b+1],new Vector2(0,-17),new Vector2(15,39));
            Place(art[a+2],bones[b+2],Vector2.zero,goalkeeper?new Vector2(21,25):new Vector2(14,18));
        }
        private void Leg(bool right,Vector2 hip,Vector2 foot,float ankle)
        {
            var b=right?12:9;var a=right?12:9;
            foot=ConstrainTarget(hip,foot,UpperLeg,LowerLeg,8,145);
            // Striker is seen from a rear three-quarter view, so both knees flex toward
            // the shot. The front-facing keeper has inward knee poles, not bowed legs.
            var knee=Elbow(hip,foot,UpperLeg,LowerLeg,goalkeeper?1:(right?1:-1));
            Limb(b,hip,knee);Limb(b+1,knee,foot);Joint(b+2,foot,Mathf.Clamp(ankle,-28,28));
            Place(art[a],bones[b],new Vector2(0,-24),new Vector2(24,54));
            Place(art[a+1],bones[b+1],new Vector2(0,-24),new Vector2(19,54));
            Place(art[a+2],bones[b+2],new Vector2(5,-3),new Vector2(31,17));
        }
        private void Joint(int index,Vector2 point,float rotation)
        {
            // Set in rig space, while keeping a real parent-child bone hierarchy.
            bones[index].position=transform.TransformPoint(point);
            bones[index].rotation=transform.rotation*Quaternion.Euler(0,0,rotation);
        }
        private void Limb(int index,Vector2 start,Vector2 end)=>Joint(index,start,Mathf.Atan2((end-start).y,(end-start).x)*Mathf.Rad2Deg+90);
        private static void Place(Image image,RectTransform bone,Vector2 position,Vector2 size)
        {image.rectTransform.position=bone.TransformPoint(position);image.rectTransform.rotation=bone.rotation;image.rectTransform.sizeDelta=size;}
        public static Vector2 Rotate(Vector2 p,float angle){var a=angle*Mathf.Deg2Rad;return new Vector2(p.x*Mathf.Cos(a)-p.y*Mathf.Sin(a),p.x*Mathf.Sin(a)+p.y*Mathf.Cos(a));}
        private static Vector2 ConstrainTarget(Vector2 root,Vector2 target,float first,float second,float minBend,float maxBend)
        {
            float Reach(float bend)=>Mathf.Sqrt(first*first+second*second+2*first*second*Mathf.Cos(bend*Mathf.Deg2Rad));
            var delta=target-root;var axis=delta.sqrMagnitude<.00001f?Vector2.down:delta.normalized;
            return root+axis*Mathf.Clamp(delta.magnitude,Reach(maxBend),Reach(minBend));
        }
        private static Vector2 Elbow(Vector2 root,Vector2 end,float first,float second,int bend)
        {
            var delta=end-root;var distance=Mathf.Clamp(delta.magnitude,.001f,first+second-.001f);
            var along=(first*first-second*second+distance*distance)/(2*distance);
            var height=Mathf.Sqrt(Mathf.Max(0,first*first-along*along));var axis=delta.normalized;
            return root+axis*along+new Vector2(-axis.y,axis.x)*height*bend;
        }
    }
}
