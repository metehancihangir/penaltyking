"""Encode Unity-rendered frames, preserving their 30 fps timing in GIF units."""
from pathlib import Path
from PIL import Image
import shutil

root = Path(__file__).resolve().parents[1] / 'docs' / 'previews'
for kind, count in [('save', 80), ('goal', 132)]:
    paths = [root / f'bone-rig-{kind}-{i:03d}.png' for i in range(count)]
    frames = []
    for path in paths:
        with Image.open(path) as source:
            frames.append(source.convert('RGB').quantize(colors=192))
    durations = [30 if i % 3 != 2 else 40 for i in range(count)]
    durations[-1] += 700
    output = root / f'bone-rig-{kind}.gif'
    frames[0].save(output, save_all=True, append_images=frames[1:], duration=durations,
                   loop=0, optimize=False, disposal=1)
    with Image.open(output) as result:
        total = 0
        for i in range(result.n_frames):
            result.seek(i)
            total += result.info['duration']
        assert total == sum(durations)
        print(f'{output.name}: {result.n_frames} frames, {total} ms, {output.stat().st_size} bytes')
shutil.copyfile(root / 'bone-rig-save-021.png', root / 'bone-rig-contact.png')
