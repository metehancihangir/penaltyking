using UnityEngine;
using UnityEngine.UI;
namespace PenaltyKing
{
    // An untextured feathered ellipse, not a second black football sprite.
    [RequireComponent(typeof(Image))]
    public sealed class BallShadowMesh : BaseMeshEffect
    {
        public override void ModifyMesh(VertexHelper vh)
        {
            if(!IsActive())return;vh.Clear();var image=(Image)graphic;var r=image.rectTransform.rect;
            var c=image.color;c.a*=.65f;vh.AddVert(r.center,c,Vector2.zero);
            for(var i=0;i<32;i++){var a=i*Mathf.PI*2/32;var edge=c;edge.a=0;vh.AddVert(r.center+new Vector2(Mathf.Cos(a)*r.width*.5f,Mathf.Sin(a)*r.height*.5f),edge,Vector2.zero);}
            for(var i=0;i<32;i++)vh.AddTriangle(0,i+1,(i+1)%32+1);
        }
    }
}
