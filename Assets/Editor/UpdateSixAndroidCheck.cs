using System.IO;
using System.Xml;
using UnityEditor;
using UnityEditor.Build.Player;
using UnityEngine;
namespace PenaltyKing.Editor
{
 public static class UpdateSixAndroidCheck
 {
  public static void Run()
  {
   var folder="Library/Update6AndroidCheck";Directory.CreateDirectory(folder);
   var settings=new ScriptCompilationSettings{target=BuildTarget.Android,group=BuildTargetGroup.Android};
   var result=PlayerBuildInterface.CompilePlayerScripts(settings,folder);
   if(result.assemblies==null || result.assemblies.Count==0)throw new System.Exception("Android script compilation failed");
   var manifestFolder=Path.Combine(folder,"manifest-fixture");Directory.CreateDirectory(Path.Combine(manifestFolder,"src/main"));var file=Path.Combine(manifestFolder,"src/main/AndroidManifest.xml");
   File.WriteAllText(file,"<manifest xmlns:android=\"http://schemas.android.com/apk/res/android\" package=\"com.penaltyking.game\"><application /></manifest>");
   var hook=new HapticsAndroidManifest();hook.OnPostGenerateGradleAndroidProject(manifestFolder);hook.OnPostGenerateGradleAndroidProject(manifestFolder);
   var xml=new XmlDocument();xml.Load(file);if(xml.SelectNodes("/manifest/uses-permission").Count!=1)throw new System.Exception("Vibration permission must appear once");
   Debug.Log("[Update Phase 6] Android player scripts compile; manifest VIBRATE permission is idempotent. This is not a device test or APK build.");
  }
 }
}

