"""Encode Unity-rendered frames; does not edit generated artwork."""
from pathlib import Path
from PIL import Image

source=Path('.utmp/six-verification/docs/previews')
frames=[Image.open(path).convert('RGB') for path in sorted(source.glob('six-target-frame-*.png'))]
ready=Image.open(source/'six-target-ready.png').convert('RGB')
frames.append(ready)
frames[0].save('docs/previews/six-target.gif',save_all=True,append_images=frames[1:],duration=[50]*(len(frames)-1)+[700],loop=0,optimize=False)
