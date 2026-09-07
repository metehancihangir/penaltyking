using UnityEngine;

namespace PenaltyKing
{
    [RequireComponent(typeof(SceneNavigator))]
    public sealed class PlayerSelectController : MonoBehaviour
    {
        [SerializeField] private PixelMenuButton online, local, back;
        public PixelMenuButton Online => online;
        public PixelMenuButton Local => local;
        public PixelMenuButton Back => back;
        private SceneNavigator navigator;

        public void Configure(PixelMenuButton onlineButton, PixelMenuButton localButton, PixelMenuButton backButton)
        { online = onlineButton; local = localButton; back = backButton; }

        private void Awake()
        {
            navigator = GetComponent<SceneNavigator>();
            online.interactable = false;
            local.onClick.AddListener(ChooseLocal);
            back.onClick.AddListener(GoBack);
        }

        private void ChooseLocal()
        {
            if (navigator.IsLoading) return;
            GameManager.Instance.SelectLocalMultiplayer();
            navigator.Navigate(GameScene.ModeSelect);
        }

        private void GoBack()
        {
            if (navigator.IsLoading) return;
            GameManager.Instance.BeginSelection();
            navigator.Navigate(GameScene.MainMenu);
        }

        private void OnDestroy()
        {
            local.onClick.RemoveListener(ChooseLocal);
            back.onClick.RemoveListener(GoBack);
        }
    }
}
