using UnityEngine;
namespace PenaltyKing
{
    public sealed class ExitConfirmation : MonoBehaviour
    {
        [SerializeField] private GameplayController game;
        [SerializeField] private GameObject shade;
        [SerializeField] private PixelMenuButton cancel, confirm;
        private int shownFrame;
        public bool Visible => shade.activeSelf;
        public PixelMenuButton CancelButton => cancel;
        public PixelMenuButton ConfirmButton => confirm;
        public void Configure(GameplayController owner,GameObject panel,PixelMenuButton resume,PixelMenuButton leave)
        { game=owner;shade=panel;cancel=resume;confirm=leave; }
        private void Awake(){cancel.onClick.AddListener(Close);confirm.onClick.AddListener(Confirm);}
        public void Show()
        {
            shownFrame=Time.frameCount;game.SetSettingsOpen(true);shade.SetActive(true);
            Canvas.ForceUpdateCanvases();
            foreach(var panel in shade.GetComponentsInChildren<FitPanelToSafeArea>())panel.Fit();
            Canvas.ForceUpdateCanvases();
        }
        public void Close(){shade.SetActive(false);game.SetSettingsOpen(false);}
        private void Confirm(){if(!Visible || Time.frameCount<=shownFrame)return;Close();game.ReturnToMenuConfirmed();}
        private void OnDestroy(){cancel.onClick.RemoveListener(Close);confirm.onClick.RemoveListener(Confirm);}
    }
}
