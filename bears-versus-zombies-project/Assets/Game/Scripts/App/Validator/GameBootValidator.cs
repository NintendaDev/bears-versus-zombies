using SampleGame.App;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace SampleGame.Gameplay
{
    public class GameBootValidator : MonoBehaviour
    {
        [SerializeField, Required] private SceneContext _sceneContext;
        
        private void Awake()
        {
            BootstrapLoader bootstrapLoader = FindObjectOfType<BootstrapLoader>();

            if (bootstrapLoader != null)
            {
                _sceneContext.Run();
                
                return;
            }
            
            SceneManager.LoadScene(0, LoadSceneMode.Single);
        }
    }
}