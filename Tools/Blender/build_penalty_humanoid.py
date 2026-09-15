"""Blender 5.2: editable 60 Hz penalty actions, skinned reference athletes and FBX.

Run: blender --background --python Tools/Blender/build_penalty_humanoid.py
Optional arguments after --: --preview (render selected contact sheets).
Metres, Z up, actor forward -Y, anatomical right +X. No external assets.
"""
import bpy
import math
import json
import sys
from pathlib import Path
from mathutils import Vector, Matrix, Quaternion

BASE = Path(__file__).resolve().parents[2]
SOURCE = BASE / 'ArtSource/Blender'
EXPORT = BASE / 'Assets/Animations/PenaltyHumanoid'
PREVIEW = BASE / 'docs/previews/blender-penalty'
for path in (SOURCE, EXPORT, PREVIEW):
    path.mkdir(parents=True, exist_ok=True)
V = Vector
FPS = 60
IK_HISTORY = {}
PALM_HISTORY = {}


def smooth(t):
    t = max(0, min(1, t))
    return t*t*(3-2*t)


def ramp(f, a, b):
    return smooth((f-a)/(b-a))


def pulse(f, a, b, c):
    return ramp(f, a, b)*(1-ramp(f, b, c))


def curve(f, keys):
    if f <= keys[0][0]:
        return V(keys[0][1])
    for (a, va), (b, vb) in zip(keys, keys[1:]):
        if f <= b:
            return V(va).lerp(V(vb), smooth((f-a)/(b-a)))
    return V(keys[-1][1])


def rot(axis, degrees):
    return Quaternion(V(axis), math.radians(degrees))


def root_curve(f, keys):
    """Monotone cubic Hermite path: continuous velocity through running steps."""
    if f <= keys[0][0]:
        return V(keys[0][1])
    if f >= keys[-1][0]:
        return V(keys[-1][1])
    def tangent(i, axis):
        if i == 0 or i == len(keys)-1:
            return 0
        h0 = keys[i][0]-keys[i-1][0]
        h1 = keys[i+1][0]-keys[i][0]
        a = (keys[i][1][axis]-keys[i-1][1][axis])/h0
        b = (keys[i+1][1][axis]-keys[i][1][axis])/h1
        if a*b <= 0:
            return 0
        w0, w1 = 2*h1+h0, h1+2*h0
        return (w0+w1)/(w0/a+w1/b)
    for i, ((a, va), (b, vb)) in enumerate(zip(keys, keys[1:])):
        if f <= b:
            t=(f-a)/(b-a)
            return V([(2*t**3-3*t*t+1)*va[j]+(t**3-2*t*t+t)*(b-a)*tangent(i,j)+(-2*t**3+3*t*t)*vb[j]+(t**3-t*t)*(b-a)*tangent(i+1,j) for j in range(3)])


def mat(name, color, roughness=.6):
    material = bpy.data.materials.new(name)
    material.diffuse_color = (*color, 1)
    material.use_nodes = True
    shader = material.node_tree.nodes.get('Principled BSDF')
    shader.inputs['Base Color'].default_value = (*color, 1)
    shader.inputs['Roughness'].default_value = roughness
    return material


def two_bone(start, target, pole, upper, lower, joint_floor=None, continuity_key=None):
    delta = target-start
    distance = max(.025, min(delta.length, upper+lower-.0005))
    direction = delta.normalized()
    target = start + direction*distance
    bend = pole-direction*pole.dot(direction)
    if bend.length < .001:
        bend = V((0, 1, 0)).cross(direction)
    bend.normalize()
    along = (upper*upper-lower*lower+distance*distance)/(2*distance)
    centre=start+direction*along
    radius=math.sqrt(max(0,upper*upper-along*along))
    previous=IK_HISTORY.get(continuity_key)
    if previous is not None:
        previous_bend=previous-centre
        previous_bend-=direction*previous_bend.dot(direction)
        if previous_bend.length>.001:
            previous_bend.normalize()
            angle=previous_bend.angle(bend)
            if angle>math.radians(12):
                # Parallel-transport the bend around the new IK circle. The
                # bounded angular change prevents pole singularities from
                # flipping an elbow/knee while preserving exact limb lengths.
                q=previous_bend.rotation_difference(bend)
                bend=Quaternion().slerp(q,math.radians(12)/angle)@previous_bend
    middle=centre+bend*radius
    if joint_floor is not None and middle.z<joint_floor and radius>.0001:
        # Rotate the knee around its exact IK circle until it clears the turf.
        # This preserves both bone lengths and the planted ankle position.
        up=V((0,0,1))-direction*direction.z
        if up.length>.001:
            up.normalize()
            side=direction.cross(up).normalized()
            alpha=max(-1,min(1,(joint_floor-centre.z)/(radius*up.z)))
            sign=1 if bend.dot(side)>=0 else -1
            bend=up*alpha+side*(sign*math.sqrt(max(0,1-alpha*alpha)))
            middle=centre+bend*radius
    if continuity_key is not None:
        IK_HISTORY[continuity_key]=middle.copy()
    return middle, target


def make_rig(name):
    data = bpy.data.armatures.new(name+'Skeleton')
    rig = bpy.data.objects.new(name, data)
    bpy.context.collection.objects.link(rig)
    bpy.context.view_layer.objects.active = rig
    rig.select_set(True)
    bpy.ops.object.mode_set(mode='EDIT')
    definitions = {}

    def bone(n, h, t, parent=None):
        b = data.edit_bones.new(n)
        b.head, b.tail = h, t
        if parent:
            b.parent = data.edit_bones[parent]
        definitions[n] = (V(h), V(t), parent)

    bone('Root', (0, 0, 0), (0, 0, .18))
    bone('Hips', (0, 0, 1.01), (0, 0, 1.15), 'Root')
    bone('Spine', (0, 0, 1.15), (0, 0, 1.32), 'Hips')
    bone('Chest', (0, 0, 1.32), (0, 0, 1.49), 'Spine')
    bone('UpperChest', (0, 0, 1.49), (0, 0, 1.56), 'Chest')
    bone('Neck', (0, 0, 1.56), (0, 0, 1.65), 'UpperChest')
    bone('Head', (0, 0, 1.65), (0, 0, 1.84), 'Neck')
    for side, s in [('Left', -1), ('Right', 1)]:
        bone(side+'UpperLeg', (s*.105, 0, 1.01), (s*.105, -.015, .55), 'Hips')
        bone(side+'LowerLeg', (s*.105, -.015, .55), (s*.105, 0, .10), side+'UpperLeg')
        bone(side+'Foot', (s*.105, 0, .10), (s*.105, -.17, .075), side+'LowerLeg')
        bone(side+'Toes', (s*.105, -.17, .075), (s*.105, -.245, .065), side+'Foot')
        bone(side+'Shoulder', (s*.07, 0, 1.51), (s*.23, 0, 1.51), 'UpperChest')
        bone(side+'UpperArm', (s*.23, 0, 1.51), (s*.54, 0, 1.51), side+'Shoulder')
        bone(side+'LowerArm', (s*.54, 0, 1.51), (s*.81, 0, 1.51), side+'UpperArm')
        bone(side+'Hand', (s*.81, 0, 1.51), (s*.90, 0, 1.51), side+'LowerArm')
        for digit, dy, length in [('Thumb', -.046, .055), ('Index', -.027, .079), ('Middle', -.004, .088), ('Ring', .019, .081), ('Little', .039, .065)]:
            head = V((s*.88, dy, 1.51))
            for segment, fraction in [('Proximal', .46), ('Intermediate', .31), ('Distal', .23)]:
                tail = head+V((s*length*fraction, -.018 if digit == 'Thumb' else 0, 0))
                parent = side+'Hand' if segment == 'Proximal' else side+digit+('Proximal' if segment == 'Intermediate' else 'Intermediate')
                bone(side+digit+segment, head, tail, parent)
                head = tail
    bpy.ops.object.mode_set(mode='OBJECT')
    rig.show_in_front = True
    data.display_type = 'STICK'
    for pb in rig.pose.bones:
        pb.rotation_mode = 'QUATERNION'
    return rig, definitions


def athlete(rig, definitions, goalkeeper):
    """Portable skinned reference mesh, named material slots and articulated fingers."""
    skin = mat('Skin', (.43, .23, .13))
    shirt = mat('Keeper amber' if goalkeeper else 'Striker red', (.96, .54, .035) if goalkeeper else (.64, .025, .04))
    shorts = mat('Navy shorts' if goalkeeper else 'Ivory shorts', (.025, .045, .09) if goalkeeper else (.86, .87, .81))
    boots = mat('Boot leather', (.016, .021, .031), .33)
    trim = mat('Ivory trim', (.9, .91, .85))
    gloves = mat('Glove latex', (.83, .87, .79), .4) if goalkeeper else skin
    hair = mat('Hair', (.025, .014, .009))
    meshes = []

    def ellipsoid(name, position, scale, material, bone, direction=None):
        bpy.ops.mesh.primitive_uv_sphere_add(segments=16, ring_count=10, location=position)
        ob = bpy.context.object
        ob.name = name
        ob.scale = scale
        if direction is not None:
            ob.rotation_mode = 'QUATERNION'
            ob.rotation_quaternion = V((0, 0, 1)).rotation_difference(direction.normalized())
        bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
        ob.data.materials.append(material)
        for poly in ob.data.polygons:
            poly.use_smooth = True
        group = ob.vertex_groups.new(name=bone)
        group.add(list(range(len(ob.data.vertices))), 1, 'REPLACE')
        meshes.append(ob)
        return ob

    ellipsoid('Pelvis shorts', (0, 0, 1.01), (.19, .125, .16), shorts, 'Hips')
    ellipsoid('Lower jersey', (0, 0, 1.2), (.17, .115, .19), shirt, 'Spine')
    ellipsoid('Chest jersey', (0, 0, 1.40), (.215, .13, .19), shirt, 'Chest')
    ellipsoid('Collar', (0, 0, 1.555), (.075, .076, .027), trim, 'UpperChest')
    ellipsoid('Neck', (0, 0, 1.59), (.065, .065, .085), skin, 'Neck')
    ellipsoid('Face', (0, -.014, 1.735), (.091, .092, .125), skin, 'Head')
    ellipsoid('Hair cap', (0, .008, 1.806), (.094, .088, .067), hair, 'Head')
    ellipsoid('Nose', (0, -.105, 1.738), (.019, .028, .023), skin, 'Head')
    for s in [-1, 1]:
        ellipsoid('Ear', (s*.092, -.008, 1.736), (.019, .026, .033), skin, 'Head')
        ellipsoid('Eye', (s*.033, -.096, 1.766), (.018, .009, .008), trim, 'Head')
        ellipsoid('Pupil', (s*.032, -.104, 1.765), (.007, .004, .006), boots, 'Head')
    for side, s in [('Left', -1), ('Right', 1)]:
        for suffix, width, depth, material in [('UpperLeg', .089, .088, skin), ('LowerLeg', .061, .057, shirt), ('UpperArm', .062, .06, shirt), ('LowerArm', .045, .044, shirt if goalkeeper else skin)]:
            h, t, _ = definitions[side+suffix]
            ellipsoid(side+suffix+' mesh', (h+t)*.5, (width, depth, (t-h).length*.59), material, side+suffix, t-h)
        h, t, _ = definitions[side+'UpperLeg']
        ellipsoid(side+' shorts leg', h.lerp(t, .19), (.102, .102, .16), shorts, side+'UpperLeg', t-h)
        ellipsoid(side+' boot', (s*.105, -.088, .073), (.063, .17, .066), boots, side+'Foot')
        ellipsoid(side+' sole', (s*.105, -.085, .025), (.065, .165, .017), trim, side+'Foot')
        ellipsoid(side+' palm', (s*.855, 0, 1.51), (.064, .047, .022 if not goalkeeper else .028), gloves, side+'Hand')
        for digit in ['Thumb', 'Index', 'Middle', 'Ring', 'Little']:
            for segment in ['Proximal', 'Intermediate', 'Distal']:
                n = side+digit+segment
                h, t, _ = definitions[n]
                ellipsoid(n+' mesh', (h+t)/2, (.009 if not goalkeeper else .012, .009 if not goalkeeper else .012, (t-h).length*.62), gloves, n, t-h)
    bpy.ops.object.select_all(action='DESELECT')
    for mesh in meshes:
        mesh.select_set(True)
    bpy.context.view_layer.objects.active = meshes[0]
    bpy.ops.object.join()
    mesh = bpy.context.object
    mesh.name = rig.name+'ReferenceAthlete'
    # Joining objects preserves all bone groups. Apply world transform before binding.
    bpy.ops.object.transform_apply(location=True, rotation=True, scale=True)
    mesh.parent = rig
    modifier = mesh.modifiers.new('Humanoid skin', 'ARMATURE')
    modifier.object = rig
    return mesh


def pose(rig, defs, f, root, hips, body_q, feet, hands, foot_q=None, chest_twist=0, head_down=0, fist=0, wrist=0, head_yaw=0):
    """Solve all limbs in rig coordinates, then write local quaternion animation."""
    desired = {}
    rotations = {}

    def orient(n, h, t, twist=0):
        rest = rig.data.bones[n].matrix_local.to_quaternion()
        swing = (defs[n][1]-defs[n][0]).normalized().rotation_difference((t-h).normalized())
        q = rot(t-h, twist) @ swing @ rest
        m = q.to_matrix().to_4x4()
        m.translation = h
        desired[n] = m
        rotations[n] = swing

    orient('Root', root, root+V((0, 0, .18)))
    spine = hips+body_q@V((0, 0, .14))
    chest = spine+body_q@V((0, 0, .17))
    upper_q = body_q @ rot((0, 0, 1), chest_twist)
    upper = chest+upper_q@V((0, 0, .17))
    neck = upper+upper_q@V((0, 0, .07))
    head = neck+upper_q@V((0, 0, .09))
    for n, h, t in [('Hips', hips, spine), ('Spine', spine, chest), ('Chest', chest, upper), ('UpperChest', upper, neck), ('Neck', neck, head)]:
        orient(n, h, t)
        # Keep axial rotation, which a direction-only solve cannot express.
        q = (body_q if n in ['Hips', 'Spine'] else upper_q) @ rig.data.bones[n].matrix_local.to_quaternion()
        desired[n] = q.to_matrix().to_4x4()
        desired[n].translation = h
    head_q = upper_q @ rot((0, 0, 1), head_yaw) @ rot((1, 0, 0), head_down)
    orient('Head', head, head+head_q@V((0, 0, .19)))
    desired['Head'] = (head_q @ rig.data.bones['Head'].matrix_local.to_quaternion()).to_matrix().to_4x4()
    desired['Head'].translation = head
    for i, (side, s) in enumerate([('Left', -1), ('Right', 1)]):
        hip = hips+body_q@V((s*.105, 0, 0))
        knee_pole=body_q@V((0,-1,.1))
        if rig.name=='KeeperDiveRight':
            # As the torso rolls, knees continue to face the turf/ready direction.
            # A body-space pole would cross the IK plane and flip the knee.
            knee_pole=knee_pole.lerp(V((0,-1,.1)),ramp(f,110,135))
        knee, ankle = two_bone(hip, feet[i], knee_pole, .46024, .45025, .115, rig.name+side+'Leg')
        orient(side+'UpperLeg', hip, knee)
        orient(side+'LowerLeg', knee, ankle)
        fq = foot_q[i] if foot_q else Quaternion()
        toe = ankle+fq@V((0, -.17, -.025))
        orient(side+'Foot', ankle, toe)
        orient(side+'Toes', toe, toe+fq@V((0, -.075, -.01)))
        shoulder_inner = upper+upper_q@V((s*.07, 0, .02))
        shoulder = upper+upper_q@V((s*.23, 0, .02))
        orient(side+'Shoulder', shoulder_inner, shoulder)
        arm_pole=upper_q@V((s*.8,.45,-.65))
        if rig.name=='KeeperDiveRight':
            arm_pole=arm_pole.lerp(V((s*.45,-.7,1)),ramp(f,95,110))
        elbow, hand = two_bone(shoulder, hands[i], arm_pole, .31, .27, .085, rig.name+side+'Arm')
        orient(side+'UpperArm', shoulder, elbow)
        orient(side+'LowerArm', elbow, hand)
        direction = (hand-elbow).normalized()
        palm_q = V((s, 0, 0)).rotation_difference(direction)
        if hand.z<.24 and hips.z<.7:
            horizontal=V((direction.x,direction.y,0))
            if horizontal.length<.001:
                horizontal=V((0,-1,0))
            flat=V((s,0,0)).rotation_difference(horizontal.normalized())
            palm_q=palm_q.slerp(flat,min(1,(.24-hand.z)/.10))
        if side == 'Right':
            palm_q = rot((1, 0, 0), wrist) @ palm_q
        palm_key=rig.name+side
        old_palm=PALM_HISTORY.get(palm_key)
        if old_palm is not None:
            if old_palm.dot(palm_q)<0:
                palm_q.negate()
            angle=old_palm.rotation_difference(palm_q).angle
            if angle>math.radians(20):
                palm_q=old_palm.slerp(palm_q,math.radians(20)/angle)
        PALM_HISTORY[palm_key]=palm_q.copy()
        desired[side+'Hand']=(palm_q@rig.data.bones[side+'Hand'].matrix_local.to_quaternion()).to_matrix().to_4x4()
        desired[side+'Hand'].translation=hand
        for digit in ['Thumb', 'Index', 'Middle', 'Ring', 'Little']:
            parent = side+'Hand'
            for segment, curl in [('Proximal', 65), ('Intermediate', 80), ('Distal', 50)]:
                n = side+digit+segment
                rest_local = rig.data.bones[parent].matrix_local.inverted() @ rig.data.bones[n].matrix_local
                bend = rot((1, 0, 0), fist*curl+3*math.sin(f*.1+i))
                desired[n] = desired[parent] @ rest_local @ bend.to_matrix().to_4x4()
                parent = n
    for pb in rig.pose.bones:
        n = pb.name
        parent = pb.parent
        if parent:
            rest_local = parent.bone.matrix_local.inverted() @ pb.bone.matrix_local
            basis = rest_local.inverted() @ desired[parent.name].inverted() @ desired[n]
        else:
            basis = pb.bone.matrix_local.inverted() @ desired[n]
        pb.location = basis.to_translation()
        q = basis.to_quaternion()
        if pb.rotation_quaternion.dot(q) < 0:
            q.negate()
        pb.rotation_quaternion = q
        pb.keyframe_insert('location', frame=f, group=n)
        pb.keyframe_insert('rotation_quaternion', frame=f, group=n)
    return {n: list(m.translation) for n, m in desired.items()}


def striker_frame(f):
    root = root_curve(f, [(0, (-.8, 5, 0)), (30, (-.8, 5, 0)), (52, (-.63, 3.90, 0)), (78, (-.40, 2.54, 0)), (94, (-.23, 1.53, 0)), (103, (-.20, 1.37, 0)), (125, (-.12, .29, 0)), (145, (-.10, .16, 0)), (163, (.02, -.19, 0)), (177, (.11, -.56, 0)), (202, (.16, -1.12, 0)), (240, (.16, -1.25, 0))])
    z = curve(f, [(0, (1.0,0,0)), (30,(.97,0,0)), (52,(.91,0,0)), (78,(.92,0,0)), (103,(.88,0,0)), (125,(.93,0,0)), (140,(.94,0,0)), (148,(1.02,0,0)), (159,(1.13,0,0)), (175,(.90,0,0)), (193,(.97,0,0)), (240,(1.01,0,0))]).x
    if f < 31:
        z += .006*math.sin(f*.24)
    left = curve(f, [(0,(-.97,5.05,.10)), (34,(-.97,5.05,.10)), (47,(-.88,4.63,.26)), (59,(-.65,3.68,.30)), (73,(-.52,2.73,.10)), (83,(-.52,2.73,.10)), (101,(-.37,1.70,.39)), (116,(-.27,.54,.23)), (128,(-.25,.08,.10)), (149,(-.25,.08,.10)), (162,(-.30,-.04,.39)), (181,(-.17,-.99,.10)), (195,(-.17,-.99,.10)), (220,(-.04,-1.36,.10)), (240,(-.04,-1.36,.10))])
    right = curve(f, [(0,(-.63,4.83,.10)), (30,(-.63,4.83,.10)), (40,(-.46,4.34,.30)), (50,(-.43,3.62,.10)), (59,(-.43,3.62,.10)), (77,(-.21,2.84,.41)), (94,(-.08,1.44,.10)), (105,(-.08,1.44,.10)), (118,(.09,1.01,.46)), (133,(.12,.70,.77)), (139,(.12,.40,.52)), (145,(0,.17,.135)), (150,(-.10,-.29,.35)), (159,(-.25,-.67,.91)), (173,(.18,-.67,.10)), (182,(.18,-.67,.10)), (192,(.31,-.98,.27)), (205,(.32,-1.36,.10)), (240,(.32,-1.36,.10))])
    lean = 15*pulse(f,30,65,132)+11*pulse(f,125,145,174)
    turn = -13*pulse(f,105,135,149)+10*pulse(f,140,158,183)+42*ramp(f,201,226)
    body = rot((0,0,1),turn) @ rot((1,0,0),lean)
    hips = root+V((0,0,z))
    run = pulse(f,30,55,128)
    swing = math.sin((f-31)*math.pi/24)*.23*run
    lh = hips+V((-.31,-.08+swing,.18))
    rh = hips+V((.32,-.08-swing,.19))
    balance = pulse(f,102,133,177)
    lh = lh.lerp(hips+V((-.64,-.05,.43)),balance)
    rh = rh.lerp(hips+V((.46,.21,.16)),balance)
    cheer = ramp(f,199,220)
    rh = rh.lerp(hips+V((.30,-.20,.78+.06*math.sin((f-220)*.24))),cheer)
    left_q = rot((1,0,0),-22*pulse(f,148,160,181))
    right_q = rot((1,0,0),-46*pulse(f,108,132,146)+20*pulse(f,146,158,174))
    return root,hips,body,[left,right],[lh,rh],[left_q,right_q],-turn*.45,8*pulse(f,100,143,177)+.8*math.sin(f*.13)*(1-ramp(f,30,50)),cheer,0


def keeper_frame(f):
    # Ballistic flight: take-off frame 67, first hip impact frame 126.
    if f < 67:
        root=curve(f,[(0,(0,0,0)),(40,(0,0,0)),(51,(.20,0,0)),(61,(.30,0,0)),(67,(.47,0,0))])
        z=curve(f,[(0,(.90,0,0)),(40,(.89,0,0)),(53,(.72,0,0)),(60,(.76,0,0)),(67,(1.02,0,0))]).x
        z+=.014*math.sin(f*.48)**2*(1-ramp(f,36,41))
    elif f<=126:
        t=(f-67)/60
        root=V((.47+4.6*t,-.18*t,0))
        launch=(.21-1.02+4.905*(59/60)**2)/(59/60)
        z=1.02+launch*t-4.905*t*t
    else:
        root=curve(f,[(126,(.47+4.6*59/60,-.177,0)),(143,(5.34,-.22,0)),(155,(5.4,-.23,0)),(180,(5.4,-.23,0))])
        z=curve(f,[(126,(.21,0,0)),(133,(.29,0,0)),(143,(.28,0,0)),(153,(.32,0,0)),(164,(.43,0,0)),(180,(.56,0,0))]).x
    hips=root+V((0,0,z))
    tilt=curve(f,[(0,(0,0,0)),(41,(0,0,0)),(61,(15,0,0)),(75,(40,0,0)),(95,(45,0,0)),(126,(76,0,0)),(133,(90,0,0)),(143,(90,0,0)),(159,(57,0,0)),(180,(12,0,0))]).x
    roll=180*pulse(f,128,150,174)
    body=rot((0,1,0),tilt) @ rot((0,0,1),roll) @ rot((1,0,0),10*(1-ramp(f,61,75))+38*ramp(f,146,164)*(1-ramp(f,164,180)))
    if f<67:
        left=curve(f,[(0,(-.30,.03,.10)),(48,(-.30,.03,.10)),(61,(-.30,.03,.10)),(67,(-.13,.02,.13))])
        right=curve(f,[(0,(.30,0,.10)),(40,(.30,0,.10)),(46,(.40,-.02,.16)),(51,(.57,0,.10)),(61,(.57,0,.10)),(67,(.77,0,.21))])
        hop=.012*max(0,math.sin(f*.48))**2*(1-ramp(f,34,40))
        left.z+=hop
        right.z+=hop
    else:
        trailing=pulse(f,67,90,125)
        left=hips+body@V((-.20,.18*trailing,-.78))
        right=hips+body@V((.20,-.12,-.78))
        left=V((-.13,.02,.13)).lerp(left,ramp(f,67,79))
        right=V((.77,0,.21)).lerp(right,ramp(f,67,79))
        recover=ramp(f,143,172)
        left=left.lerp(root+V((-.20,.49,.11)),recover)
        right=right.lerp(root+V((.20,-.38,.10)),recover)
        left.z=max(.09,left.z)
        right.z=max(.09,right.z)
    reach=ramp(f,51,84)*(1-ramp(f,100,118))
    lh=hips+body@V((-.30,-.31,.41))
    rh=hips+body@V((.31,-.30,.44))
    rh=rh.lerp(hips+body@V((.24,-.045,1.10)),reach)
    lh=lh.lerp(hips+body@V((-.24,-.19,.72)),reach*.70)
    tuck=pulse(f,110,128,153)
    rh=rh.lerp(hips+body@V((-.09,-.25,.37)),tuck)
    lh=lh.lerp(hips+body@V((-.17,-.26,.33)),tuck)
    push=pulse(f,146,158,175)
    rh=rh.lerp(root+V((.35,-.40,.08)),push)
    lh=lh.lerp(root+V((-.12,-.39,.08)),push)
    wrist_floor=.14-.06*ramp(f,145,153)
    rh.z=max(wrist_floor,rh.z)
    lh.z=max(wrist_floor,lh.z)
    wrist=30*pulse(f,87,91,101)-14*pulse(f,92,99,108)
    # Let the ankles articulate during the roll; rotating soles with the torso
    # would drive the toes underground as the athlete gets back onto one knee.
    sole_q=Quaternion().slerp(body,ramp(f,61,75)*(1-ramp(f,110,126)))
    return root,hips,body,[left,right],[lh,rh],[sole_q,sole_q],0,-6*ramp(f,160,180),.15*tuck,wrist,-70*ramp(f,163,180)


def stage(rig, keeper):
    turf=mat('Studio turf',(.025,.105,.074))
    bpy.ops.mesh.primitive_plane_add(size=200)
    bpy.context.object.name='Turf'
    bpy.context.object.data.materials.append(turf)
    white=mat('Pitch marking',(.75,.85,.77))
    for x in (-3.66,3.66):
        bpy.ops.mesh.primitive_cube_add(size=1,location=(x,-.30 if keeper else -7,.055))
        bpy.context.object.scale=(.055,5,.006)
        bpy.context.object.data.materials.append(white)
    # A fixed reference ball for the contact frame, intentionally excluded from FBX.
    bpy.ops.mesh.primitive_uv_sphere_add(segments=24, ring_count=12, radius=.11,location=(0,0,.11) if not keeper else (0,0,-2))
    ball=bpy.context.object
    ball.name='Contact reference ball (not exported)'
    ball.data.materials.append(white)
    dark=mat('Ball panels',(.02,.03,.04))
    ball.data.materials.append(dark)
    for poly in ball.data.polygons:
        poly.material_index=1 if poly.index%7<2 else 0
    if not keeper:
        for f,position in [(0,(0,0,.11)),(145,(0,0,.11)),(160,(.25,-4,1.15)),(180,(.65,-10,2.1)),(240,(.65,-10,2.1))]:
            ball.location=position
            ball.keyframe_insert('location',frame=f)
    world=bpy.context.scene.world
    world.use_nodes=True
    world.node_tree.nodes['Background'].inputs[0].default_value=(.18,.23,.30,1)
    world.node_tree.nodes['Background'].inputs[1].default_value=.45
    for name,pos,energy,size in [('Key',(-3,-4,7),1400,5),('Rim',(4,3,6),1800,4),('Fill',(1,-6,3),650,4)]:
        data=bpy.data.lights.new(name,'AREA')
        data.energy=energy
        data.shape='DISK'
        data.size=size
        ob=bpy.data.objects.new(name,data)
        bpy.context.collection.objects.link(ob)
        ob.location=pos
        ob.rotation_euler=(V((0,0,1))-ob.location).to_track_quat('-Z','Y').to_euler()
    camera=bpy.data.cameras.new('Review camera')
    ob=bpy.data.objects.new('Review camera',camera)
    bpy.context.collection.objects.link(ob)
    ob.location=(5,-8,4) if keeper else (7,8,4)
    look=V((2.6,0,1)) if keeper else V((-.1,1.7,.9))
    ob.rotation_euler=(look-ob.location).to_track_quat('-Z','Y').to_euler()
    camera.type='ORTHO'
    camera.ortho_scale=7.7 if keeper else 7.5
    bpy.context.scene.camera=ob
    scene=bpy.context.scene
    scene.render.engine='CYCLES'
    scene.cycles.samples=16
    scene.render.resolution_x=960
    scene.render.resolution_y=640
    scene.render.resolution_percentage=100
    scene.world.color=(.08,.08,.08)
    scene.view_settings.view_transform='AgX'


def build(keeper):
    IK_HISTORY.clear()
    PALM_HISTORY.clear()
    bpy.ops.wm.read_factory_settings(use_empty=False)
    bpy.ops.object.select_all(action='SELECT')
    bpy.ops.object.delete(use_global=False)
    name='KeeperDiveRight' if keeper else 'StrikerPenalty'
    end=180 if keeper else 240
    scene=bpy.context.scene
    scene.render.fps=FPS
    scene.render.fps_base=1
    scene.frame_start=0
    scene.frame_end=end
    rig,defs=make_rig(name)
    mesh=athlete(rig,defs,keeper)
    data=[]
    sample=keeper_frame if keeper else striker_frame
    for f in range(end+1):
        joints=pose(rig,defs,f,*sample(f))
        data.append({'frame':f,'joints':joints})
    action=rig.animation_data.action
    action.name=name+'_60fps'
    action.use_fake_user=True
    for layer in action.layers:
        for strip in layer.strips:
            for bag in strip.channelbags:
                for fc in bag.fcurves:
                    for key in fc.keyframe_points:
                        key.interpolation='LINEAR'
    markers=({'Ready':0,'React':41,'Takeoff':67,'Parry':91,'HipImpact':126,'Roll':150,'KneelingReady':180} if keeper else {'Prepare':0,'RunUp':31,'Stutter':94,'Plant':128,'BallContact':145,'FollowThrough':159,'RightLanding':173,'Celebrate':220,'Finish':240})
    for label,frame in markers.items():
        scene.timeline_markers.new(label,frame=frame)
    rig['description']='60 FPS, metre scale, anatomical right +X, forward -Y. Root translates on ground; hips carry vertical motion.'
    rig['contact_frame']=91 if keeper else 145
    rig['reference_mesh']='Original procedural reference athlete. Retarget action to production character with Unity Humanoid.'
    # Export only animation rig and weighted mesh. T-pose remains the bind/rest pose.
    scene.frame_set(0)
    bpy.ops.object.select_all(action='DESELECT')
    rig.select_set(True)
    mesh.select_set(True)
    bpy.context.view_layer.objects.active=rig
    bpy.ops.export_scene.fbx(filepath=str(EXPORT/(name+'.fbx')), use_selection=True, object_types={'ARMATURE','MESH'}, axis_forward='-Z',axis_up='Y',apply_unit_scale=True,apply_scale_options='FBX_SCALE_UNITS',add_leaf_bones=False,use_armature_deform_only=False,bake_anim=True,bake_anim_use_all_bones=True,bake_anim_use_nla_strips=False,bake_anim_use_all_actions=False,bake_anim_step=1,bake_anim_simplify_factor=0,mesh_smooth_type='FACE',path_mode='AUTO')
    stage(rig,keeper)
    scene.frame_set(91 if keeper else 145)
    bpy.ops.wm.save_as_mainfile(filepath=str(SOURCE/(name+'.blend')))
    (PREVIEW/(name+'-samples.json')).write_text(json.dumps(data,separators=(',',':')),encoding='utf-8')
    if '--preview' in sys.argv:
        frames=[0,53,75,91,110,126,150,180] if keeper else [0,52,94,128,145,159,173,220]
        for f in frames:
            scene.frame_set(f)
            scene.render.filepath=str(PREVIEW/(name+f'-{f:03d}.png'))
            bpy.ops.render.render(write_still=True)
    print('PENALTY_EXPORT_OK',name,end,len(rig.data.bones),len(mesh.data.vertices))


if __name__=='__main__':
    if '--keeper-only' not in sys.argv:
        build(False)
    if '--striker-only' not in sys.argv:
        build(True)
