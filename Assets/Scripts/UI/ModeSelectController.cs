using UnityEngine;
using UnityEngine.UI;

namespace PenaltyKing
{
    [RequireComponent(typeof(SceneNavigator))]
    public sealed class ModeSelectController : MonoBehaviour
    {
        [SerializeField] private PixelMenuButton fixedRound, endless, back;
        public PixelMenuButton FixedRound => fixedRound;
        public PixelMenuButton Endless => endless;
        public PixelMenuButton Back => back;
        private SceneNavigator navigator;

        public void Configure(PixelMenuButton fixedButton, PixelMenuButton endlessButton, PixelMenuButton backButton)
        { fixedRound = fixedButton; endless = endlessButton; back = backButton; }

        private void Awake()
        {
            navigator = GetComponent<SceneNavigator>();
            var state = GameManager.Instance;
            fixedRound.interactable = endless.interactable = state.IsLocalMultiplayer;
            fixedRound.onClick.AddListener(ChooseFixed);
            endless.onClick.AddListener(ChooseEndless);
            back.onClick.AddListener(GoBack);
        }

        private void ChooseFixed() => Choose(GameMode.FixedRound);
        private void ChooseEndless() => Choose(GameMode.Endless);
        private void Choose(GameMode mode)
        {
            if (navigator.IsLoading || !GameManager.Instance.IsLocalMultiplayer) return;
            GameManager.Instance.SelectMode(mode);
            navigator.Navigate(GameScene.Gameplay);
        }
        private void GoBack()
        {
            if (navigator.IsLoading) return;
            GameManager.Instance.BeginSelection();
            navigator.Navigate(GameScene.PlayerSelect);
        }

        private void OnDestroy()
        {
            fixedRound.onClick.RemoveListener(ChooseFixed);
            endless.onClick.RemoveListener(ChooseEndless);
            back.onClick.RemoveListener(GoBack);
        }
    }
}
