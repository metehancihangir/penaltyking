using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace PenaltyKing.Editor
{
    public static class PhaseZeroSetup
    {
        [MenuItem("Penalty King/Phase 0/Create missing infrastructure scenes")]
        public static void CreateProjectScenes()
        {
            EditorSettings.defaultBehaviorMode = EditorBehaviorMode.Mode2D;
            EditorSettings.serializationMode = SerializationMode.ForceText;
            PlayerSettings.companyName = "Penalty King";
            PlayerSettings.productName = "Penalty King";
            PlayerSettings.defaultScreenWidth = 1280;
            PlayerSettings.defaultScreenHeight = 720;
            var names = Enum.GetNames(typeof(GameScene));
            var buildScenes = new EditorBuildSettingsScene[names.Length];
            for (var index = 0; index < names.Length; index++)
            {
                var path = $"Assets/Scenes/{names[index]}.unity";
                // Never overwrite scenes that later phases or the user have edited.
                if (!File.Exists(path))
                {
                    var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                    var cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
                    cameraObject.tag = "MainCamera";
                    cameraObject.transform.position = new Vector3(0f, 0f, -10f);
                    var camera = cameraObject.GetComponent<Camera>();
                    camera.orthographic = true;
                    camera.orthographicSize = 5f;
                    camera.clearFlags = CameraClearFlags.SolidColor;
                    camera.backgroundColor = new Color(0.035f, 0.055f, 0.12f);
                    var infrastructure = new GameObject("Phase 0 Infrastructure");
                    infrastructure.AddComponent<SceneNavigator>();
                    infrastructure.AddComponent<TouchInputProbe>();
                    infrastructure.AddComponent<PhaseZeroDiagnostics>();
                    EditorSceneManager.SaveScene(scene, path);
                }
                buildScenes[index] = new EditorBuildSettingsScene(path, true);
            }
            EditorBuildSettings.scenes = buildScenes;
            AssetDatabase.SaveAssets();
            EditorSceneManager.OpenScene(buildScenes[0].path);
            Debug.Log("[Phase 0] Five 2D infrastructure scenes created and registered.");
        }
    }
}
