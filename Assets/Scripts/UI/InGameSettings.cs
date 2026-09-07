using UnityEngine;
namespace PenaltyKing
{
    public sealed class InGameSettings : MonoBehaviour
    {
        [SerializeField] private GameplayController game;
        [SerializeField] private PixelMenuButton open;
        [SerializeField] private GameObject shade;
        [SerializeField] private OptionsController options;
        public PixelMenuButton OpenButton => open;
        public OptionsController Options => options;
        public bool Visible => shade.activeSelf;
        public void Configure(GameplayController owner, PixelMenuButton button, GameObject panel, OptionsController controls)
        { game = owner; open = button; shade = panel; options = controls; }
        private void Awake() { open.onClick.AddListener(Open); options.Closed += Close; }
        private void Open()
        {
            if (SceneTransition.IsBusy || game.State == PlayState.Leaving || Visible) return;
            game.SetSettingsOpen(true);
            shade.SetActive(true);
        }
        private void Close()
        {
            options.Flush();
            shade.SetActive(false);
            game.SetSettingsOpen(false);
        }
        private void OnDestroy() { open.onClick.RemoveListener(Open); options.Closed -= Close; }
    }
}
