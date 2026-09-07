using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PenaltyKing
{
    public enum GameScene { MainMenu, DifficultySelect, ModeSelect, Gameplay, Options, PlayerSelect }

    public sealed class SceneNavigator : MonoBehaviour
    {
        public bool IsLoading => SceneTransition.IsBusy;

        public void Navigate(GameScene destination)
        {
            if (!Enum.IsDefined(typeof(GameScene), destination))
                throw new ArgumentOutOfRangeException(nameof(destination));
            RuntimeBootstrap.EnsureServices();
            SceneTransition.Instance.Navigate(destination);
        }
    }
}
