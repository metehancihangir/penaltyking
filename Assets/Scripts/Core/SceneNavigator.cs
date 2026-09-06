using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PenaltyKing
{
    public enum GameScene { MainMenu, DifficultySelect, ModeSelect, Gameplay, Options }

    public sealed class SceneNavigator : MonoBehaviour
    {
        public bool IsLoading { get; private set; }

        public void Navigate(GameScene destination)
        {
            if (!Enum.IsDefined(typeof(GameScene), destination))
                throw new ArgumentOutOfRangeException(nameof(destination));
            if (!IsLoading) StartCoroutine(Load(destination));
        }

        private IEnumerator Load(GameScene destination)
        {
            IsLoading = true;
            var operation = SceneManager.LoadSceneAsync(destination.ToString());
            if (operation != null) yield return operation;
            IsLoading = false;
        }
    }
}
