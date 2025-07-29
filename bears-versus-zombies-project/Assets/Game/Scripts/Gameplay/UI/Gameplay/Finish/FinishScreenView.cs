using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace SampleGame.Gameplay.UI
{
    public sealed class FinishScreenView : MonoBehaviour
    {
        [SerializeField, Required] private TMP_Text _titleLabel;
        [SerializeField, Required] private TMP_Text _messageLabel;
        private GameObject _gameObject;

        public void Initialize()
        {
            _gameObject = gameObject;
        }

        public void Show(string title)
        {
            Show(title, string.Empty);
        }
        
        public void Show(string title, string message)
        {
            _titleLabel.text = title;
            _messageLabel.text = message;
            _gameObject.SetActive(true);
        }
        
        public void Hide()
        {
            _gameObject.SetActive(false);
        }
    }
}