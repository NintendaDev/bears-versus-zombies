using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace SampleGame.Gameplay.UI
{
    public sealed class TextView : MonoBehaviour
    {
        [SerializeField, Required] private TMP_Text _label;
        
        private GameObject _gameObject;

        public void Initialize()
        {
            _gameObject = gameObject;
        }

        public void Show()
        {
            _gameObject.SetActive(true);
        }
        
        public void Hide()
        {
            _gameObject.SetActive(false);
        }
        
        public void SetText(string text)
        {
            _label.text = text;
        }
    }
}