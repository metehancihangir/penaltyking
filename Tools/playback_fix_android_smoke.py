"""Cold launch audio and portrait penalty regression checks using actual Android taps."""
from update7_android_smoke import tap, choose, shot, match
from android_smoke import adb, logs, screenshot, ROOT, PACKAGE
import time, re, json, socket, sys
from pathlib import Path
revision='--crowd-revision' in sys.argv
prefix='crowd-revision' if revision else 'playback-fix'

def portrait_sensor():
    # The adb emu proxy can return OK without forwarding authenticated commands.
    with socket.create_connection(('127.0.0.1',5554),5) as console:
        def response():
            data=b''
            while b'OK' not in data and b'KO:' not in data:
                chunk=console.recv(4096)
                if not chunk: raise RuntimeError('Emulator console closed')
                data+=chunk
            assert b'KO:' not in data,data.decode(errors='replace')
        response()
        token=(Path.home()/'.emulator_console_auth_token').read_text().strip()
        console.sendall(('auth '+token+'\n').encode());response()
        console.sendall(b'sensor set acceleration 0:9.8:0\n');response()

adb('shell','am','force-stop',PACKAGE)
adb('logcat','-c')
adb('shell','wm','size','720x1280')
adb('shell','settings','put','system','accelerometer_rotation','1')
portrait_sensor()
adb('shell','monkey','-p',PACKAGE,'-c','android.intent.category.LAUNCHER','1')
time.sleep(5)  # Deliberately no input before inspecting autoplay.
initial=logs()
assert re.search(r'\[MobileAudio\] cue=menu playing=True',initial),initial[-2000:]
choose('Endless')
if 'guide=0' in match():
    for _ in range(3): tap('Devam Button')
tap('Turn Handoff')
width,height=screenshot(prefix+'-android-portrait')
assert height>width
tap('SOL Shot Zone');tap('Turn Handoff');tap('SAĞ Shot Zone');time.sleep(1.6)
shot(False)
assert 'shots=2 p1=1 p2=0' in match(),match()
tap('Settings Button');time.sleep(2);tap('Continue Button')
tap('Menu Button')
text=logs()
samples=[dict(cue=cue,playing=playing,volume=float(volume),peak=float(peak)) for cue,playing,volume,peak in re.findall(r'\[MobileAudio\] cue=(\S+) playing=(\S+) volume=([\d.]+) outputPeak=([\d.]+)',text)]
for cue in ['menu','crowd','Kick','GoalCrowd' if revision else 'GoalVictory','SaveOff']:
    assert any(s['cue']==cue and s['peak']>0.00001 for s in samples),(cue,samples)
assert not any(e in text for e in ['NullReferenceException','FATAL EXCEPTION','AndroidJavaException'])
report={'resolution':[width,height],'coldMenuNoInput':True,'checks':['Goal and save via real taps','Portrait composition','Settings resume','Nonzero audio output for music, crowd, kick, goal, save'],'audioOutput':samples,'note':'Digital source output measured; physical speaker listening is not automated.'}
(ROOT/('docs/'+prefix+'-android-results.json')).write_text(json.dumps(report,indent=2),encoding='utf-8')
(ROOT/('Logs/'+prefix+'-android.log')).write_text(text,encoding='utf-8')
print(json.dumps(report,indent=2))
