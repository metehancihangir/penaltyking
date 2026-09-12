using UnityEngine;
using UnityEngine.UI;

namespace PenaltyKing
{
    // Continuous skinning of a single drawing. Adjacent parts share vertices, so
    // the waist and knees cannot open into the gaps of the old cutout renderer.
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class FootballSkin : MaskableGraphic
    {
        [SerializeField] private Sprite drawing;
        [SerializeField] private RectTransform[] joints;
        [SerializeField] private Vector2[] bindPoints;
        [SerializeField] private float[] bindAngles;
        [SerializeField] private Vector2 pixelHips;
        [SerializeField] private bool keeper, handsOnly;
        private struct SkinVertex { public Vector2 point, uv; public int first, second; public float blend; }
        private SkinVertex[] vertices;
        private Vector3[] deformed;
        private int columns, rows;
        private readonly Matrix4x4[] matrices=new Matrix4x4[15];
        private static readonly int[] ends={1,2,2,4,5,5,7,8,8,10,11,11,13,14,14};
        public override Texture mainTexture=>drawing!=null?drawing.texture:base.mainTexture;
        public Vector2[] BindPoints=>bindPoints;
        public float[] BindAngles=>bindAngles;

        public void Configure(Sprite sprite,RectTransform[] bones,Vector2 hips,Vector2[] points,float[] angles,bool goalie,bool onlyHands=false)
        {
            drawing=sprite;joints=bones;pixelHips=hips;bindPoints=points;bindAngles=angles;keeper=goalie;handsOnly=onlyHands;
            raycastTarget=false;Build();SetAllDirty();
        }
        protected override void OnEnable(){base.OnEnable();if(drawing!=null)Build();}
        public void RefreshPose()=>SetVerticesDirty();
        private Vector2 End(int bone)
        {
            if(bone==2)return bindPoints[2]+Vector2.up*18;
            if(bone==5||bone==8)return bindPoints[bone]+(bindPoints[bone]-bindPoints[bone-1]).normalized*8;
            if(bone==11||bone==14)return bindPoints[bone]+new Vector2(bone==11?-12:16,-2);
            return bindPoints[ends[bone]];
        }
        private static float Distance(Vector2 p,Vector2 a,Vector2 b)
        {var d=b-a;return (p-Vector2.Lerp(a,b,Mathf.Clamp01(Vector2.Dot(p-a,d)/Mathf.Max(.001f,d.sqrMagnitude)))).sqrMagnitude;}
        private void Build()
        {
            var rect=drawing.rect;columns=Mathf.CeilToInt(rect.width/4);rows=Mathf.CeilToInt(rect.height/4);
            vertices=new SkinVertex[(columns+1)*(rows+1)];deformed=new Vector3[vertices.Length];var scale=210/rect.height;
            for(var y=0;y<=rows;y++)for(var x=0;x<=columns;x++)
            {
                var pixel=new Vector2(rect.x+rect.width*x/columns,rect.y+rect.height*y/rows);
                var p=(pixel-pixelHips)*scale;
                int start,count;
                var bodyWidth=Mathf.Lerp(37,25,Mathf.Clamp01(p.y/65));
                if(p.y>69){start=2;count=1;}
                else if(p.y>4 && Mathf.Abs(p.x)<bodyWidth){start=0;count=2;}
                else if(p.y>-13 && p.x<=-bodyWidth){start=3;count=3;}
                else if(p.y>-13 && p.x>=bodyWidth){start=6;count=3;}
                else if(p.y> -22 && Mathf.Abs(p.x)<42){start=0;count=1;}
                else {start=p.x<0?9:12;count=3;}
                var first=start;var second=start;var d1=float.MaxValue;var d2=float.MaxValue;
                for(var b=start;b<start+count;b++)
                {
                    var distance=Distance(p,bindPoints[b],End(b));
                    if(distance<d1){d2=d1;second=first;d1=distance;first=b;}
                    else if(distance<d2){d2=distance;second=b;}
                }
                // Blend only near a joint; distant limbs never influence each other.
                var blend=count==1?0:Mathf.Clamp01((20-Mathf.Sqrt(d2)) / 20)*.5f;
                if(start==0&&count==2){first=0;second=1;blend=Mathf.SmoothStep(0,1,Mathf.Clamp01(p.y/60));}
                vertices[y*(columns+1)+x]=new SkinVertex{point=p,uv=new Vector2(pixel.x/drawing.texture.width,pixel.y/drawing.texture.height),first=first,second=second,blend=blend};
            }
        }
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();if(drawing==null||joints==null)return;if(vertices==null)Build();
            for(var i=0;i<15;i++)
            {
                var bind=Matrix4x4.TRS(bindPoints[i],Quaternion.Euler(0,0,bindAngles[i]),Vector3.one);
                matrices[i]=transform.worldToLocalMatrix*joints[i].localToWorldMatrix*bind.inverse;
            }
            for(var index=0;index<vertices.Length;index++)
            {
                var vertex=vertices[index];
                var a=matrices[vertex.first].MultiplyPoint3x4(vertex.point);
                var b=matrices[vertex.second].MultiplyPoint3x4(vertex.point);
                deformed[index]=Vector3.Lerp(a,b,vertex.blend);
                vh.AddVert(deformed[index],color,vertex.uv);
            }
            for(var y=0;y<rows;y++)for(var x=0;x<columns;x++)
            {
                var i=y*(columns+1)+x;var j=i+columns+1;
                if(handsOnly && !Hand(vertices[i].first) && !Hand(vertices[j+1].first))continue;
                Triangle(vh,i,j,j+1);Triangle(vh,i,j+1,i+1);
            }
        }
        private void Triangle(VertexHelper vh,int a,int b,int c)
        {
            // A transparent area between two independent limbs is not a skin bridge.
            // Reject stretched bridge cells, rather than drawing long ribbons of fabric.
            var maximum=210/drawing.rect.height*4*4;
            if((deformed[a]-deformed[b]).sqrMagnitude>maximum*maximum ||
                (deformed[b]-deformed[c]).sqrMagnitude>maximum*maximum ||
                (deformed[c]-deformed[a]).sqrMagnitude>maximum*maximum)return;
            vh.AddTriangle(a,b,c);
        }
        private static bool Hand(int b)=>b==5||b==8;
    }
}
