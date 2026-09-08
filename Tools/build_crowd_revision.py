"""Use the real CC0 crowd recordings without the added synthesized percussion/sting."""
from update_audio import ROOT,read,write,np
import json
folder=ROOT/'Assets/Audio/Stadium'
metrics={}
for source,name,ceiling,rms in [('ChantDrumLoop','CrowdChant',.19,.045),('GoalCheer','GoalCrowd',.62,.13)]:
    x,rate=read(folder/(source+'.wav'))
    x*=min(ceiling/np.max(np.abs(x)),rms/np.sqrt(np.mean(x*x)))
    write(folder/(name+'.wav'),x,rate)
    metrics[name]={'source':source,'seconds':len(x)/rate,'peak':float(np.max(np.abs(x))),'rms':float(np.sqrt(np.mean(x*x)))}
(ROOT/'docs/audio-source/crowd-revision-metrics.json').write_text(json.dumps(metrics,indent=2),encoding='utf-8')
print(metrics)
