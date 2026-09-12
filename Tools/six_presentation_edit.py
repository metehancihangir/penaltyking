exec(Path('tools/six_target_edit.py').read_text().split("edit('Assets/Scripts/Gameplay/PenaltyRound.cs'")[0]) if False else None
from pathlib import Path
def edit(path,old,new):
 p=Path(path);s=p.read_text();assert old in s,old[:80];p.write_text(s.replace(old,new),encoding='utf-8')
p='Assets/Scripts/Gameplay/ShotPresentation.cs'
edit(p,'private Vector2 contactFootPixels;','''private Vector2 contactFootPixels;
        [SerializeField] private Sprite[] shotFrames, keeperSix;
        [SerializeField] private Vector2 shotContactPixels;
        public void ConfigureSixAnimation(Sprite[] shots,Vector2 toe,Sprite[] saves)
        { shotFrames=shots;shotContactPixels=toe;keeperSix=saves;shooterIdle=shots[0];keeperIdle=saves[7]; }
        public Vector2 TargetPoint(ShotDirection direction)
        {
            if(net==null)return new Vector2((ShotTargets.Column(direction)-1)*spacing,targetY+(ShotTargets.IsHigh(direction)?65:0));
            var rect=((RectTransform)net.transform).rect;
            var point=new Vector2((ShotTargets.Column(direction)-1)*rect.width*.32f,rect.height*(ShotTargets.IsHigh(direction)?.23f:-.25f));
            return stage.InverseTransformPoint(net.transform.TransformPoint(point));
        }''')
edit(p,'public Vector2 ContactBootPosition => ShooterPosition + (contactFootPixels + shooter.sprite.pivot - shooter.sprite.rect.size * .5f) * (shooterHeight / performanceFrames[0].rect.height);','''public Vector2 ContactBootPosition => ShooterPosition + ((shotFrames!=null&&shotFrames.Length==12?shotContactPixels:contactFootPixels) + shooter.sprite.pivot - shooter.sprite.rect.size * .5f) * (shooterHeight / (shotFrames!=null&&shotFrames.Length==12?shotFrames[0]:performanceFrames[0]).rect.height);''')
edit(p,'var sign = (int)shot.KeeperDirection - 1;','var sign = ShotTargets.Column(shot.KeeperDirection) - 1;')
edit(p,'var target = new Vector2(((int)shot.PlayerDirection - 1) * spacing, targetY);','var target = TargetPoint(shot.PlayerDirection);')
edit(p,'var ballPosition = Trajectory(target, flight);','''if(keeperSix!=null&&keeperSix.Length==8)SampleSixKeeper(shot.KeeperDirection,time);
            var ballPosition = Trajectory(target, flight);''')
edit(p,'dust[i].enabled = kickAge >= 0 && kickAge < .24f;','dust[i].enabled = false; // Avoid detached debris at the striking foot.')
edit(p,'var scale = shooterHeight / 160;\n            var frame','''if(shotFrames!=null&&shotFrames.Length==12 && celebrationAge<0)
            { SampleShotPlayer(time);return; }
            var scale = shooterHeight / 160;
            var frame''')
edit(p,'var plantedFoot = ballRest - contactFootPixels * (shooterHeight / performanceFrames[0].rect.height);','var plantedFoot = ballRest - (shotFrames!=null&&shotFrames.Length==12 ? shotContactPixels*(shooterHeight/shotFrames[0].rect.height) : contactFootPixels * (shooterHeight / performanceFrames[0].rect.height));')
edit(p,'private static void Pose(Image image','''private void SampleShotPlayer(float time)
        {
            // Load, approach, plant, backswing, contact and follow-through have distinct drawings.
            var frame=time<ContactTime ? Mathf.Clamp(Mathf.FloorToInt(time/ContactTime*8),0,7)
                : time<ContactTime+.09f?8:time<ContactTime+.20f?9:time<ContactTime+.33f?10:11;
            if(time<=0)frame=0;
            var pixelScale=shooterHeight/shotFrames[0].rect.height;
            var foot=shooterRest-Vector2.up*shooterHeight*.5f;
            var planted=ballRest-shotContactPixels*pixelScale;
            var approach=Mathf.SmoothStep(0,1,Mathf.Clamp01((time-.16f)/(ContactTime-.16f)));
            foot=Vector2.Lerp(foot,planted,approach);
            var sprite=shotFrames[frame];
            Pose(shooter,sprite,sprite.rect.height*pixelScale,false,foot+(sprite.rect.size*.5f-sprite.pivot)*pixelScale,1);
        }

        private void SampleSixKeeper(ShotDirection direction,float time)
        {
            var column=ShotTargets.Column(direction)-1;var high=ShotTargets.IsHigh(direction);
            var scale=114*keeperSize/keeperSix[7].rect.height;
            var age=time-ContactTime;var flight=ImpactTime-ContactTime;
            if(age<.08f || time>Duration-.1f){Pose(keeper,keeperSix[7],keeperSix[7].rect.height*scale,false,keeperRest,1);return;}
            var dive=Mathf.SmoothStep(0,1,Mathf.Clamp01((age-.08f)/(flight-.08f)));
            var target=TargetPoint(direction);var sign=column==0?1:column;
            var frame=column==0?(high?2:3):(high?0:1);
            var sprite=keeperSix[frame];
            // Save-pose pivot is the glove contact point. Absolute pixel scale stays fixed.
            var offset=(sprite.rect.size*.5f-sprite.pivot)*scale;offset.x*=sign;
            var position=Vector2.Lerp(keeperRest,target+offset,dive);
            position.y+=Mathf.Sin(dive*Mathf.PI)*(high?13:4);
            var after=time-ImpactTime;
            if(after>.16f)
            {
                var groundY=keeperRest.y-57*keeperSize;
                var landing=Mathf.SmoothStep(0,1,Mathf.Clamp01((after-.16f)/.22f));
                frame=column==0?6:after<.5f?4:5;sprite=keeperSix[frame];
                var feet=new Vector2(target.x-column*42*keeperSize,groundY);
                var footOffset=(sprite.rect.size*.5f-sprite.pivot)*scale;footOffset.x*=sign;
                position=Vector2.Lerp(position,feet+footOffset,landing);
                var recover=Mathf.SmoothStep(0,1,Mathf.Clamp01((after-.65f)/.5f));
                if(recover>0){sprite=keeperSix[7];sign=1;position=Vector2.Lerp(feet+Vector2.up*57*keeperSize,keeperRest,recover);}
            }
            Pose(keeper,sprite,sprite.rect.height*scale,false,position,sign);
        }

        private static void Pose(Image image''')
