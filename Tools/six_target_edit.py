from pathlib import Path
def edit(path, old, new):
 p=Path(path);s=p.read_text(encoding='utf-8-sig');assert old in s,path+': '+old[:80];p.write_text(s.replace(old,new),encoding='utf-8')
edit('Assets/Scripts/Gameplay/PenaltyRound.cs','public enum ShotDirection { Left, Center, Right }','''// Existing serialized directions remain the three lower targets.
    public enum ShotDirection { Left, Center, Right, LeftHigh, CenterHigh, RightHigh }
    public static class ShotTargets
    {
        public static int Column(ShotDirection direction) => (int)direction % 3;
        public static bool IsHigh(ShotDirection direction) => (int)direction >= 3;
        public static string Label(ShotDirection direction) =>
            (Column(direction) == 0 ? "Sol" : Column(direction) == 1 ? "Orta" : "Sağ") + (IsHigh(direction) ? " Üst" : " Alt");
        public static Vector2Dummy Unused; // removed below
    }'''.replace('        public static Vector2Dummy Unused; // removed below\n',''))
p='Assets/Scripts/Gameplay/GameplayController.cs'
edit(p,'[SerializeField] private ShotZone left, center, right;','''[SerializeField] private ShotZone left, center, right;
        [SerializeField] private ShotZone leftHigh, centerHigh, rightHigh;
        public ShotZone[] Targets => new[] { left, center, right, leftHigh, centerHigh, rightHigh };
        public void ConfigureSixTargets(ShotZone[] zones)
        { left=zones[0];center=zones[1];right=zones[2];leftHigh=zones[3];centerHigh=zones[4];rightHigh=zones[5]; }''')
edit(p,'replay.onClick.AddListener(Restart);','''if(leftHigh!=null)leftHigh.onClick.AddListener(ShootLeftHigh);
            if(centerHigh!=null)centerHigh.onClick.AddListener(ShootCenterHigh);
            if(rightHigh!=null)rightHigh.onClick.AddListener(ShootRightHigh);
            replay.onClick.AddListener(Restart);''')
edit(p,'private void ShootLeft()','''private void ShootLeftHigh() => Shoot(ShotDirection.LeftHigh);
        private void ShootCenterHigh() => Shoot(ShotDirection.CenterHigh);
        private void ShootRightHigh() => Shoot(ShotDirection.RightHigh);
        private void ShootLeft()''')
edit(p,'private void EnableZones(bool enabled) { left.interactable = center.interactable = right.interactable = enabled; }','private void EnableZones(bool enabled) { foreach(var zone in Targets) if(zone!=null)zone.interactable=enabled; }')
edit(p,'((int)direction - 1) * targetSpacing','(ShotTargets.Column(direction) - 1) * targetSpacing')
edit(p,'direction == ShotDirection.Left ? "Sol" : direction == ShotDirection.Center ? "Orta" : "Sağ"','ShotTargets.Label(direction)')
edit(p,'replay.onClick.RemoveListener(Restart);','''if(leftHigh!=null)leftHigh.onClick.RemoveListener(ShootLeftHigh);
            if(centerHigh!=null)centerHigh.onClick.RemoveListener(ShootCenterHigh);
            if(rightHigh!=null)rightHigh.onClick.RemoveListener(ShootRightHigh);
            replay.onClick.RemoveListener(Restart);''')
edit(p,'Bir yöne dokun.','Altı bölgeden birine dokun.')
edit(p,'Sol, orta veya sağ.','Sol, orta, sağ · Üst veya alt.')
p='Assets/Scripts/UI/GoalNetRipple.cs'
edit(p,'var rect=image.GetPixelAdjustedRect();','''// Preserve Image's aspect-fitted drawing bounds. The RectTransform can be larger.
            if(vh.currentVertCount==0)return;
            var vertex=UIVertex.simpleVert;vh.PopulateUIVertex(ref vertex,0);
            var min=(Vector2)vertex.position;var max=min;
            for(var i=1;i<vh.currentVertCount;i++){vh.PopulateUIVertex(ref vertex,i);min=Vector2.Min(min,vertex.position);max=Vector2.Max(max,vertex.position);}
            var rect=Rect.MinMaxRect(min.x,min.y,max.x,max.y);''')
p='Assets/Scripts/UI/GameplayStageLayout.cs'
edit(p,'pitch.GetComponent<PixelPitch>()?.SetGoalScale(goal.localScale.x);','''pitch.GetComponent<PixelPitch>()?.SetGoalScale(goal.localScale.x);
            if(controller.Targets[3]!=null)
            {
                zones.anchoredPosition=goal.anchoredPosition;zones.localScale=goal.localScale;
                for(var i=0;i<6;i++)
                {
                    var rt=(RectTransform)controller.Targets[i].transform;
                    rt.anchoredPosition=new Vector2((i%3-1)*goal.rect.width*.32f,goal.rect.height*(i>=3?.23f:-.25f));
                    rt.sizeDelta=new Vector2(goal.rect.width*.30f,goal.rect.height*.43f);
                }
            }''')
p='Assets/Scripts/UI/FirstPlayGuide.cs'
edit(p,'Completed.v1','Completed.six.v2')
edit(p,'Kalede bir yöne dokun.','Altı hedeften birine dokun.\\nÜst veya alt köşeni seç.')
edit(p,'Kaleci de bir yön seçer.','Kaleci de altı bölgeden birini seçer.')
edit('Assets/Scripts/UI/GuidePractice.cs','Aynı yön = kurtarış!\\nFarklı yön = gol!','Aynı bölge = kurtarış!\\nFarklı bölge = gol!')
p='Assets/Scripts/UI/PlayerKitColours.cs'
edit(p,'private Sprite previousSprite;','private Sprite previousSprite, previousKeeperSprite;')
edit(p,'if(previousSprite==shooter.sprite)return;','''if(previousKeeperSprite!=keeper.sprite){previousKeeperSprite=keeper.sprite;keeperMaterial.SetFloat("_Chroma",keeper.sprite.texture.name=="KeeperSix"?1:0);}
            if(previousSprite==shooter.sprite)return;''')
edit(p,'shooter.sprite.texture.name=="StrikerAdult"?1:0','shooter.sprite.texture.name=="StrikerAdult" || shooter.sprite.texture.name=="StrikerShot12"?1:0')
