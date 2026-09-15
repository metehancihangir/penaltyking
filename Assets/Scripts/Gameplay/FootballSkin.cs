using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using UnityEngine.U2D.Animation;
using UnityEngine.Rendering;
using Unity.Collections;
using System.Collections.Generic;

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
        [SerializeField] private SpriteSkin nativeSkin;
        public SpriteSkin NativeSkin=>nativeSkin;
        public bool UsedNativeDeformation {get;private set;}
        public void ConfigureNative(SpriteSkin value)=>nativeSkin=value;
        [System.Serializable] private struct SkinVertex { public Vector2 point, uv; public BoneWeight weights; }
        [SerializeField] private SkinVertex[] vertices;
        [SerializeField] private bool[] surface;
        private Vector3[] deformed;
        [SerializeField] private int columns, rows;
        private readonly Matrix4x4[] matrices=new Matrix4x4[17];
        public float MaximumNativeDeviation {get;private set;}
        private static readonly int[] ends={1,2,2,4,5,5,7,8,8,10,11,11,13,14,14};
        public override Texture mainTexture=>drawing!=null?drawing.texture:base.mainTexture;
        public Vector2[] BindPoints=>bindPoints;
        public float[] BindAngles=>bindAngles;

        public void Configure(Sprite sprite,RectTransform[] bones,Vector2 hips,Vector2[] points,float[] angles,bool goalie,bool onlyHands=false)
        {
            drawing=sprite;joints=bones;pixelHips=hips;bindPoints=points;bindAngles=angles;keeper=goalie;handsOnly=onlyHands;
            raycastTarget=false;Build();SetAllDirty();
        }
        protected override void OnEnable(){base.OnEnable();if(drawing!=null){if(vertices==null||vertices.Length==0)Build();deformed=new Vector3[vertices.Length];}}
        public void RefreshPose()=>SetVerticesDirty();
        private Vector2 End(int bone)
        {
            if(bone==0)return bindPoints[15];
            if(bone==15)return bindPoints[16];
            if(bone==16)return bindPoints[1];
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
            var strengths=new float[17];var indices=new int[4];var weight=new float[4];
            for(var y=0;y<=rows;y++)for(var x=0;x<=columns;x++)
            {
                var pixel=new Vector2(rect.x+rect.width*x/columns,rect.y+rect.height*y/rows);
                var p=(pixel-pixelHips)*scale;
                for(var b=0;b<bindPoints.Length;b++)
                {
                    var leg=b>=9&&b<=14;var arm=b>=3&&b<=8;var spine=b==0||b==1||b>=15;
                    var distance=Distance(p,bindPoints[b],End(b));
                    if(spine&&Mathf.Abs(p.x)<26&&p.y>0)distance*=.35f;
                    if(b==2&&p.y>74)distance*=.2f;
                    var influence=1/Mathf.Pow(distance+28,2);
                    // Fade anatomical regions; hard region switches tear a shared mesh.
                    if(leg)influence*=1-Mathf.SmoothStep(0,1,Mathf.InverseLerp(-8,15,p.y));
                    if(arm)influence*=Mathf.SmoothStep(0,1,Mathf.InverseLerp(-30,-12,p.y));
                    if(b==2)influence*=Mathf.SmoothStep(0,1,Mathf.InverseLerp(52,75,p.y));
                    if(spine)influence*=Mathf.SmoothStep(0,1,Mathf.InverseLerp(-38,-12,p.y));
                    strengths[b]=influence;
                }
                var total=0f;
                for(var n=0;n<4;n++)
                {
                    var best=0;for(var b=1;b<17;b++)if(strengths[b]>strengths[best])best=b;
                    indices[n]=best;weight[n]=strengths[best];total+=weight[n];strengths[best]=0;
                }
                var weights=new BoneWeight{boneIndex0=indices[0],boneIndex1=indices[1],boneIndex2=indices[2],boneIndex3=indices[3],
                    weight0=weight[0]/total,weight1=weight[1]/total,weight2=weight[2]/total,weight3=weight[3]/total};
                vertices[y*(columns+1)+x]=new SkinVertex{point=p,uv=new Vector2(pixel.x/drawing.texture.width,pixel.y/drawing.texture.height),weights=weights};
            }
        }
#if UNITY_EDITOR
        // Diffuse along the painted silhouette, never across the air between a hand
        // and shorts. The resulting four weights are baked, not solved during play.
        public void BakeSurfaceWeights(Color32[] pixels,int width,int height)
        {
            var n=vertices.Length;var valid=new bool[n];var fixedVertex=new bool[n];
            var values=new float[n*17];var next=new float[values.Length];var pitch=columns+1;
            bool Opaque(int x,int y)
            {
                if(x<0||y<0||x>=width||y>=height)return false;
                var p=pixels[y*width+x];return p.a>100&&!(p.r>110&&p.b>85&&p.g<Mathf.Min(p.r,p.b)*.6f);
            }
            for(var i=0;i<n;i++)
            {
                var pixel=vertices[i].uv*new Vector2(width,height);
                for(var dy=-2;dy<=2;dy++)for(var dx=-2;dx<=2;dx++)valid[i]|=Opaque(Mathf.RoundToInt(pixel.x)+dx,Mathf.RoundToInt(pixel.y)+dy);
                var w=vertices[i].weights;values[i*17+w.boneIndex0]+=w.weight0;values[i*17+w.boneIndex1]+=w.weight1;
                values[i*17+w.boneIndex2]+=w.weight2;values[i*17+w.boneIndex3]+=w.weight3;
                var nearest=float.MaxValue;
                for(var b=0;b<17;b++)nearest=Mathf.Min(nearest,Distance(vertices[i].point,bindPoints[b],End(b)));
                fixedVertex[i]=nearest<9;
            }
            var neighbours=new int[4];
            for(var iteration=0;iteration<240;iteration++)
            {
                System.Array.Copy(values,next,values.Length);
                for(var i=0;i<n;i++)
                {
                    if(!valid[i]||fixedVertex[i])continue;
                    var count=0;var x=i%pitch;
                    if(x>0&&valid[i-1])neighbours[count++]=i-1;
                    if(x<columns&&valid[i+1])neighbours[count++]=i+1;
                    if(i>=pitch&&valid[i-pitch])neighbours[count++]=i-pitch;
                    if(i<n-pitch&&valid[i+pitch])neighbours[count++]=i+pitch;
                    if(count==0)continue;
                    for(var b=0;b<17;b++){var sum=0f;for(var k=0;k<count;k++)sum+=values[neighbours[k]*17+b];next[i*17+b]=sum/count;}
                }
                var swap=values;values=next;next=swap;
            }
            // Extend edge weights into transparent grid vertices without bridging limbs.
            var seen=(bool[])valid.Clone();var queue=new Queue<int>();for(var i=0;i<n;i++)if(valid[i])queue.Enqueue(i);
            while(queue.Count>0)
            {
                var i=queue.Dequeue();var x=i%pitch;var count=0;
                if(x>0)neighbours[count++]=i-1;if(x<columns)neighbours[count++]=i+1;
                if(i>=pitch)neighbours[count++]=i-pitch;if(i<n-pitch)neighbours[count++]=i+pitch;
                for(var k=0;k<count;k++){var j=neighbours[k];if(seen[j])continue;seen[j]=true;System.Array.Copy(values,i*17,values,j*17,17);queue.Enqueue(j);}
            }
            var indices=new int[4];var weights=new float[4];
            for(var i=0;i<n;i++)
            {
                var total=0f;
                for(var k=0;k<4;k++){var best=0;for(var b=1;b<17;b++)if(values[i*17+b]>values[i*17+best])best=b;indices[k]=best;weights[k]=values[i*17+best];values[i*17+best]=0;total+=weights[k];}
                vertices[i].weights=new BoneWeight{boneIndex0=indices[0],boneIndex1=indices[1],boneIndex2=indices[2],boneIndex3=indices[3],
                    weight0=weights[0]/total,weight1=weights[1]/total,weight2=weights[2]/total,weight3=weights[3]/total};
            }
            surface=valid;SetVerticesDirty();
        }
#endif
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();if(drawing==null||joints==null)return;if(vertices==null||vertices.Length==0)Build();if(deformed==null||deformed.Length!=vertices.Length)deformed=new Vector3[vertices.Length];
            UsedNativeDeformation=Application.isPlaying&&nativeSkin!=null&&nativeSkin.HasCurrentDeformedVertices();
            if(UsedNativeDeformation)
            {
                var index=0;foreach(var point in nativeSkin.GetDeformedVertexPositionData())deformed[index++]=point;
            }
            for(var i=0;i<joints.Length;i++)
            {
                var bind=Matrix4x4.TRS(bindPoints[i],Quaternion.Euler(0,0,bindAngles[i]),Vector3.one);
                matrices[i]=transform.worldToLocalMatrix*joints[i].localToWorldMatrix*bind.inverse;
            }
            MaximumNativeDeviation=0;
            for(var index=0;index<vertices.Length;index++)
            {
                var vertex=vertices[index];var w=vertex.weights;
                var point=matrices[w.boneIndex0].MultiplyPoint3x4(vertex.point)*w.weight0+
                    matrices[w.boneIndex1].MultiplyPoint3x4(vertex.point)*w.weight1+
                    matrices[w.boneIndex2].MultiplyPoint3x4(vertex.point)*w.weight2+
                    matrices[w.boneIndex3].MultiplyPoint3x4(vertex.point)*w.weight3;
                if(!UsedNativeDeformation)deformed[index]=point;
                else MaximumNativeDeviation=Mathf.Max(MaximumNativeDeviation,(point-deformed[index]).magnitude);
                vh.AddVert(deformed[index],color,vertex.uv);
            }
            for(var y=0;y<rows;y++)for(var x=0;x<columns;x++)
            {
                var i=y*(columns+1)+x;var j=i+columns+1;
                if(surface!=null&&surface.Length==vertices.Length&&!surface[i]&&!surface[j]&&!surface[i+1]&&!surface[j+1])continue;
                if(handsOnly && !Hand(vertices[i].weights.boneIndex0) && !Hand(vertices[j+1].weights.boneIndex0))continue;
                Triangle(vh,i,j,j+1);Triangle(vh,i,j+1,i+1);
            }
        }
        private void Triangle(VertexHelper vh,int a,int b,int c)
        {
            vh.AddTriangle(a,b,c);
        }
        private static bool Hand(int b)=>b==5||b==8;
        public Sprite CreateWeightedSprite()
        {
            if(vertices==null)Build();var r=drawing.rect;
            var sprite=Sprite.Create(drawing.texture,r,new Vector2((pixelHips.x-r.x)/r.width,(pixelHips.y-r.y)/r.height),r.height/210,0,SpriteMeshType.FullRect);
            var positions=new NativeArray<Vector3>(vertices.Length,Allocator.Temp);
            var weights=new NativeArray<BoneWeight>(vertices.Length,Allocator.Temp);
            for(var i=0;i<vertices.Length;i++)
            {var v=vertices[i];positions[i]=v.point;weights[i]=v.weights;}
            var indices=new List<ushort>();
            for(var y=0;y<rows;y++)for(var x=0;x<columns;x++){var i=y*(columns+1)+x;var j=i+columns+1;indices.Add((ushort)i);indices.Add((ushort)j);indices.Add((ushort)(j+1));indices.Add((ushort)i);indices.Add((ushort)(j+1));indices.Add((ushort)(i+1));}
            using(var nativeIndices=new NativeArray<ushort>(indices.ToArray(),Allocator.Temp))
            {
                sprite.SetVertexCount(vertices.Length);sprite.SetVertexAttribute(VertexAttribute.Position,positions);
                sprite.SetVertexAttribute(VertexAttribute.BlendWeight,weights);sprite.SetIndices(nativeIndices);
            }
            positions.Dispose();weights.Dispose();
            var bind=new NativeArray<Matrix4x4>(joints.Length,Allocator.Temp);
            try
            {
                var data=new SpriteBone[joints.Length];var parents=new[]{-1,16,1,1,3,4,1,6,7,0,9,10,0,12,13,0,15};
                for(var i=0;i<data.Length;i++)
                {
                    var matrix=Matrix4x4.TRS(bindPoints[i],Quaternion.Euler(0,0,bindAngles[i]),Vector3.one);bind[i]=matrix.inverse;
                    var parent=parents[i];var local=parent<0?matrix:Matrix4x4.TRS(bindPoints[parent],Quaternion.Euler(0,0,bindAngles[parent]),Vector3.one).inverse*matrix;
                    data[i]=new SpriteBone{name=joints[i].name,parentId=parent,position=local.GetColumn(3),rotation=local.rotation,length=20};
                }
                sprite.SetBindPoses(bind);sprite.SetBones(data);
            }
            finally{bind.Dispose();}
            return sprite;
        }
    }
}
