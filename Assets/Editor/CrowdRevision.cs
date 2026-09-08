using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Reporting;
using UnityEngine;
namespace PenaltyKing.Editor
{
    public static class CrowdRevision
    {
        public static void Setup()
        {
            foreach(var name in new[]{"CrowdChant","GoalCrowd"})
            {
                var path=$"Assets/Audio/Stadium/{name}.wav";
                AssetDatabase.ImportAsset(path,ImportAssetOptions.ForceSynchronousImport);
                var importer=(AudioImporter)AssetImporter.GetAtPath(path);
                var settings=importer.defaultSampleSettings;
                settings.loadType=AudioClipLoadType.DecompressOnLoad;
                settings.compressionFormat=AudioCompressionFormat.PCM;
                settings.preloadAudioData=true;
                importer.defaultSampleSettings=settings;importer.SaveAndReimport();
            }
            var scene=EditorSceneManager.OpenScene("Assets/Scenes/Gameplay.unity");
            var sound=Object.FindFirstObjectByType<GameplayAudio>();
            sound.Configure(AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Stadium/CrowdChant.wav"),sound.KickClip,AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Stadium/GoalCrowd.wav"),sound.SaveClip);
            EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);
            MenuPreview.Render(390,844,"portrait","crowd-revision");
            MenuPreview.Render(1280,720,"landscape","crowd-revision");
            EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
        }
        public static void Android()
        {
            PlayerSettings.bundleVersion="0.2.2";PlayerSettings.Android.bundleVersionCode=4;
            const string path="Builds/Android/PenaltyKing-update7-crowd.apk";
            var report=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=EditorBuildSettings.scenes.Where(s=>s.enabled).Select(s=>s.path).ToArray(),locationPathName=path,target=BuildTarget.Android,options=BuildOptions.Development});
            if(report.summary.result!=BuildResult.Succeeded)throw new System.Exception("Crowd revision build failed");
            File.WriteAllText("Builds/Android/crowd-revision-build.txt",$"Succeeded\nBytes: {new FileInfo(path).Length}\nErrors: {report.summary.totalErrors}");
        }
    }
}
