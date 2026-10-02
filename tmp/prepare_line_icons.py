from pathlib import Path
from PIL import Image

source = Path(r"C:\Users\DTI\.codex\generated_images\01a0f8ed-8d85-7080-93f0-5e1df0268015")
dest = Path(r"C:\Users\DTI\OneDrive\Documentos\GitHub\tecaverso-vr-labs\assets\icons\tecaverso-line")
dest.mkdir(parents=True, exist_ok=True)

icons = {
    "physics": "exec-40b32e7e-3ebc-416f-9355-c0ca4aa45935.png",
    "chemistry": "exec-23aa950f-0ca8-48ca-b209-76adc5e526c8.png",
    "biology": "exec-0d602ccd-8359-4297-b7e5-121d63e445de.png",
    "mathematics": "exec-ea5452b6-9193-4dfd-a608-7f33fc14f342.png",
    "projectile-motion": "exec-dd28e213-59fb-44d0-b231-86b841ccc09a.png",
    "electricity": "exec-7a742b65-3d70-4648-963a-64c76ad680ec.png",
    "optics": "exec-17165816-9842-46e6-bf23-7306a497cba8.png",
    "thermodynamics": "exec-4f55b19f-a3cb-410e-9630-cc5ca5f4267f.png",
    "waves": "exec-8b965930-ae2b-497b-884b-efefbcbdf763.png",
    "gravity": "exec-c7e31078-a4d0-4069-b0c5-0683bed3742b.png",
}

for name, filename in icons.items():
    image = Image.open(source / filename).convert("RGBA")
    alpha = image.getchannel("A").point(lambda value: 0 if value < 56 else value)
    white = Image.new("RGBA", image.size, (255, 255, 255, 0))
    white.putalpha(alpha)
    white.save(dest / f"{name}-white.png")
    blue = Image.new("RGBA", image.size, (40, 78, 160, 0))
    blue.putalpha(alpha)
    blue.save(dest / f"{name}-blue.png")
