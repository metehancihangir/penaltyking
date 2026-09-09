using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace PenaltyKing.Editor
{
    public static class LandscapeRevision
    {
        [MenuItem("Penalty King/Apply Landscape Revision (no APK)")]
        public static void Setup()
        {
            PlayerSettings.defaultInterfaceOrientation=UIOrientation.AutoRotation;
            PlayerSettings.allowedAutorotateToPortrait=false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown=false;
            PlayerSettings.allowedAutorotateToLandscapeLeft=true;
            PlayerSettings.allowedAutorotateToLandscapeRight=true;
            const string path="Assets/Audio/Stadium/UserStadiumLoop.wav";
            AssetDatabase.ImportAsset(path,ImportAssetOptions.ForceSynchronousImport);
            var importer=(AudioImporter)AssetImporter.GetAtPath(path);
            var settings=importer.defaultSampleSettings;settings.loadType=AudioClipLoadType.DecompressOnLoad;
            settings.compressionFormat=AudioCompressionFormat.PCM;settings.preloadAudioData=true;
            importer.defaultSampleSettings=settings;importer.SaveAndReimport();
            var scene=EditorSceneManager.OpenScene("Assets/Scenes/Gameplay.unity");
            var sound=Object.FindFirstObjectByType<GameplayAudio>();
            sound.Configure(AssetDatabase.LoadAssetAtPath<AudioClip>(path),sound.KickClip,sound.GoalClip,sound.SaveClip);
            var goal=Object.FindFirstObjectByType<GameplayStageLayout>().transform.Find("05 Goal and Net");
            if(goal.GetComponent<GoalNetRipple>()==null)goal.gameObject.AddComponent<GoalNetRipple>();
            EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();
            Capture();
            EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
        }
        public static void Capture()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/Gameplay.unity");
            MenuPreview.Render(1280,720,"idle","landscape-revision");
            MenuPreview.Render(2340,1080,"wide","landscape-revision");
            MenuPreview.Render(1280,720,"idle","landscape-revision");
            var shot=Object.FindFirstObjectByType<ShotPresentation>();
            for(var i=0;i<8;i++)
            {
                shot.Sample(new ShotResult(ShotDirection.Right,ShotDirection.Left),.9f+i*.07f);
                MenuPreview.Render(1280,720,"net-"+i,"landscape-revision");
            }
            foreach(var time in new[]{1.3f,1.6f,2.1f})
            {
                shot.Sample(new ShotResult(ShotDirection.Right,ShotDirection.Right),time);
                MenuPreview.Render(1280,720,"keeper-"+Mathf.RoundToInt(time*10),"landscape-revision");
            }
        }
    }
}
