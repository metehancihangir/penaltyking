using UnityEngine;
using UnityEngine.UI;
namespace PenaltyKing
{
    [RequireComponent(typeof(Image))]
    public sealed class GoalNetRipple : BaseMeshEffect
    {
        private float age = -1;
        private Vector2 hit;
        public bool Moving => age > 0 && age < 1.05f;
        public void Sample(float seconds, Vector2 impact)
        {
            if (Mathf.Approximately(age,seconds) && hit == impact) return;
            age=seconds; hit=impact; graphic.SetVerticesDirty();
        }
        public Vector2 Displacement(Vector2 point)
        {
            if (!Moving) return Vector2.zero;
            var rect=((RectTransform)transform).rect;
            var u=(point.x-rect.xMin)/rect.width; var v=(point.y-rect.yMin)/rect.height;
            // Pin the perimeter, side posts and crossbar; only the inner net flexes.
            var pin=Mathf.SmoothStep(0,1,Mathf.Clamp01((Mathf.Min(u,1-u)-.12f)/.16f))
                *Mathf.SmoothStep(0,1,Mathf.Clamp01((Mathf.Min(v,1-v)-.12f)/.16f));
            var distance=Vector2.Distance(point,hit);
            var wave=Mathf.Sin(age*23-distance*.065f)*Mathf.Exp(-age*4)
                *Mathf.Sin(Mathf.Min(1,age/.08f)*Mathf.PI*.5f)*Mathf.Clamp01((1.05f-age)/.15f);
            return new Vector2(wave*3.5f,wave*-11)*pin*Mathf.Exp(-distance/180);
        }
        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive()) return;
            var image=(Image)graphic;
            if (image.sprite == null) return;
            var rect=image.GetPixelAdjustedRect();
            var uv=UnityEngine.Sprites.DataUtility.GetOuterUV(image.sprite);
            vh.Clear(); const int columns=48, rows=20;
            for(var y=0;y<=rows;y++) for(var x=0;x<=columns;x++)
            {
                var u=x/(float)columns; var v=y/(float)rows;
                var point=new Vector2(Mathf.Lerp(rect.xMin,rect.xMax,u),Mathf.Lerp(rect.yMin,rect.yMax,v));
                vh.AddVert(point+Displacement(point),image.color,new Vector2(Mathf.Lerp(uv.x,uv.z,u),Mathf.Lerp(uv.y,uv.w,v)));
            }
            for(var y=0;y<rows;y++) for(var x=0;x<columns;x++)
            {
                var a=y*(columns+1)+x;var b=a+columns+1;
                vh.AddTriangle(a,b,b+1);vh.AddTriangle(a,b+1,a+1);
            }
        }
    }
}
