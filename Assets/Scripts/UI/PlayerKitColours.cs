using UnityEngine;
using UnityEngine.UI;
namespace PenaltyKing
{
    public sealed class PlayerKitColours : MonoBehaviour
    {
        [SerializeField] private Image shooter,keeper;
        [SerializeField] private Material template;
        private Material shooterMaterial,keeperMaterial;
        private Sprite previousSprite, previousKeeperSprite;
        public int ShooterPlayer {get;private set;}=1;
        public void Configure(Image player,Image goalie,Material baseMaterial){shooter=player;keeper=goalie;template=baseMaterial;}
        public void SetShooterPlayer(int player)
        {
            ShooterPlayer=player;
            if(template==null)return;
            if(shooterMaterial==null)
            {
                shooterMaterial=new Material(template){hideFlags=HideFlags.HideAndDontSave};
                keeperMaterial=new Material(template){hideFlags=HideFlags.HideAndDontSave};
                shooter.material=shooterMaterial;keeper.material=keeperMaterial;
                keeperMaterial.SetFloat("_Keeper",1);
            }
            shooterMaterial.SetFloat("_Kit",player==2?1:0);
            keeperMaterial.SetFloat("_Kit",player==2?2:0);
            UpdateUv();
            var strikerRig=shooter.GetComponent<FootballRig>();var goalieRig=keeper.GetComponent<FootballRig>();
            if(strikerRig!=null){shooterMaterial.SetFloat("_Chroma",1);strikerRig.SetMaterial(shooterMaterial);}
            if(goalieRig!=null){keeperMaterial.SetFloat("_Chroma",1);goalieRig.SetMaterial(keeperMaterial);}
        }
        private void LateUpdate()=>UpdateUv();
        public void UpdateUv()
        {
            if(shooterMaterial==null)return;
            if(previousKeeperSprite!=keeper.sprite){previousKeeperSprite=keeper.sprite;keeperMaterial.SetFloat("_Chroma",keeper.sprite.texture.name=="KeeperSix"?1:0);}
            if(previousSprite==shooter.sprite)return;
            previousSprite=shooter.sprite;
            shooterMaterial.SetVector("_SpriteUV",UnityEngine.Sprites.DataUtility.GetOuterUV(shooter.sprite));
            shooterMaterial.SetFloat("_Chroma",shooter.sprite.texture.name=="ShooterPerformance" || shooter.sprite.texture.name=="StrikerAdult" || shooter.sprite.texture.name=="StrikerShot12"?1:0);
        }
        private void OnDestroy()
        {
            if(Application.isPlaying){Destroy(shooterMaterial);Destroy(keeperMaterial);}
            else {DestroyImmediate(shooterMaterial);DestroyImmediate(keeperMaterial);}
        }
    }
}
