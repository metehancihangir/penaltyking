# Faz 6 görsel üretim kaydı

Araç: yerleşik `image_gen.imagegen`. Faz 5 karakterleri vuruş ve merkez kurtarış karelerine referans oldu. Görseller `Assets/Sprites/Characters/ShooterActions.png`, `KeeperCenter.png`, `KeeperSide.png` olarak kaydedildi. Kaynak PNG'ler değiştirilmeden korunur; Unity atlas hücrelerini sprite rect ile dilimler.

## shooter

Referans: C:/Users/Metehan/Documents/ChatGPT/Penalty-King/Assets/Sprites/Characters/ShooterIdle.png

Create a production 2x2 sprite sheet, four equal 512x512 cells, 1024x1024 transparent PNG. Use exactly this red shirt, cream trim, white shorts, red socks, dark boots, dark hair footballer, viewed from behind towards the goal. Four distinct FULL BODY animation poses, one centered fully inside each cell, no overlap, no labels or grid lines. Top left: forward run-up with right leg drawn back. Top right: football kick CONTACT, left foot planted, right foot extended diagonally forward to upper right, striking an invisible ball. Bottom left: FOLLOW THROUGH, right kicking leg raised, arms balance, body leaning forward. Bottom right: RECOVERY standing on both feet. Consistent figure scale and design, crisp 16-bit pixel art. Absolutely NO balls, NO ground, NO shadows, NO glow, NO background: real alpha transparency everywhere outside figures.

## keeper-side

Referans: C:/Users/Metehan/Documents/ChatGPT/Penalty-King/Assets/Sprites/Characters/KeeperIdle.png

Create a production 2x2 sprite sheet of THIS gold shirt/navy shorts goalkeeper, four equal 512x512 cells on a 1024x1024 transparent PNG. One FULL BODY pose fully inside each cell; clear separation, no labels or grid. Same face outfit pixel style. All action dives to SCREEN RIGHT. Top left TAKEOFF: crouching and pushing off left foot towards screen right, both gloved hands reaching right. Top right AIRBORNE CATCH/PARRY: entire body almost HORIZONTAL in midair, arms fully extended to RIGHT, legs trailing LEFT; an authentic goalkeeper dive. Bottom left LANDING: lying on right side on an invisible ground, arms extending right, bent trailing legs. Bottom right RECOVERY: kneeling and pushing torso upright. No ball. No glow/shadow/background/floor. Genuinely alpha transparent cutouts, crisp 16-bit pixel sprites.

## keeper-center

Referans: C:/Users/Metehan/Documents/ChatGPT/Penalty-King/Assets/Sprites/Characters/KeeperIdle.png

Create a production 2x2 sprite sheet, four equal 512x512 cells on 1024x1024 transparent canvas. THIS gold jersey navy shorts gold socks goalkeeper FRONT VIEW full body consistent person/outfit/scale. One pose in each cell with generous margins, no labels/grid. Top left: crouch anticipation hands raised toward center. Top right: CENTRAL SAVE, jump slightly up with both gloves together in front of upper chest to catch an invisible ball, knees flexed. Bottom left: LANDING in low crouch with both gloves clasped at chest. Bottom right: RECOVERY rising to standing ready stance. All full body from hair to boots inside each cell. NO ball, NO ground, NO shadows or glow. Actual transparent alpha outside figures. Crisp 16-bit pixel game sprite sheet.

## pitch

Referans: C:/Users/Metehan/Documents/ChatGPT/Penalty-King/Assets/Sprites/Stadium/NightStadium.png

Edit ONLY the small white penalty spot on the grass near bottom center of this stadium image: remove the white spot and fill its tiny footprint with the same green grass. Preserve every other pixel and the exact 1536x1024 composition, all field lines, floodlights, stands, sky and grass. No other changes. This backdrop will have a separate movable football sprite overlaid by the game engine.

## Yan dalış son üretimi

İlk iki yan dalış denemesinde kareli arka plan oluştu; projeye alınmadı. Son üretim gerçek alpha içeren yeni bir atlas oldu. İstem: Transparent PNG sprite atlas for a 16-bit football game. Actual empty alpha channel, NEVER draw a checkerboard. Four cutout sprites in equal quadrants of a 2 by 2 atlas. Each stays completely within its quadrant with 15% padding. Athletic male goalkeeper dark brown hair gold long sleeve jersey with navy cuffs navy shorts gold socks white gloves dark boots, front three-quarter view. TOP LEFT crouching takeoff to the right. TOP RIGHT full horizontal flying dive to screen right, straight outstretched arms right and legs left. BOTTOM LEFT landing lying on right side with gloves reaching right. BOTTOM RIGHT kneeling recovery facing right. Same character in all four. Crisp pixel art. Blank transparent space between and around all four figures, NO checkerboard NO floor NO scenery NO backdrop NO shadow NO aura NO text.

Yatay atlasın oranı 1536×1024 olduğundan dilim sınırları kare varsayılmaz. Diğer iki atlas 1254×1254; her poz alpha sınırına göre alınır.
