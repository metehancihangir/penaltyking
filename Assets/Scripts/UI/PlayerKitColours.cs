using UnityEngine;
using UnityEngine.UI;
namespace PenaltyKing
{
    public sealed class PlayerKitColours : MonoBehaviour
    {
        [SerializeField] private Image shooter,keeper;
        [SerializeField] private Material template;
        private Material shooterMaterial,keeperMaterial;
        private Sprite previousSprite;
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
        }
        private void LateUpdate()=>UpdateUv();
        public void UpdateUv()
        {
            if(shooterMaterial==null)return;
            if(previousSprite==shooter.sprite)return;
            previousSprite=shooter.sprite;
            shooterMaterial.SetVector("_SpriteUV",UnityEngine.Sprites.DataUtility.GetOuterUV(shooter.sprite));
            shooterMaterial.SetFloat("_Chroma",shooter.sprite.texture.name=="ShooterPerformance"?1:0);
        }
        private void OnDestroy()
        {
            if(Application.isPlaying){Destroy(shooterMaterial);Destroy(keeperMaterial);}
            else {DestroyImmediate(shooterMaterial);DestroyImmediate(keeperMaterial);}
        }
    }
}
