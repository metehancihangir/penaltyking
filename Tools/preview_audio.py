"""Assemble an asset audition at the game's cue times (not a runtime recording)."""
from array import array
from pathlib import Path
import math
import sys
import wave

ROOT = Path(__file__).resolve().parents[1]
RATE = 44100
mix = [0.0] * (RATE * 6 * 2)

def add(name, start, gain=.8):
    with wave.open(str(ROOT / 'Assets/Audio/Stadium' / (name + '.wav')), 'rb') as source:
        assert source.getsampwidth() == 2
        channels, rate = source.getnchannels(), source.getframerate()
        samples = array('h', source.readframes(source.getnframes()))
        if sys.byteorder != 'little':
            samples.byteswap()
    count = len(samples) // channels
    first = round(start * RATE)
    for frame in range(min(round(count * RATE / rate), len(mix) // 2 - first)):
        position = frame * rate / RATE
        index, fraction = int(position), position % 1
        for channel in range(2):
            c = min(channel, channels - 1)
            a = samples[index * channels + c]
            b = samples[min(index + 1, count - 1) * channels + c]
            mix[(first + frame) * 2 + channel] += (a + (b - a) * fraction) / 32768 * gain

add('CrowdLoop', 0)
add('Kick', .24)
add('GoalCheer', .9)
add('Kick', 3.24)
add('SaveReaction', 3.9)
for frame in range(len(mix) // 2):
    fade = min(1, frame / (RATE * .03), (len(mix) // 2 - 1 - frame) / (RATE * .25))
    mix[frame * 2] *= fade
    mix[frame * 2 + 1] *= fade
peak = max(map(abs, mix))
assert peak < 1, peak
output = array('h', (round(sample * 32767) for sample in mix))
if sys.byteorder != 'little':
    output.byteswap()
path = ROOT / 'docs/previews/phase7-audio-demo.wav'
with wave.open(str(path), 'wb') as target:
    target.setnchannels(2)
    target.setsampwidth(2)
    target.setframerate(RATE)
    target.writeframes(output.tobytes())
print(f'{path}: peak={peak:.3f}, 6 seconds, crowd + goal + save')
