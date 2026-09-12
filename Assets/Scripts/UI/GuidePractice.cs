using UnityEngine;
using UnityEngine.UI;
namespace PenaltyKing
{
 public sealed class GuidePractice : MonoBehaviour
 {
  [SerializeField] private FirstPlayGuide guide;
  [SerializeField] private GameplayController game;
  [SerializeField] private Button[] targets;
  [SerializeField] private Image[] dots;
  [SerializeField] private Text detail;
  private int lastStep=-1;
  public void Configure(FirstPlayGuide owner,GameplayController match,Button[] choices,Image[] pages,Text label){guide=owner;game=match;targets=choices;dots=pages;detail=label;}
  private void Awake(){for(var i=0;i<targets.Length;i++){var index=i;targets[i].onClick.AddListener(()=>Select(index));}}
  private void Select(int index)
  {
   if(!guide.Visible || game.SettingsOpen)return;
   for(var i=0;i<targets.Length;i++)targets[i].targetGraphic.color=i==index?new Color32(186,235,113,255):new Color32(39,70,83,255);
   detail.text=guide.Step==0?"Harika! Seçimin gizli kalır.":"Aynı bölge = kurtarış!\nFarklı bölge = gol!";
  }
  private void LateUpdate()
  {
   if(guide.Step==lastStep)return;lastStep=guide.Step;
   for(var i=0;i<dots.Length;i++)dots[i].color=i<=lastStep?new Color32(186,235,113,255):new Color32(39,70,83,255);
   foreach(var target in targets)target.targetGraphic.color=new Color32(39,70,83,255);
  }
 }
}
