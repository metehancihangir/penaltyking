from pathlib import Path
p=Path('Assets/Scripts/Gameplay/ShotPresentation.cs');s=p.read_text(encoding='utf-8-sig')
s=s.replace('private GoalCelebration goalCelebration;','''private GoalCelebration goalCelebration;
        [SerializeField] private FootballRig strikerRig, goalieRig;
        public bool UsesBoneRigs => strikerRig!=null && goalieRig!=null;
        public Vector2 GripPosition => goalieRig!=null?goalieRig.GripPosition:KeeperPosition;
        public void ConfigureRigs(FootballRig player,FootballRig goalkeeper){strikerRig=player;goalieRig=goalkeeper;}
        private void SampleRigKeeper(ShotResult shot,float time)
        { RigMotion.Keeper(goalieRig,shot,time,keeperRest,TargetPoint(shot.KeeperDirection),keeperSize); }''')
s=s.replace('public Vector2 ContactBootPosition => ShooterPosition +','public Vector2 ContactBootPosition => strikerRig!=null ? strikerRig.ToePosition : ShooterPosition +')
s=s.replace('SamplePlayer(0, -1);','''SamplePlayer(0, -1);
            if(goalieRig!=null){SampleRigKeeper(new ShotResult(ShotDirection.Center,ShotDirection.Center),0);goalieRig.HoldBall(false,ball.sprite,Vector2.zero);}''')
s=s.replace('if(keeperSix!=null&&keeperSix.Length==8)SampleSixKeeper(shot.KeeperDirection,time);','''if(goalieRig!=null)SampleRigKeeper(shot,time);
            else if(keeperSix!=null&&keeperSix.Length==8)SampleSixKeeper(shot.KeeperDirection,time);''')
s=s.replace('ball.rectTransform.anchoredPosition = ballPosition;','''var held=goalieRig!=null && shot.Outcome==ShotOutcome.Save && time>=ImpactTime;
            if(held)ballPosition=goalieRig.GripPosition;
            ball.color=held?new Color(1,1,1,0):Color.white;
            ball.rectTransform.anchoredPosition = ballPosition;''')
s=s.replace('ball.rectTransform.localRotation = Quaternion.Euler(0, 0, spin);','''ball.rectTransform.localRotation = held?Quaternion.identity:Quaternion.Euler(0, 0, spin);
            goalieRig?.HoldBall(held,ball.sprite,ball.rectTransform.rect.size*1.2f*.55f);''')
s=s.replace('shadow.enabled = true;\n            shadow.rectTransform.anchoredPosition = new Vector2(ballPosition.x','shadow.enabled = !held;\n            shadow.rectTransform.anchoredPosition = new Vector2(ballPosition.x')
s=s.replace('private void SamplePlayer(float time, float celebrationAge)\n        {','''private void SamplePlayer(float time, float celebrationAge)
        {
            if(strikerRig!=null){RigMotion.Striker(strikerRig,time,celebrationAge,shooterRest,ballRest,shooterHeight);return;}''')
p.write_text(s,encoding='utf-8')
p=Path('Assets/Scripts/UI/PlayerKitColours.cs');s=p.read_text(encoding='utf-8-sig')
s=s.replace('UpdateUv();\n        }','''UpdateUv();
            var strikerRig=shooter.GetComponent<FootballRig>();var goalieRig=keeper.GetComponent<FootballRig>();
            if(strikerRig!=null){shooterMaterial.SetFloat("_Chroma",1);strikerRig.SetMaterial(shooterMaterial);}
            if(goalieRig!=null){keeperMaterial.SetFloat("_Chroma",1);goalieRig.SetMaterial(keeperMaterial);}
        }''',1)
p.write_text(s,encoding='utf-8')
