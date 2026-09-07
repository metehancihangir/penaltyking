using UnityEngine;

namespace PenaltyKing
{
    [RequireComponent(typeof(SceneNavigator))]
    public sealed class MainMenuController : MonoBehaviour
    {
        [SerializeField] private PixelMenuButton singleplayer;
        [SerializeField] private PixelMenuButton multiplayer;
        [SerializeField] private PixelMenuButton options;
        public PixelMenuButton Play => singleplayer;
        public PixelMenuButton Singleplayer => singleplayer;
        public PixelMenuButton Multiplayer => multiplayer;
        public PixelMenuButton Options => options;
        private SceneNavigator navigator;

        public void Configure(PixelMenuButton single, PixelMenuButton multi, PixelMenuButton settings)
        {
            singleplayer = single;
            multiplayer = multi;
            options = settings;
        }

        private void Awake()
        {
            navigator = GetComponent<SceneNavigator>();
            if (multiplayer != null) multiplayer.interactable = false;
            singleplayer.onClick.AddListener(OpenSingleplayer);
            options.onClick.AddListener(OpenOptions);
        }

        private void OpenSingleplayer()
        {
            if (navigator.IsLoading) return;
            GameManager.Instance.BeginSelection();
            navigator.Navigate(GameScene.PlayerSelect);
        }
        private void OpenOptions() => navigator.Navigate(GameScene.Options);

        private void OnDestroy()
        {
            singleplayer.onClick.RemoveListener(OpenSingleplayer);
            options.onClick.RemoveListener(OpenOptions);
        }
    }
}
