using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace PenaltyKing.Editor
{
    public static class GoalCelebrationPreview
    {
        [MenuItem("Penalty King/Preview Goal Celebration (no APK)")]
        public static void Capture()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/Gameplay.unity");
            var game = Object.FindFirstObjectByType<GameplayController>();
            var kits = game.GetComponent<PlayerKitColours>(); kits.SetShooterPlayer(1);
            MenuPreview.Render(1280,720,"ready","goal-celebration");
            var shot = new ShotResult(ShotDirection.Right, ShotDirection.Left);
            for (var i = 0; i <= 50; i++)
            {
                game.Presentation.Sample(shot, ShotPresentation.ImpactTime + i * .1f); kits.UpdateUv();
                MenuPreview.Render(1280,720,"frame-"+i.ToString("D2"),"goal-celebration");
            }
            game.Presentation.ResetPose();
            EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
        }
    }
}
