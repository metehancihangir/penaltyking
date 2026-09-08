using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace PenaltyKing.Editor
{
 public static class UpdatePhaseSixSetup
 {
  [MenuItem("Penalty King/Updates/Phase 6 - Connect crowd and haptics")]
  public static void Build()
  {
   foreach(var name in new[]{"ChantDrumLoop","GoalOuff"})
   {
    var path="Assets/Audio/Stadium/"+name+".wav";AssetDatabase.ImportAsset(path,ImportAssetOptions.ForceSynchronousImport);
    var importer=(AudioImporter)AssetImporter.GetAtPath(path);var settings=importer.defaultSampleSettings;settings.loadType=AudioClipLoadType.DecompressOnLoad;settings.compressionFormat=AudioCompressionFormat.PCM;settings.preloadAudioData=true;importer.defaultSampleSettings=settings;importer.SaveAndReimport();
   }
   var scene=EditorSceneManager.OpenScene("Assets/Scenes/Gameplay.unity");var game=Object.FindFirstObjectByType<GameplayController>();var sound=game.GetComponent<GameplayAudio>();
   sound.Configure(AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Stadium/ChantDrumLoop.wav"),sound.KickClip,AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Stadium/GoalOuff.wav"),sound.SaveClip);
   if(game.GetComponent<GameplayHaptics>()==null)game.gameObject.AddComponent<GameplayHaptics>();
   EditorUtility.SetDirty(sound);EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();
   Debug.Log("[Update Phase 6] Chant with drums, ouff goal reaction and 65ms goal haptic connected.");
  }
 }
}
