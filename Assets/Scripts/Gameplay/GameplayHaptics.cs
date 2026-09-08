using System;
using UnityEngine;
namespace PenaltyKing
{
 [DisallowMultipleComponent, RequireComponent(typeof(ShotPresentation))]
 public sealed class GameplayHaptics : MonoBehaviour
 {
  public const long PulseMilliseconds=65;
  public event Action<long> PulseRequested;
  private ShotPresentation presentation;
  private bool resolved;
  private void OnEnable(){presentation=GetComponent<ShotPresentation>();presentation.Kick+=OnKick;presentation.Impact+=OnImpact;}
  private void OnKick()=>resolved=false;
  private void OnImpact(ShotResult shot)
  {
   if(resolved)return;resolved=true;
   if(shot.Outcome!=ShotOutcome.Goal || !GameManager.Instance.VibrationEnabled)return;
   PulseRequested?.Invoke(PulseMilliseconds);
   DeviceHaptics.Pulse(PulseMilliseconds);
  }
  private void OnDisable(){if(presentation!=null){presentation.Kick-=OnKick;presentation.Impact-=OnImpact;}}
 }
 public static class DeviceHaptics
 {
  public static void Pulse(long milliseconds)
  {
#if UNITY_ANDROID && !UNITY_EDITOR
   try
   {
    using(var player=new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
    using(var activity=player.GetStatic<AndroidJavaObject>("currentActivity"))
    using(var vibrator=activity.Call<AndroidJavaObject>("getSystemService","vibrator"))
    {
     if(vibrator==null || !vibrator.Call<bool>("hasVibrator"))return;
     using(var effectClass=new AndroidJavaClass("android.os.VibrationEffect"))
     using(var effect=effectClass.CallStatic<AndroidJavaObject>("createOneShot",milliseconds,-1))
      vibrator.Call("vibrate",effect);
    }
   }
   catch(AndroidJavaException exception){Debug.LogWarning("Haptic unavailable: "+exception.Message);}
#endif
  }
 }
}
