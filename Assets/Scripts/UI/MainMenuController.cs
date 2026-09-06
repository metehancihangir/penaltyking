using UnityEngine;

namespace PenaltyKing
{
    [RequireComponent(typeof(SceneNavigator))]
    public sealed class MainMenuController : MonoBehaviour
    {
        [SerializeField] private PixelMenuButton singleplayer;
        [SerializeField] private PixelMenuButton multiplayer;
        [SerializeField] private PixelMenuButton options;
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
            multiplayer.interactable = false;
            singleplayer.onClick.AddListener(OpenSingleplayer);
            options.onClick.AddListener(OpenOptions);
        }

        private void OpenSingleplayer() => navigator.Navigate(GameScene.DifficultySelect);
        private void OpenOptions() => navigator.Navigate(GameScene.Options);

        private void OnDestroy()
        {
            singleplayer.onClick.RemoveListener(OpenSingleplayer);
            options.onClick.RemoveListener(OpenOptions);
        }
    }
}
