"""Validate the saved .blend files and render compact, 60 FPS motion reviews.
Run after build_penalty_humanoid.py. Pass -- --video to render review MP4s.
"""
import bpy
import json
import sys
from pathlib import Path
from mathutils import Vector

BASE = Path(__file__).resolve().parents[2]
OUT = BASE/'docs/previews/blender-penalty'
reports=[]
video_reports=[]
for name,end in [('StrikerPenalty',240),('KeeperDiveRight',180)]:
    bpy.ops.wm.open_mainfile(filepath=str(BASE/'ArtSource/Blender'/(name+'.blend')))
    scene=bpy.context.scene
    rig=bpy.data.objects[name]
    samples=[]
    mesh=next(o for o in rig.children if o.type=='MESH')
    floor_min=0
    floor_detail={}
    for f in range(end+1):
        scene.frame_set(f)
        joints={bone.name: list(rig.matrix_world@bone.head) for bone in rig.pose.bones}
        samples.append(joints)
        # Check the deformed mesh at every authored frame.
        evaluated=mesh.evaluated_get(bpy.context.evaluated_depsgraph_get())
        geometry=evaluated.to_mesh()
        vertex=min(geometry.vertices,key=lambda v:(evaluated.matrix_world@v.co).z)
        low=(evaluated.matrix_world@vertex.co).z
        if low<floor_min:
            floor_min=low
            floor_detail={'frame':f,'bone':mesh.vertex_groups[max(vertex.groups,key=lambda g:g.weight).group].name}
        evaluated.to_mesh_clear()
    report={'name':name,'fps':scene.render.fps,'duration_seconds':end/scene.render.fps,'bones':len(rig.pose.bones),'minimum_mesh_z_m':round(floor_min,5),'floor_detail':floor_detail}
    jump=max(((Vector(b[n])-Vector(a[n])).length,f+1,n) for f,(a,b) in enumerate(zip(samples,samples[1:])) for n in a)
    report['largest_joint_step_m']=jump[0]
    report['largest_joint_step_frame']=jump[1]
    report['largest_joint_step_bone']=jump[2]
    assert len(rig.pose.bones)==53
    assert scene.render.fps==60
    assert all(abs(value)<100 for sample in samples for joint in sample.values() for value in joint)
    if name=='StrikerPenalty':
        planted=Vector(samples[128]['LeftFoot'])
        drift=max((Vector(samples[f]['LeftFoot'])-planted).length for f in range(128,146))
        contact=(Vector(samples[145]['RightToes'])-Vector((0,0,.11))).length
        report.update(plant_drift_m=round(drift,6),contact_error_m=round(contact,6))
        assert drift<.005, ('Plant slide',drift)
        assert contact<.035, ('Ball contact',contact)
    else:
        wrist=Vector(samples[91]['RightHand'])
        shoulder=Vector(samples[91]['RightUpperArm'])
        extension=(wrist-shoulder).length/.58
        report.update(parry_wrist_position_m=list(wrist),arm_extension_ratio=extension)
        assert extension>.98, ('Parry elbow still bent',extension)
        fingertip=rig.pose.bones['RightMiddleDistal']
        scene.frame_set(91)
        tip=rig.matrix_world@fingertip.tail
        report['parry_fingertip_height_m']=tip.z
        assert tip.z>2.44, ('Parry below crossbar',tip.z)
    reports.append(report)
    print('BLENDER_MOTION_AUDIT',json.dumps(report))
    assert floor_min>-.005, ('Mesh penetrates turf',floor_min,floor_detail)
    assert jump[0]<.25, ('Discontinuous pose',jump)
    if '--stills' in sys.argv:
        frames=[0,53,75,91,110,126,150,180] if name=='KeeperDiveRight' else [0,52,94,128,145,159,173,220]
        for f in frames:
            scene.frame_set(f)
            scene.render.filepath=str(OUT/(name+f'-{f:03d}.png'))
            bpy.ops.render.render(write_still=True)
    if '--video' not in sys.argv:
        continue
    # Studio viewport render for motion review; lit Cycles stills accompany it.
    scene.render.engine='BLENDER_WORKBENCH'
    scene.display.shading.light='STUDIO'
    scene.display.shading.color_type='MATERIAL'
    scene.display.shading.show_shadows=True
    scene.display.shading.show_cavity=True
    scene.display.shading.cavity_type='BOTH'
    scene.display.shading.background_type='WORLD'
    scene.world.color=(.035,.075,.06)
    scene.render.resolution_x=960
    scene.render.resolution_y=640
    scene.render.resolution_percentage=75
    camera=scene.camera
    camera.data.ortho_scale=3.4 if name=='StrikerPenalty' else 3.8
    offset=Vector((6,8,3.1)) if name=='StrikerPenalty' else Vector((4,-8,3.0))
    for f in range(end+1):
        scene.frame_set(f)
        target=Vector(samples[f]['Hips'])+Vector((0,0,.25))
        camera.location=target+offset
        camera.rotation_euler=(target-camera.location).to_track_quat('-Z','Y').to_euler()
        camera.keyframe_insert('location',frame=f)
        camera.keyframe_insert('rotation_euler',frame=f)
    scene.frame_end=end-1
    scene.render.image_settings.media_type='VIDEO'
    scene.render.image_settings.file_format='FFMPEG'
    scene.render.ffmpeg.format='MPEG4'
    scene.render.ffmpeg.codec='H264'
    scene.render.ffmpeg.constant_rate_factor='MEDIUM'
    scene.render.filepath=str(OUT/(name+'-60fps.mp4'))
    bpy.ops.render.render(animation=True)
    movie=bpy.data.movieclips.load(scene.render.filepath)
    assert movie.frame_duration==end and movie.fps==60
    video_reports.append({'clip':name,'frames':movie.frame_duration,'fps':movie.fps,'size':list(movie.size)})
(OUT/'validation.json').write_text(json.dumps(reports,indent=2),encoding='utf-8')
if video_reports:
    (OUT/'video-validation.json').write_text(json.dumps(video_reports,indent=2),encoding='utf-8')
