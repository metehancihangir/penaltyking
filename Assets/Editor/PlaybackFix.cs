using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEditor.Build.Reporting;
namespace PenaltyKing.Editor
{
    public static class PlaybackFix
    {
        public static void Android()
        {
            Capture();
            EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
            PlayerSettings.bundleVersion = "0.2.1";
            PlayerSettings.Android.bundleVersionCode = 3;
            PlayerSettings.runInBackground = true;
            const string path = "Builds/Android/PenaltyKing-update7-fix.apk";
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions {
                scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray(),
                locationPathName = path, target = BuildTarget.Android, options = BuildOptions.Development
            });
            if (report.summary.result != BuildResult.Succeeded) throw new System.Exception("Playback fix build failed");
            File.WriteAllText("Builds/Android/playback-fix-build.txt", $"{report.summary.result}\nBytes: {new FileInfo(path).Length}\nErrors: {report.summary.totalErrors}\nDuration: {report.summary.totalTime}");
        }
        public static void Capture()
        {
            AudioPreferences.Load(out var music, out var sfx);
            Debug.Log($"[PlaybackFix] Saved volumes: music={music:R} sfx={sfx:R}");
            EditorSceneManager.OpenScene("Assets/Scenes/Gameplay.unity");
            MenuPreview.Render(390,844,"portrait","playback-fix");
            MenuPreview.Render(1280,720,"landscape","playback-fix");
        }
    }
}
