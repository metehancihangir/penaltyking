using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace PenaltyKing.Humanoid.Editor
{
    /// <summary>Import contract for the Blender sources; independent of the 2D game.</summary>
    public sealed class PenaltyHumanoidImport : AssetPostprocessor
    {
        public const string Folder = "Assets/Animations/PenaltyHumanoid";
        private void OnPreprocessModel()
        {
            if (!assetPath.StartsWith(Folder + "/", StringComparison.Ordinal) ||
                !assetPath.EndsWith(".fbx", StringComparison.OrdinalIgnoreCase)) return;
            var importer = (ModelImporter)assetImporter;
            importer.animationType = ModelImporterAnimationType.Human;
            importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
            importer.importAnimation = true;
            importer.resampleCurves = true;
            importer.animationCompression = ModelImporterAnimationCompression.Off;
            importer.importCameras = false;
            importer.importLights = false;
            importer.optimizeGameObjects = false;
            importer.useFileScale = true;
            importer.globalScale = 1;
            var names = new List<string> { "Hips", "Spine", "Chest", "UpperChest", "Neck", "Head" };
            foreach (var side in new[] { "Left", "Right" })
            {
                foreach (var limb in new[] { "UpperLeg", "LowerLeg", "Foot", "Toes", "Shoulder", "UpperArm", "LowerArm", "Hand" })
                    names.Add(side + limb);
                foreach (var finger in new[] { "Thumb", "Index", "Middle", "Ring", "Little" })
                    foreach (var segment in new[] { "Proximal", "Intermediate", "Distal" })
                        names.Add(side + finger + segment);
            }
            var description = importer.humanDescription;
            description.human = names.Select(name => new HumanBone {
                boneName = name,
                humanName = HumanTrait.BoneName[(int)Enum.Parse(typeof(HumanBodyBones), name)],
                limit = new HumanLimit { useDefaultValues = true }
            }).ToArray();
            description.upperArmTwist = .5f;
            description.lowerArmTwist = .5f;
            description.upperLegTwist = .5f;
            description.lowerLegTwist = .5f;
            description.armStretch = 0;
            description.legStretch = 0;
            description.feetSpacing = 0;
            description.hasTranslationDoF = false;
            importer.humanDescription = description;
            bool keeper = Path.GetFileNameWithoutExtension(assetPath) == "KeeperDiveRight";
            importer.clipAnimations = new[] { new ModelImporterClipAnimation {
                name = keeper ? "KeeperDiveRight_60fps" : "StrikerPenalty_60fps",
                firstFrame = 0,
                lastFrame = keeper ? 180 : 240,
                loopTime = false,
                loopPose = false,
                lockRootRotation = false,
                lockRootHeightY = false,
                lockRootPositionXZ = false,
                keepOriginalOrientation = true,
                keepOriginalPositionY = true,
                keepOriginalPositionXZ = true,
                heightFromFeet = false
            }};
        }

        [MenuItem("Penalty King/Blender/Build and Validate Humanoid Assets")]
        public static void BuildAndValidate()
        {
            var report = new List<string>();
            foreach (var name in new[] { "StrikerPenalty", "KeeperDiveRight" })
            {
                var path = Folder + "/" + name + ".fbx";
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                var model = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (model == null) throw new InvalidOperationException("Model missing: " + path);
                var imported = AssetDatabase.LoadAllAssetsAtPath(path);
                // Surface importer diagnostics in batch logs as well as the Inspector.
                var modelImporter = (ModelImporter)AssetImporter.GetAtPath(path);
                var serializedImporter = new SerializedObject(modelImporter);
                var diagnostic = serializedImporter.GetIterator();
                while (diagnostic.Next(true))
                    if (diagnostic.propertyType == SerializedPropertyType.String &&
                        (diagnostic.name.IndexOf("warning", StringComparison.OrdinalIgnoreCase) >= 0 ||
                         diagnostic.name.IndexOf("error", StringComparison.OrdinalIgnoreCase) >= 0) &&
                        !string.IsNullOrEmpty(diagnostic.stringValue))
                        Debug.Log(name + " import " + diagnostic.name + ": " + diagnostic.stringValue);
                var avatar = imported.OfType<Avatar>().Single();
                if (!avatar.isValid || !avatar.isHuman) throw new InvalidOperationException(name + " avatar is not valid Humanoid");
                var clip = imported.OfType<AnimationClip>().Single(c => !c.name.StartsWith("__preview__"));
                float expected = name == "StrikerPenalty" ? 4 : 3;
                if (Mathf.Abs(clip.length - expected) > .018f || Mathf.Abs(clip.frameRate - 60) > .01f)
                    throw new InvalidOperationException(name + " duration or sample rate mismatch: " + clip.length + "/" + clip.frameRate);
                if (clip.isLooping || !clip.humanMotion || !clip.hasRootCurves)
                    throw new InvalidOperationException(name + " must be non-looping Humanoid with root curves");
                var controllerPath = Folder + "/" + name + ".controller";
                var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);
                if (controller == null) controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
                var machine = controller.layers[0].stateMachine;
                foreach (var child in machine.states) machine.RemoveState(child.state);
                var state = machine.AddState(clip.name);
                state.motion = clip;
                state.writeDefaultValues = false;
                machine.defaultState = state;
                var instance = (GameObject)PrefabUtility.InstantiatePrefab(model);
                try
                {
                    var animator = instance.GetComponent<Animator>();
                    animator.avatar = avatar;
                    animator.runtimeAnimatorController = controller;
                    animator.applyRootMotion = true;
                    animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
                    var renderers = instance.GetComponentsInChildren<SkinnedMeshRenderer>();
                    if (renderers.Length == 0 || renderers.Any(r => r.sharedMesh == null || r.bones.Length == 0))
                        throw new InvalidOperationException(name + " skin binding missing");
                    if (renderers.Any(r => r.bounds.size.y < 1.4f || r.bounds.size.y > 2.3f))
                        throw new InvalidOperationException(name + " imported scale is not metres");
                    // Keep the bind pose prefab untouched; sample another temporary instance below.
                    PrefabUtility.SaveAsPrefabAsset(instance, Folder + "/" + name + ".prefab");
                    animator.Rebind();
                    animator.Play(state.name, 0, 0);
                    animator.Update(0);
                    var start = instance.transform.position;
                    var handMax = float.NegativeInfinity;
                    for (int i = 0; i < expected * 60; i++)
                    {
                        animator.Update(1f / 60);
                        var hand = animator.GetBoneTransform(HumanBodyBones.RightHand).position;
                        var hips = animator.GetBoneTransform(HumanBodyBones.Hips).position;
                        if (!float.IsFinite(hand.x) || !float.IsFinite(hips.y))
                            throw new InvalidOperationException(name + " has non-finite sampled pose");
                        handMax = Mathf.Max(handMax, hand.y);
                    }
                    var displacement = instance.transform.position - start;
                    if (new Vector2(displacement.x, displacement.z).magnitude < 2)
                        throw new InvalidOperationException(name + " root movement was lost: " + displacement);
                    report.Add(name + ": valid Humanoid, " + clip.frameRate + " FPS, " + clip.length +
                        " sec, root displacement " + displacement.ToString("F3") + ", peak right wrist " + handMax.ToString("F3") + " m");
                }
                finally { UnityEngine.Object.DestroyImmediate(instance); }
            }
            AssetDatabase.SaveAssets();
            Directory.CreateDirectory("TestResults");
            File.WriteAllLines("TestResults/blender-humanoid-validation.txt", report);
            Debug.Log("PENALTY_HUMANOID_VALIDATION_OK\n" + string.Join("\n", report));
        }
    }
}
