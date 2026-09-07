"""Drive the development APK using Android input taps and collect app telemetry."""
from pathlib import Path
import json
import re
import subprocess
import time
import struct

ROOT = Path(__file__).resolve().parents[1]
ADB = str(ROOT / 'Builds/AndroidSdk/platform-tools/adb.exe')
PACKAGE = 'com.penaltyking.game'

def adb(*args):
    return subprocess.check_output([ADB, '-s', 'emulator-5554', *args], encoding='utf-8', errors='replace', timeout=35)

def logs():
    return adb('logcat', '-d', '-s', 'Unity:I', '*:S')

def target(fragment):
    text = logs()
    # Every target batch starts with a scene marker. Ignore coordinates from previous scenes.
    latest = text.rsplit('[MobileScene]', 1)[-1]
    for name, x, y in re.findall(r'\[MobileTarget\] ([^|\r\n]+)\|(\d+)\|(\d+)', latest):
        if fragment.casefold() in name.casefold():
            return int(x), int(y)
    return None

def tap(fragment):
    deadline = time.monotonic() + 20
    while time.monotonic() < deadline:
        point = target(fragment)
        if point:
            adb('shell', 'input', 'tap', str(point[0]), str(point[1]))
            time.sleep(1.2)
            print('Tapped:', fragment, flush=True)
            return
        time.sleep(.5)
    raise RuntimeError('Missing touch target: ' + fragment + '\n' + logs()[-3500:])

def screenshot(name):
    remote = '/sdcard/Download/penaltyking-phase8.png'
    adb('shell', 'screencap', '-p', remote)
    adb('pull', remote, str(ROOT / 'docs/previews' / (name + '.png')))
    data = (ROOT / 'docs/previews' / (name + '.png')).read_bytes()
    width, height = struct.unpack('>II', data[16:24])
    if 'landscape' in name:
        assert width > height, f'Expected landscape screenshot, got {width}x{height}'
    return width, height

if __name__ == '__main__':
    adb('logcat', '-c')
    adb('shell', 'wm', 'size', '720x1280')
    adb('emu', 'sensor', 'set', 'acceleration', '0:9.8:0')
    adb('shell', 'settings', 'put', 'system', 'accelerometer_rotation', '1')
    adb('shell', 'settings', 'put', 'system', 'user_rotation', '0')
    adb('shell', 'am', 'force-stop', PACKAGE)
    adb('shell', 'monkey', '-p', PACKAGE, '-c', 'android.intent.category.LAUNCHER', '1')
    time.sleep(5)
    print(logs()[-2500:], flush=True)
    # Follow the UI's actual development telemetry rather than hardcoded screen coordinates.
    tap('Singleplayer')
    tap('Orta')
    tap('Sabit Round')
    time.sleep(8)
    screenshot('phase8-android-portrait-gameplay')
    point = target('SOL Shot')
    assert point
    for index in range(5):
        adb('shell', 'input', 'tap', str(point[0]), str(point[1]))
        time.sleep(3.2)
        print('Fixed round shot', index + 1, flush=True)
    assert target('Tekrar'), 'Fixed round did not reach summary'
    screenshot('phase8-android-portrait-summary')
    tap('Tekrar')
    for index in range(5):
        adb('shell', 'input', 'tap', str(point[0]), str(point[1]))
        time.sleep(3.2)
    assert target('Ana Men'), 'Replay did not complete'
    tap('Ana Men')
    tap('Options')
    screenshot('phase8-android-options')
    tap('Geri')
    adb('emu', 'sensor', 'set', 'acceleration', '9.8:0:0')
    time.sleep(2)
    adb('shell', 'am', 'force-stop', PACKAGE)
    adb('shell', 'monkey', '-p', PACKAGE, '-c', 'android.intent.category.LAUNCHER', '1')
    time.sleep(5)
    tap('Singleplayer')
    tap('Zor')
    tap('Endless')
    time.sleep(8)
    screenshot('phase8-android-landscape-gameplay')
    point = target('ORTA Shot')
    assert point
    endless_shots = 0
    for index in range(20):
        adb('shell', 'input', 'tap', str(point[0]), str(point[1]))
        time.sleep(3.2)
        endless_shots += 1
        if target('Tekrar'):
            break
    assert target('Tekrar'), 'Endless did not reach a save in 20 attempts'
    screenshot('phase8-android-landscape-summary')
    tap('Ana Men')
    text = logs()
    (ROOT / 'Logs/phase8-android-logcat.txt').write_text(text, encoding='utf-8')
    performance = [dict(frames=int(n), fps=float(fps), p95Ms=float(p95), maxMs=float(worst)) for n, fps, p95, worst in
                   re.findall(r'\[MobilePerformance\] frames=(\d+) fps=([\d.]+) p95Ms=([\d.]+) maxMs=([\d.]+)', text)]
    touch = [float(value.replace(',', '.')) for value in re.findall(r'\[MobileShot\] queueMs=([\d.,]+)', text)]
    assert performance and len(touch) >= 11, 'Missing mobile measurements'
    assert 'NullReferenceException' not in text and 'FATAL EXCEPTION' not in text
    report = dict(device='Android 11 API 30 x86_64 emulator, WHPX, host GPU, 3072 MB RAM, 4 cores',
                  resolution='720x1280 portrait / 1280x720 landscape',
                  completed=['Fixed Round', 'Replay', 'Options and back', 'Endless', 'Home return'],
                  endlessShots=endless_shots, performanceWindows=performance, touchQueueMs=touch,
                  note='Android injected touch event to game handler; not physical finger-to-photon latency.')
    (ROOT / 'docs/phase8-mobile-results.json').write_text(json.dumps(report, indent=2), encoding='utf-8')
    print(json.dumps(report, indent=2), flush=True)
