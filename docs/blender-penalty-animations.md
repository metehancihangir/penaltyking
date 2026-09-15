# Blender penalty / Humanoid animation pack

This pack is independent of the existing 2D match presentation. It does not change
Gameplay, shot timing, scoring, or the existing sprite rigs.

## Deliverables

| Actor | Editable Blender source | Unity model and clip | Range | Duration |
| --- | --- | --- | --- | --- |
| Right-footed striker | `ArtSource/Blender/StrikerPenalty.blend` | `Assets/Animations/PenaltyHumanoid/StrikerPenalty.fbx` | 0–240 | 4 s |
| Right-diving goalkeeper | `ArtSource/Blender/KeeperDiveRight.blend` | `Assets/Animations/PenaltyHumanoid/KeeperDiveRight.fbx` | 0–180 | 3 s |

Both actions use 60 FPS. Inclusive endpoints give 241 / 181 pose samples; elapsed
duration is exactly 4 / 3 seconds. Review movies contain 240 / 180 frames at 60 FPS.

The original, procedural sports mannequins are **reference characters**, not
photoreal final character art. They include shirts, shorts, boots, goalkeeper
gloves and articulated fingers. Replace their meshes with final character art
through Humanoid retargeting. The animation is authored, not motion capture or a
rigid-body simulation. The keeper's airborne pelvis follows a gravity equation;
ground contact and limbs are authored.

## Motion beats

| Striker marker | Frame | Time (s) |
| --- | ---: | ---: |
| Preparation, breathing / head tracking | 0–30 | 0–0.500 |
| Curved, accelerating approach from 5 m | 31 | 0.517 |
| Deceptive deceleration | 94 | 1.567 |
| Left plant, 25 cm left of ball | 128 | 2.133 |
| Right-foot contact | 145 | 2.417 |
| Cross-body follow-through, airborne transfer | 159 | 2.650 |
| Right-leg landing | 173 | 2.883 |
| Deceleration, torso turn and fist pump | 176–240 | 2.933–4.000 |

| Keeper marker | Frame | Time (s) |
| --- | ---: | ---: |
| Ready stance and small foot adjustments | 0–40 | 0–0.667 |
| Reaction / lateral loading | 41 | 0.683 |
| Takeoff | 67 | 1.117 |
| Full right-hand extension / wrist parry | 91 | 1.517 |
| Side impact | 126 | 2.100 |
| Slide / roll | 150 | 2.500 |
| Kneeling recovery and head check | 180 | 3.000 |

These are **independent clips**, not a pre-synchronized shot/save pair. Align the
keeper's reaction with the game ball's trajectory when integrating. For example,
if parry occurs 0.45 s after striker contact, start the keeper clip at global time
`145/60 + 0.45 - 91/60 = 1.35 s`. Choose this from actual ball flight time.

The keeper clip is specifically a right high parry. It is not a two-handed catch,
and does not implement all six existing game outcomes. Celebration is part of the
requested striker sequence; use an appropriate separate recovery/transition for
failed shots in a future 3D integration.

## Open and play

### Blender 5.2

Open either `.blend`, select the armature and use the Timeline markers to inspect
the beats. Space plays the sequence. The saved camera gives a wide review view.
The source includes a lit turf stage and, for the striker, a contact reference
ball. Only the skinned character and skeleton are exported to FBX.

The editable rest pose is a T-pose. Each action has 53 bones, including all finger
chains, a ground-plane `Root`, and `Hips` carrying vertical movement. Local bone
rotations are quaternions with sign continuity. Every frame is baked, with linear
interpolation between 60 Hz samples and no export curve simplification. Units are
metres; source coordinates are Z up, actor forward −Y and anatomical right +X.

### Unity 6000.4

1. Open the project and allow FBX import. The scoped postprocessor configures a
   **Humanoid** Avatar, explicit bone mapping, non-looping clips, no compression
   and unbaked root translation/rotation.
2. Inspect either FBX's **Rig** and **Animation** tabs to preview its Avatar/clip.
3. Drag its companion `.prefab` into a separate test scene and enter Play mode.
   Its controller plays once, holds the final pose, and has `Apply Root Motion`
   enabled. No transitions silently return the athlete to a T-pose.
4. To retarget, use the clip/controller on a production character's Animator with
   that character's valid Humanoid Avatar. Check foot contacts on the target body;
   different limb proportions can require Animation Rigging / foot IK corrections.

The reference materials are imported FBX materials. A project's render pipeline
may need corresponding material conversion; the current game's Universal 2D
renderer is not a full 3D lighting setup. Blender previews are the visual reference.
No game callbacks or event receivers are attached. Contact timing is documented
above and marked in Blender, so playing a standalone preview cannot trigger
existing match logic.

## Rebuild and validate

From the repository root in PowerShell:

```powershell
$blenderExe = 'C:\Program Files\Blender Foundation\Blender 5.2\blender.exe'
& $blenderExe --background --python-exit-code 1 --python Tools/Blender/build_penalty_humanoid.py -- --preview
& $blenderExe --background --python-exit-code 1 --python Tools/Blender/review_penalty_humanoid.py -- --video
```

The generator is deterministic and uses Blender's own API, without downloads,
external textures or add-ons. Edit the landmark curves and marker frames in
`Tools/Blender/build_penalty_humanoid.py` to revise the choreography. Avoid running
the generator while Unity is importing these files.

In an already-open Unity Editor, run **Penalty King → Blender → Build and Validate
Humanoid Assets**. With the Editor closed, the equivalent command is:

```powershell
& 'C:\Program Files\Unity\Hub\Editor\6000.4.4f1\Editor\Unity.exe' -batchmode -nographics -projectPath . -executeMethod PenaltyKing.Humanoid.Editor.PenaltyHumanoidImport.BuildAndValidate -quit -logFile Logs/humanoid-import.log
```

The command creates/updates the companion prefabs and controllers and checks valid
Humanoid Avatars, frame rate, duration, non-looping behavior, mesh binding, metre
scale, finite sampled poses and actual Animator root movement. Its text report is
written to `TestResults/blender-humanoid-validation.txt`.

`review_penalty_humanoid.py` inspects the **saved Blender files**, checks plant drift,
ball contact, keeper reach and mesh/ground contact, then produces review MP4s.
Numerical results are saved in `docs/previews/blender-penalty/validation.json`.
The small studio-render movies review timing and silhouette; Cycles stills review
the source materials and lighting. These checks do not replace final art direction
or retargeting tests on a future production character.

No APK or mobile performance claim is included in this asset-only change.

## Verification results

Blender 5.2.1 and Unity 6000.4.4f1:

- Both imported Avatars are valid Humanoid; both clips retain 60 FPS and exact
  4 / 3 second durations. Animator playback preserves horizontal root motion.
- Striker plant drift, frames 128–145: below 0.001 mm at sampled precision.
  Boot contact landmark error: approximately **2.01 mm**.
- Keeper right arm extension: **99.9%**; fingertip height at frame 91:
  approximately **2.52 m**.
- Deformed reference meshes checked at all 422 authored samples. Striker clears
  the plane; keeper's smallest sole overlap is **3 mm**, inside the 5 mm turf
  contact tolerance.
- Adjacent joint positions checked to catch IK flips. Maximum movement is
  15.3 cm per frame for the striker's fast kicking foot and 17.3 cm for a keeper
  finger landmark. This is a continuity check, not a biomechanics certification.

Saved results: [Blender audit](previews/blender-penalty/validation.json) and
[Unity import / Animator validation](previews/blender-penalty/unity-validation.txt).

Review movies: [striker](previews/blender-penalty/StrikerPenalty-60fps.mp4) and
[keeper](previews/blender-penalty/KeeperDiveRight-60fps.mp4).
