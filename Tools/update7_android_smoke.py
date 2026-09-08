"""Update 7: injected Android taps, real UI flow; no game state shortcuts."""
from android_smoke import adb,logs,target,screenshot,ROOT,PACKAGE
import time,json,re

def tap(name):
 end=time.monotonic()+20
 while time.monotonic()<end:
  text=logs().rsplit('[MobileScene]',1)[-1]
  points=[(n,int(x),int(y)) for n,x,y in re.findall(r'\[MobileTarget\] ([^|\r\n]+)\|(\d+)\|(\d+)',text)]
  matches=[p for p in points if p[0]==name]
  if matches:
   _,x,y=matches[0];adb('shell','input','tap',str(x),str(y));time.sleep(1.05);print('tap',name,flush=True);return
  time.sleep(.3)
 raise RuntimeError('Missing '+name+'\n'+logs()[-2000:])
def match():
 lines=re.findall(r'\[MobileMatch\] ([^\r\n]+)',logs());return lines[-1] if lines else ''
def choose(mode):
 tap('Play Button');tap('2 Kişilik Button');tap(mode+' Button')
def shot(goal=True):
 tap('Turn Handoff');tap('SOL Shot Zone');tap('Turn Handoff');tap('SAĞ Shot Zone' if goal else 'SOL Shot Zone');time.sleep(1.6)
def save_report():
 text=logs();(ROOT/'Logs/update7-android-logcat.txt').write_text(text,encoding='utf-8')
 perf=[dict(frames=int(n),fps=float(fps),p95Ms=float(p95),maxMs=float(mx)) for n,fps,p95,mx in re.findall(r'\[MobilePerformance\] frames=(\d+) fps=([\d.]+) p95Ms=([\d.]+) maxMs=([\d.]+)',text)]
 touches=[float(x.replace(',','.')) for x in re.findall(r'\[MobileShot\] queueMs=([\d.,]+)',text)]
 assert perf and touches
 assert not any(x in text for x in ['NullReferenceException','FATAL EXCEPTION','AndroidJavaException'])
 report={'device':'Android 11 API30 x86_64 emulator; host GPU; 1536 MB configured RAM','checks':['Fresh guide','Fixed 5 shots each, draw','Replay','In-game settings and resume','Menu options','Landscape rotation','Endless survives saves and >10 attempts','Guide stays completed after app restart'],'performanceWindows':perf,'touchQueueMs':touches,'physicalVibration':'Deferred by user; emulator is not physical verification'}
 (ROOT/'docs/update7-mobile-results.json').write_text(json.dumps(report,indent=2),encoding='utf-8');print(json.dumps(report,indent=2),flush=True)
if __name__=='__main__':
 adb('logcat','-c');adb('shell','wm','size','720x1280');adb('shell','settings','put','system','accelerometer_rotation','1');adb('emu','sensor','set','acceleration','0:9.8:0')
 adb('shell','monkey','-p',PACKAGE,'-c','android.intent.category.LAUNCHER','1');time.sleep(5)
 screenshot('update7-android-menu');choose('Sabit Round');screenshot('update7-android-guide')
 for i in range(3):tap('Devam Button')
 assert 'guide=-1' in match(),match()
 screenshot('update7-android-handoff');tap('Settings Button');screenshot('update7-android-settings');tap('Continue Button')
 for i in range(10):shot(True);print('Fixed',i+1,match(),flush=True)
 assert 'state=Finished shots=10 p1=5 p2=5' in match(),match()
 screenshot('update7-android-draw');tap('Tekrar Oyna Button');assert 'shots=0 p1=0 p2=0' in match(),match();shot(False);tap('Menu Button')
 tap('Options Button');screenshot('update7-android-options');tap('Geri Button')
 adb('emu','sensor','set','acceleration','9.8:0:0');time.sleep(3)
 # Force-stop/relaunch also verifies the guide completion survives process death.
 adb('shell','am','force-stop',PACKAGE);adb('shell','monkey','-p',PACKAGE,'-c','android.intent.category.LAUNCHER','1');time.sleep(4)
 choose('Endless');assert 'guide=-1' in match(),match()
 tap('Turn Handoff');screenshot('update7-android-landscape-gameplay');tap('SOL Shot Zone');tap('Turn Handoff');tap('SOL Shot Zone');time.sleep(1.6)
 for i in range(11):shot(i%2==0)
 assert 'state=PassingPhone shots=12' in match(),match()
 screenshot('update7-android-landscape-endless');tap('Menu Button');save_report()
