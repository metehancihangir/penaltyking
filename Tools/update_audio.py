from pathlib import Path
import wave,numpy as np
ROOT=Path(__file__).resolve().parents[1]
def read(path):
 with wave.open(str(path),'rb') as w:
  return np.frombuffer(w.readframes(w.getnframes()),dtype='<i2').astype(np.float64).reshape(-1,w.getnchannels())/32768,w.getframerate()
def write(path,x,rate):
 with wave.open(str(path),'wb') as w:
  w.setnchannels(x.shape[1]);w.setsampwidth(2);w.setframerate(rate);w.writeframes((np.clip(x,-1,1)*32767).astype('<i2').tobytes())
if __name__=='__main__':
 import sys
 name=sys.argv[1];start=float(sys.argv[2]);duration=float(sys.argv[3]);x,rate=read(ROOT/'docs/audio-source'/f'{name}.wav');x=x[int(start*rate):int((start+duration)*rate)].mean(axis=1);positions=np.arange(0,len(x),rate/16000);x=np.interp(positions,np.arange(len(x)),x)[:,None];write(ROOT/'docs/previews'/f'{name}-audition.wav',x,16000)
