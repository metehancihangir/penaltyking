using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace PenaltyKing.Editor
{
    public static class PhaseEightBuild
    {
        [MenuItem("Penalty King/Phase 8/Build Android development APK")]
        public static void Android()
        {
            PlayerSettings.companyName = "PenaltyKing";
            PlayerSettings.productName = "Penalty King";
            PlayerSettings.SetApplicationIdentifier(UnityEditor.Build.NamedBuildTarget.Android, "com.penaltyking.game");
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64 | AndroidArchitecture.X86_64;
            PlayerSettings.SetScriptingBackend(UnityEditor.Build.NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
            Directory.CreateDirectory("Builds/Android");
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path).ToArray(),
                locationPathName = "Builds/Android/PenaltyKing-development.apk", target = BuildTarget.Android,
                options = BuildOptions.Development
            });
            File.WriteAllText("Builds/Android/build-summary.txt", $"{report.summary.result}\nSize: {report.summary.totalSize}\nErrors: {report.summary.totalErrors}\nDuration: {report.summary.totalTime}");
            if (report.summary.result != BuildResult.Succeeded) throw new System.InvalidOperationException("Android build failed: " + report.summary.result);
            Debug.Log("[Phase 8] Android APK built successfully.");
        }
    }
}
