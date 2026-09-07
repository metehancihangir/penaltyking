using UnityEngine;
namespace PenaltyKing
{
    [ExecuteAlways, RequireComponent(typeof(RectTransform))]
    public sealed class FitPanelToSafeArea : MonoBehaviour
    {
        public void Fit()
        {
            var rect=(RectTransform)transform;var parent=rect.parent as RectTransform;
            if(parent==null || rect.rect.width<=0 || rect.rect.height<=0)return;
            var scale=Mathf.Clamp(Mathf.Min((parent.rect.width-24)/rect.rect.width,(parent.rect.height-24)/rect.rect.height),.1f,1);
            rect.localScale=new Vector3(scale,scale,1);
        }
        private void OnEnable()=>Fit();
        private void LateUpdate()=>Fit();
    }
}
