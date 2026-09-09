"""First ten seconds of the user-supplied four-channel ambiX WAV; no synthetic layers."""
from update_audio import ROOT,write,np
from pathlib import Path
import wave,json
source=Path(r'C:/Users/Metehan/Desktop/638367__usbmed_ambiences_sound_library__colombian_soccer_stadium_crowd_ambix.wav')
with wave.open(str(source),'rb') as w:
    rate=w.getframerate();channels=w.getnchannels();assert channels==4 and w.getsampwidth()==3
    raw=np.frombuffer(w.readframes(rate*10),dtype=np.uint8).reshape(-1,3)
values=raw[:,0].astype(np.int32)|(raw[:,1].astype(np.int32)<<8)|(raw[:,2].astype(np.int32)<<16)
values=(values^(1<<23))-(1<<23)
# Use the first omnidirectional channel as dual mono, rather than treating the
# directional ambisonic channels as ordinary left/right speaker feeds.
mono=values.reshape(-1,channels)[:,0].astype(np.float64)/(1<<23)
mono-=mono.mean()
# Equal-power overlap preserves the crowd bed across the wrap, without fading
# either edge to silence. All content comes from the first ten source seconds.
overlap=int(rate*.75);theta=np.linspace(0,np.pi/2,overlap)
mono=np.concatenate((mono[overlap:-overlap],mono[-overlap:]*np.cos(theta)+mono[:overlap]*np.sin(theta)))
mono*=min(.20/max(abs(mono)),.045/np.sqrt(np.mean(mono*mono)))
stereo=np.stack([mono,mono],axis=1)
write(ROOT/'Assets/Audio/Stadium/UserStadiumLoop.wav',stereo,rate)
(ROOT/'docs/audio-source/user-stadium-metrics.json').write_text(json.dumps({'source':str(source),'sourceChannels':4,'selectedChannel':0,'selectionSeconds':[0,10],'duration':len(mono)/rate,'equalPowerOverlapSeconds':.75,'boundaryJump':float(abs(mono[-1]-mono[0])),'peak':float(max(abs(mono))),'rms':float(np.sqrt(np.mean(mono*mono)))},indent=2),encoding='utf-8')
write(ROOT/'docs/previews/seamless-crowd-demo.wav',np.tile(stereo,(3,1)),rate)
