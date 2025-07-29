using SampleGame.App;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SampleGame.Gameplay
{
    public class GameBootValidator : MonoBehaviour
    {
        private void Awake()
        {
            GameLoader gameLoader = FindObjectOfType<GameLoader>();

            if (gameLoader != null)
                return;
            
            SceneManager.LoadScene(0, LoadSceneMode.Single);
        }
    }
}