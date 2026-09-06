using UnityEngine;

namespace PenaltyKing
{
    [RequireComponent(typeof(SceneNavigator))]
    public sealed class DifficultySelectController : MonoBehaviour
    {
        [SerializeField] private PixelMenuButton easy, medium, hard, back;
        public PixelMenuButton Easy => easy;
        public PixelMenuButton Medium => medium;
        public PixelMenuButton Hard => hard;
        public PixelMenuButton Back => back;
        private SceneNavigator navigator;

        public void Configure(PixelMenuButton easyButton, PixelMenuButton mediumButton, PixelMenuButton hardButton, PixelMenuButton backButton)
        { easy = easyButton; medium = mediumButton; hard = hardButton; back = backButton; }

        private void Awake()
        {
            navigator = GetComponent<SceneNavigator>();
            easy.onClick.AddListener(ChooseEasy);
            medium.onClick.AddListener(ChooseMedium);
            hard.onClick.AddListener(ChooseHard);
            back.onClick.AddListener(GoBack);
        }

        private void ChooseEasy() => Choose(Difficulty.Easy);
        private void ChooseMedium() => Choose(Difficulty.Medium);
        private void ChooseHard() => Choose(Difficulty.Hard);
        private void Choose(Difficulty difficulty)
        {
            if (navigator.IsLoading) return;
            GameManager.Instance.SelectDifficulty(difficulty);
            navigator.Navigate(GameScene.ModeSelect);
        }
        private void GoBack() { if (!navigator.IsLoading) navigator.Navigate(GameScene.MainMenu); }

        private void OnDestroy()
        {
            easy.onClick.RemoveListener(ChooseEasy);
            medium.onClick.RemoveListener(ChooseMedium);
            hard.onClick.RemoveListener(ChooseHard);
            back.onClick.RemoveListener(GoBack);
        }
    }
}
