using UnityEngine;
using UnityEngine.UI;
namespace PenaltyKing
{
    // Native pixel geometry: grass and markings are separate from the stadium artwork.
    public sealed class PixelPitch : MaskableGraphic
    {
        [SerializeField] private float spotY;
        private float goalScale = 1;
        public float SpotY => spotY;
        public void SetGoalScale(float value) { if (Mathf.Approximately(goalScale,value)) return; goalScale=value; SetVerticesDirty(); }
        public void SetSpot(float y) { if (Mathf.Approximately(spotY,y)) return; spotY=y; SetVerticesDirty(); }
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();var r=rectTransform.rect;
            for(var i=0;i<14;i++)
            {
                var top=Mathf.Lerp(r.yMax,r.yMin,Mathf.Pow(i/14f,1.35f));
                var bottom=Mathf.Lerp(r.yMax,r.yMin,Mathf.Pow((i+1)/14f,1.35f));
                Quad(vh,r.xMin,bottom,r.width,top-bottom,i%2==0?new Color32(40,100,47,255):new Color32(49,115,49,255));
            }
            // Repeatable small grass flecks, without a texture or per-frame allocations.
            for(var i=0;i<720;i++)
            {
                var x=r.xMin+((i*139+37)%997)/997f*r.width;
                var y=r.yMin+((i*277+91)%991)/991f*r.height;
                var near=Mathf.InverseLerp(r.yMax,r.yMin,y);
                Quad(vh,x,y,1+near*2,1+near*2,new Color32(115,158,76,(byte)(22+near*16)));
            }
            var line=new Color32(226,234,202,255);
            var goalLine=r.yMax-24.625f;
            Quad(vh,r.xMin,goalLine-2,r.width,4,line);
            var depth=Mathf.Min(goalLine-spotY+80, goalLine-r.yMin-28);
            Box(vh,goalLine,315+(goalScale-1)*150,430+(goalScale-1)*70,depth,line);
            Box(vh,goalLine,252*goalScale,(280-(goalScale-1)*22)*goalScale,Mathf.Min(48*goalScale,depth*.24f),line);
            var edge=goalLine-depth;
            // The penalty arc sits outside the box, towards the camera.
            for(var i=0;i<64;i++)
            {
                var a=Mathf.PI*i/64;var b=Mathf.PI*(i+1)/64;
                var x=235*Mathf.Cos(a);var next=235*Mathf.Cos(b);
                var y=edge-78*Mathf.Sin(a);var nextY=edge-78*Mathf.Sin(b);
                Line(vh,new Vector2(x,y),new Vector2(next,nextY),3,line);
            }
            // Elliptical spot follows the pitch perspective.
            var start=vh.currentVertCount;vh.AddVert(new Vector2(0,spotY),line,Vector2.zero);
            for(var i=0;i<20;i++){var a=i*Mathf.PI*2/20;vh.AddVert(new Vector2(Mathf.Cos(a)*4.5f,spotY+Mathf.Sin(a)*2.5f),line,Vector2.zero);}
            for(var i=0;i<20;i++)vh.AddTriangle(start,start+1+i,start+1+(i+1)%20);
        }
        private static void Box(VertexHelper vh,float top,float backWidth,float frontWidth,float depth,Color32 color)
        {
            Line(vh,new Vector2(-frontWidth,top-depth),new Vector2(frontWidth,top-depth),3.5f,color);
            for(var side=-1;side<=1;side+=2)
                Line(vh,new Vector2(side*backWidth,top),new Vector2(side*frontWidth,top-depth),3.5f,color);
        }
        private static void Line(VertexHelper vh,Vector2 a,Vector2 b,float width,Color32 color)
        {
            var n=new Vector2(-(b-a).y,(b-a).x).normalized*width*.5f;var i=vh.currentVertCount;
            vh.AddVert(a-n,color,Vector2.zero);vh.AddVert(a+n,color,Vector2.zero);
            vh.AddVert(b+n,color,Vector2.zero);vh.AddVert(b-n,color,Vector2.zero);
            vh.AddTriangle(i,i+1,i+2);vh.AddTriangle(i,i+2,i+3);
        }
        private static void Quad(VertexHelper vh,float x,float y,float width,float height,Color32 color)
        {
            var start=vh.currentVertCount;
            vh.AddVert(new Vector3(x,y),color,Vector2.zero);vh.AddVert(new Vector3(x,y+height),color,Vector2.zero);
            vh.AddVert(new Vector3(x+width,y+height),color,Vector2.zero);vh.AddVert(new Vector3(x+width,y),color,Vector2.zero);
            vh.AddTriangle(start,start+1,start+2);vh.AddTriangle(start,start+2,start+3);
        }
    }
}
