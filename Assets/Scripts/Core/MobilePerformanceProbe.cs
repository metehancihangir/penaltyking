#if DEVELOPMENT_BUILD
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace PenaltyKing
{
 public sealed class MobilePerformanceProbe : MonoBehaviour
 {
  readonly List<float> frames=new List<float>(600);
  float windowStart,sceneStart;string lastState="";GameplayController game;FirstPlayGuide guide;
  void Awake(){SceneManager.sceneLoaded+=Loaded;Debug.Log($"[MobileDevice] model={SystemInfo.deviceModel} gpu={SystemInfo.graphicsDeviceName} memoryMB={SystemInfo.systemMemorySize} resolution={Screen.width}x{Screen.height}");}
  void Loaded(Scene scene,LoadSceneMode mode){frames.Clear();sceneStart=windowStart=Time.realtimeSinceStartup;lastState="";game=FindFirstObjectByType<GameplayController>();guide=FindFirstObjectByType<FirstPlayGuide>();StartCoroutine(Targets());}
  void LogTarget(Component target)
  {
   var canvas=target.GetComponentInParent<Canvas>();if(canvas==null)return;
   var point=RectTransformUtility.WorldToScreenPoint(canvas.worldCamera,target.transform.position);
   Debug.Log($"[MobileTarget] {target.name}|{Mathf.RoundToInt(point.x)}|{Mathf.RoundToInt(Screen.height-point.y)}");
  }
  IEnumerator Targets()
  {
   yield return new WaitForSecondsRealtime(.7f);
   Debug.Log("[MobileScene] "+SceneManager.GetActiveScene().name);
   foreach(var control in FindObjectsByType<Selectable>(FindObjectsSortMode.None))if(control.isActiveAndEnabled && control.interactable)LogTarget(control);
   if(game!=null && game.Handoff.Visible)LogTarget(game.Handoff);
  }
  void Update()
  {
   if(game==null)return;
   var round=game.Round;
   var state=$"state={game.State} shots={round?.ShotsTaken} p1={round?.Player1.Goals} p2={round?.Player2.Goals} guide={(guide!=null && guide.Visible?guide.Step:-1)} settings={game.SettingsOpen} size={Screen.width}x{Screen.height}";
   if(state!=lastState){lastState=state;Debug.Log("[MobileMatch] "+state);StartCoroutine(Targets());}
   var now=Time.realtimeSinceStartup;if(now-sceneStart<2){windowStart=now;return;}
   frames.Add(Time.unscaledDeltaTime*1000);
   if(now-windowStart>=5 && frames.Count>0)
   {
    var total=0f;foreach(var frame in frames)total+=frame;frames.Sort();
    Debug.Log(string.Format(CultureInfo.InvariantCulture,"[MobilePerformance] frames={0} fps={1:F2} p95Ms={2:F2} maxMs={3:F2}",frames.Count,1000*frames.Count/total,frames[(int)((frames.Count-1)*.95f)],frames[frames.Count-1]));frames.Clear();windowStart=now;
   }
  }
  void OnDestroy()=>SceneManager.sceneLoaded-=Loaded;
 }
}
#endif
