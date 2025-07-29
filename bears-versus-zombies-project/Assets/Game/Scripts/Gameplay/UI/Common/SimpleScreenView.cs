using UnityEngine;

namespace SampleGame.Gameplay.UI
{
    public class SimpleScreenView : MonoBehaviour
    {
        private GameObject _gameObject;

        public void Initialize()
        {
            _gameObject = gameObject;
        }
        
        public virtual void Show()
        {
            _gameObject.SetActive(true);
        }
        
        public virtual void Hide()
        {
            _gameObject.SetActive(false);
        }
    }
}