using UnityEngine;
using UnityEngine.UI;
namespace PenaltyKing
{
    // Native pixel geometry: grass and markings are separate from the stadium artwork.
    public sealed class PixelPitch : MaskableGraphic
    {
        [SerializeField] private float spotY;
        public float SpotY => spotY;
        public void SetSpot(float y) { if (Mathf.Approximately(spotY,y)) return; spotY=y; SetVerticesDirty(); }
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();var r=rectTransform.rect;
            for(var i=0;i<14;i++)
            {
                var top=Mathf.Lerp(r.yMax,r.yMin,Mathf.Pow(i/14f,1.35f));
                var bottom=Mathf.Lerp(r.yMax,r.yMin,Mathf.Pow((i+1)/14f,1.35f));
                Quad(vh,r.xMin,bottom,r.width,top-bottom,i%2==0?new Color32(53,109,42,255):new Color32(63,124,45,255));
            }
            // Repeatable small grass flecks, without a texture or per-frame allocations.
            for(var i=0;i<720;i++)
            {
                var x=r.xMin+((i*139+37)%997)/997f*r.width;
                var y=r.yMin+((i*277+91)%991)/991f*r.height;
                Quad(vh,x,y,2,3,new Color32(111,151,70,45));
            }
            var line=new Color32(210,225,179,255);
            var goalLine=r.yMax-24.625f;
            Quad(vh,r.xMin,goalLine-2,r.width,4,line);
            var depth=Mathf.Clamp((goalLine-spotY)*.50f,26,260);
            Box(vh,goalLine,335,405,depth,line);
            Box(vh,goalLine,252,280,depth*.38f,line);
            Quad(vh,-5,spotY-2,10,4,line);Quad(vh,-3,spotY-3,6,6,line);
        }
        private static void Box(VertexHelper vh,float top,float backWidth,float frontWidth,float depth,Color32 color)
        {
            Quad(vh,-frontWidth,top-depth-2,frontWidth*2,4,color);
            for(var side=-1;side<=1;side+=2)
                for(var i=0;i<40;i++)
                {
                    var x=side*Mathf.Lerp(backWidth,frontWidth,i/40f);
                    var next=side*Mathf.Lerp(backWidth,frontWidth,(i+1)/40f);
                    Quad(vh,Mathf.Min(x,next)-2,top-depth*(i+1)/40f,Mathf.Abs(next-x)+4,depth/40f+1,color);
                }
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

