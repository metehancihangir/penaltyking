using UnityEngine;
using UnityEngine.UI;
namespace PenaltyKing
{
    // Six quiet brackets appear only while an input can be made; they never reveal a choice.
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class TargetReticle : MaskableGraphic
    {
        private ShotZone zone;
        protected override void Awake(){base.Awake();zone=GetComponentInParent<ShotZone>();raycastTarget=false;}
        private void LateUpdate()
        {
            if(!Application.isPlaying)return;
            if(zone==null)zone=GetComponentInParent<ShotZone>();
            var c=color;c.a=zone!=null&&zone.interactable?.75f:0;if(color!=c)color=c;
        }
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();var r=rectTransform.rect;float d=9,w=1.5f;
            for(int x=-1;x<=1;x+=2)for(int y=-1;y<=1;y+=2)
            {
                var p=new Vector2(x*r.width*.5f,y*r.height*.5f);
                Bar(vh,p,new Vector2(p.x-x*d,p.y),w);Bar(vh,p,new Vector2(p.x,p.y-y*d),w);
            }
            Bar(vh,new Vector2(-3,0),new Vector2(3,0),1);Bar(vh,new Vector2(0,-3),new Vector2(0,3),1);
        }
        private void Bar(VertexHelper vh,Vector2 a,Vector2 b,float width)
        {
            var n=new Vector2(-(b-a).y,(b-a).x).normalized*width*.5f;var i=vh.currentVertCount;
            vh.AddVert(a-n,color,Vector2.zero);vh.AddVert(a+n,color,Vector2.zero);vh.AddVert(b+n,color,Vector2.zero);vh.AddVert(b-n,color,Vector2.zero);
            vh.AddTriangle(i,i+1,i+2);vh.AddTriangle(i,i+2,i+3);
        }
    }
}
