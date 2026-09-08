"""Original percussion/chiptune composition plus documented CC0 crowd layers."""
from update_audio import ROOT,read,write,np
import json
rate=44100
rng=np.random.default_rng(807)
def resample(x,r):
 p=np.arange(0,len(x),r/rate);return np.stack([np.interp(p,np.arange(len(x)),x[:,min(c,x.shape[1]-1)]) for c in range(2)],axis=1)
def save(path,x,peak,rms):
 x-=x.mean(axis=0)
 if len(x)<2*rate:
  t=np.arange(len(x))/rate;x*=np.minimum(1,t/.005)[:,None]*np.minimum(1,(len(x)/rate-t)/.055)[:,None]
 x*=min(peak/np.max(np.abs(x)),rms/np.sqrt(np.mean(x*x)));write(path,x,rate)
 return x
out=ROOT/'Assets/Audio/Stadium'
x,r=read(out/'ChantDrumLoop.wav');x=resample(x,r)
# Quiet, distant voices; percussion is a separate original dominant layer.
f=np.fft.rfftfreq(len(x),1/rate);s=np.fft.rfft(x,axis=0);s*= (1/(1+(f/430)**4))[:,None];bed=np.fft.irfft(s,n=len(x),axis=0);bed*=.013/max(np.sqrt(np.mean(bed*bed)),1e-8)
loop=bed.copy()
def drum(freq,gain=1):
 t=np.arange(int(.65*rate))/rate;phase=2*np.pi*(freq*t+22*.035*(1-np.exp(-t/.035)));body=np.sin(phase)*np.exp(-t/ .14);skin=rng.normal(size=len(t))*np.exp(-t/.012)*.12;return (body+skin)*np.minimum(1,t/.002)*gain
for beat in range(36):
 for offset,gain,freq in [(0,.095,94),(.333,.038,145)]:
  d=drum(freq,gain);idx=(np.arange(len(d))+int((beat*2/3+offset)*rate))%len(loop);loop[idx]+=d[:,None]*np.array([1,.93])
loop=save(out/'CalmDrumLoop.wav',loop,.20,.044)
# Positive goal sting: original ascending major arpeggio, a net-like snap, soft sparkle.
def tone(midi,duration,kind='triangle'):
 t=np.arange(int(duration*rate))/rate;phase=(440*2**((midi-69)/12)*t)%1
 v=(4*np.abs(phase-.5)-1) if kind=='triangle' else np.tanh(np.sin(2*np.pi*phase)*3)
 return v*np.minimum(1,t/.006)*np.minimum(1,(duration-t)/.055)
def put(dst,v,at,gain):
 i=int(at*rate);n=min(len(v),len(dst)-i)
 if n>0:dst[i:i+n]+=v[:n,None]*gain
sting=np.zeros((int(1.18*rate),2))
for note,at in [(72,0),(76,.10),(79,.20),(84,.32)]:put(sting,tone(note,.42),at,.15)
for note in [60,64,67]:put(sting,tone(note,.73),.42,.04)
snap=rng.normal(size=int(.08*rate))*np.exp(-np.arange(int(.08*rate))/rate/.012);put(sting,snap,0,.04)
save(out/'GoalVictory.wav',sting,.62,.13)
# Quieter group disappointment, no old ahhh recording.
x,r=read(out/'GoalOuff.wav');x=resample(x,r);save(out/'SaveOff.wav',x,.30,.05)
# Original 16-bar 120 BPM menu loop, no borrowed melody.
menu=np.zeros((rate*32,2));chords=[(57,60,64),(53,57,60),(60,64,67),(55,59,62)]
melody=[0,2,1,2,3,2,1,0]
for bar in range(16):
 chord=chords[(bar//2)%4];start=bar*2
 for beat in range(4):put(menu,tone(chord[0]-12,.34),start+beat*.5,.050)
 for step,index in enumerate(melody):
  note=(chord+[chord[0]+12] if isinstance(chord,list) else list(chord)+[chord[0]+12])[index]+12
  if bar%4==3 and step in (3,7):continue
  put(menu,tone(note,.19,'soft-square'),start+step*.25,.023)
 for beat in range(4):put(menu,drum(78,.022),start+beat*.5,1)
 for note in chord:put(menu,tone(note,1.9),start,.012)
path=ROOT/'Assets/Resources/Audio';path.mkdir(exist_ok=True)
save(path/'MenuPixelTheme.wav',menu,.26,.060)
# Short audition at normal SFX level.
demo=np.tile(loop,(1,1))
for name,at in [('Kick',3.24),('GoalVictory',3.9),('Kick',9.24),('SaveOff',9.9)]:
 x,r=read(out/(name+'.wav'));x=resample(x,r);i=int(at*rate);demo[i:i+len(x)]+=x
write(ROOT/'docs/previews/pre7-audio.wav',demo,rate)
metrics={}
for name in ['CalmDrumLoop','GoalVictory','SaveOff']:
 x,r=read(out/(name+'.wav'));metrics[name]={'peak':float(np.max(np.abs(x))),'rms':float(np.sqrt(np.mean(x*x))),'length':len(x)/r,'seam':float(np.max(np.abs(x[0]-x[-1])))}
(ROOT/'docs/audio-source/pre7-metrics.json').write_text(json.dumps(metrics,indent=2));print(metrics)

