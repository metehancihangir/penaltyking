using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
namespace PenaltyKing.Editor
{
 public static class UpdateSevenBuild
 {
  public static void Android()
  {
   PlayerSettings.companyName="PenaltyKing";PlayerSettings.productName="Penalty King";PlayerSettings.bundleVersion="0.2.0";PlayerSettings.Android.bundleVersionCode=2;
   PlayerSettings.SetApplicationIdentifier(UnityEditor.Build.NamedBuildTarget.Android,"com.penaltyking.game");PlayerSettings.Android.targetArchitectures=AndroidArchitecture.ARM64|AndroidArchitecture.X86_64;
   PlayerSettings.SetScriptingBackend(UnityEditor.Build.NamedBuildTarget.Android,ScriptingImplementation.IL2CPP);PlayerSettings.Android.minSdkVersion=AndroidSdkVersions.AndroidApiLevel26;PlayerSettings.defaultInterfaceOrientation=UIOrientation.AutoRotation;
   var path="Builds/Android/PenaltyKing-update7.apk";Directory.CreateDirectory("Builds/Android");
   var result=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=EditorBuildSettings.scenes.Where(x=>x.enabled).Select(x=>x.path).ToArray(),locationPathName=path,target=BuildTarget.Android,options=BuildOptions.Development});
   File.WriteAllText("Builds/Android/update7-build-summary.txt",$"{result.summary.result}\nSize: {result.summary.totalSize}\nErrors: {result.summary.totalErrors}\nDuration: {result.summary.totalTime}");
   if(result.summary.result!=BuildResult.Succeeded)throw new System.Exception("Update7 APK build failed");Debug.Log("[Update7] APK ready: "+path);
  }
 }
}
