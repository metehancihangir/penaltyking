"""Reproducible CC0 source edits. Run UpdateAudioSource.Decode in Unity first."""
from update_audio import ROOT,read,write,np
import json
out=ROOT/'Assets/Audio/Stadium'
metrics={}
def normalize(name,x,rate,rms,ceiling):
 x=x-x.mean(axis=0);gain=min(rms/np.sqrt(np.mean(x*x)),ceiling/np.max(np.abs(x)));x*=gain
 write(out/(name+'.wav'),x,rate)
 metrics[name]={'seconds':len(x)/rate,'peak':float(np.max(np.abs(x))),'rms':float(np.sqrt(np.mean(x*x))),'boundary_jump':float(np.max(np.abs(x[0]-x[-1])))}
 return x
x,r=read(ROOT/'docs/audio-source/chant-drums.wav');x=x[int(30*r):int(54.5*r)];overlap=int(.5*r);a=np.linspace(0,1,overlap)[:,None];loop=np.concatenate((x[overlap:-overlap],x[-overlap:]*(1-a)+x[:overlap]*a));loop=normalize('ChantDrumLoop',loop,r,.045,.19)
x,r=read(ROOT/'docs/audio-source/crowd-ooh.wav');x=x[int(6.25*r):int(7.43*r)].copy();t=np.arange(len(x))/r
# Short group 'ou' onset, falling body, then a restrained unvoiced 'ff' release.
envelope=np.minimum(1,t/.018)*np.clip((1.18-t)/.48,0,1);x*=envelope[:,None]
rng=np.random.default_rng(6006);noise=rng.normal(0,1,len(x));noise=noise-np.roll(noise,1);release=np.clip((t-.64)/.15,0,1)*np.clip((1.14-t)/.32,0,1)
x+=noise[:,None]*release[:,None]*.014
normalize('GoalOuff',x,r,.14,.65)
# Listening sample: two loop joins, kick+goal, then kick+save. Asset mix, not runtime capture.
rate=44100;demo=np.zeros((rate*52,2))
def add(name,start,gain=1):
 x,r=read(out/(name+'.wav'));pos=np.arange(0,len(x),r/rate);x=np.stack([np.interp(pos,np.arange(len(x)),x[:,min(c,x.shape[1]-1)]) for c in range(2)],axis=1);offset=int(start*rate);n=min(len(x),len(demo)-offset);demo[offset:offset+n]+=x[:n]*gain
for start in (0,24,48):add('ChantDrumLoop',start)
for start in (3,29):add('Kick',start+.24);add('GoalOuff',start+.9)
add('Kick',9.24);add('SaveReaction',9.9)
assert np.max(np.abs(demo))<1
write(ROOT/'docs/previews/update6-audio-demo.wav',demo,rate)
(ROOT/'docs/audio-source/update6-metrics.json').write_text(json.dumps(metrics,indent=2))
print(json.dumps(metrics,indent=2))
