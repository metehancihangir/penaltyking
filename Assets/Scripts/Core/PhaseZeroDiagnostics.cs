using UnityEngine;
using UnityEngine.SceneManagement;

namespace PenaltyKing
{
    // Temporary development-only harness, not the Phase 1 game menu.
    public sealed class PhaseZeroDiagnostics : MonoBehaviour
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        private SceneNavigator navigator;
        private TouchInputProbe probe;

        private void Awake()
        {
            navigator = GetComponent<SceneNavigator>();
            probe = GetComponent<TouchInputProbe>();
        }

        private void OnGUI()
        {
            var scale = Mathf.Max(0.5f, Mathf.Min(Screen.width / 850f, Screen.height / 520f));
            var previousMatrix = GUI.matrix;
            GUI.matrix = Matrix4x4.Scale(new Vector3(scale, scale, 1f));
            GUILayout.BeginArea(new Rect(24, 24, 790, 470), GUI.skin.box);
            GUILayout.Label("PENALTY KING / PHASE 0 - infrastructure test");
            GUILayout.Label("Scene: " + SceneManager.GetActiveScene().name);
            GUILayout.Label("Temporary diagnostics. Game menus arrive in Phase 1.");
            GUILayout.Space(16);
            GUILayout.BeginHorizontal();
            foreach (GameScene scene in System.Enum.GetValues(typeof(GameScene)))
                if (GUILayout.Button(scene.ToString(), GUILayout.Height(48))) navigator.Navigate(scene);
            GUILayout.EndHorizontal();
            var state = GameManager.Instance;
            GUILayout.Space(16);
            GUILayout.Label($"Preserved state: {state.SelectedDifficulty} / {state.SelectedMode}");
            GUILayout.Label($"Music: {state.MusicVolume:F2} / SFX + crowd: {state.SfxVolume:F2}");
            if (GUILayout.Button("Set test state: Hard / Endless / Music 0.25 / SFX 0.65", GUILayout.Height(48)))
            {
                state.SelectDifficulty(Difficulty.Hard);
                state.SelectMode(GameMode.Endless);
                state.SetMusicVolume(0.25f);
                state.SetSfxVolume(0.65f);
            }
            GUILayout.Space(16);
            GUILayout.Label($"Input presses: {probe.PressCount} / Last position: {probe.LastPosition}");
            GUILayout.Label("Tap/click, then switch scenes to check state preservation.");
            GUILayout.EndArea();
            GUI.matrix = previousMatrix;
        }
#endif
    }
}
