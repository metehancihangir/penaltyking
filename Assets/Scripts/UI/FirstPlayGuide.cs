using UnityEngine;
using UnityEngine.UI;
namespace PenaltyKing
{
    public sealed class FirstPlayGuide : MonoBehaviour
    {
        public const string CompletedKey="PenaltyKing.Guide.Completed.v1";
        [SerializeField] private GameplayController game;
        [SerializeField] private GameObject panel, directionsDiagram, handoffDiagram;
        [SerializeField] private Text title, description, progress, nextLabel;
        [SerializeField] private PixelMenuButton next;
        public PixelMenuButton Next => next;
        public bool Visible => panel.activeSelf;
        public int Step { get; private set; }
        private int lastAdvanceFrame;
        public void Configure(GameplayController owner,GameObject shade,GameObject directions,GameObject handoff,Text heading,Text detail,Text page,Text buttonLabel,PixelMenuButton button)
        { game=owner;panel=shade;directionsDiagram=directions;handoffDiagram=handoff;title=heading;description=detail;progress=page;nextLabel=buttonLabel;next=button; }
        private void Awake()=>next.onClick.AddListener(Advance);
        private void Start()
        {
            if(game.Round!=null && PlayerPrefs.GetInt(CompletedKey,0)==0)
            { Step=0;lastAdvanceFrame=Time.frameCount;game.SetTutorialOpen(true);panel.SetActive(true);Refresh(); }
        }
        private void Advance()
        {
            if(!Visible || game.SettingsOpen || SceneTransition.IsBusy || lastAdvanceFrame==Time.frameCount)return;
            lastAdvanceFrame=Time.frameCount;
            if(Step<2){Step++;Refresh();return;}
            PlayerPrefs.SetInt(CompletedKey,1);PlayerPrefs.Save();
            panel.SetActive(false);game.SetTutorialOpen(false);
        }
        public void ShowPreview(int step)
        { Step=step;panel.SetActive(true);Refresh(); }
        private void Refresh()
        {
            progress.text=$"{Step+1} / 3";
            title.text=Step==0?"ŞUTUNU SEÇ":Step==1?"TELEFONU DEVRET":"KURTARIŞINI SEÇ";
            description.text=Step==0?"Kalede bir yöne dokun.":Step==1?"Şutun gizli kalır.\nTelefonu diğer oyuncuya ver.":"Kaleci de bir yön seçer.\nHer atıştan sonra roller değişir.";
            directionsDiagram.SetActive(Step!=1);handoffDiagram.SetActive(Step==1);
            nextLabel.text=Step==2?"Başlayalım":"Devam";
        }
        private void OnDestroy()
        { next.onClick.RemoveListener(Advance);if(game!=null)game.SetTutorialOpen(false); }
    }
}
