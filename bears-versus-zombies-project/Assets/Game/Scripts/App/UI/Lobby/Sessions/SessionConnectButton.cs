using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SampleGame.App.UI
{
    public sealed class SessionConnectButton : MonoBehaviour
    {
        [SerializeField, Required] private Button _button;
        [SerializeField, Required] private TMP_Text _sessionNameLabel;
        
        private GameObject _gameObject;

        private string SessionName => _sessionNameLabel.text;
        
        public event Action<string> Clicked;
        
        public void Initialize()
        {
            _gameObject = gameObject;
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(OnClick);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnClick);
        }

        public void Enable(string sessionName)
        {
            _gameObject.SetActive(true);
            _sessionNameLabel.text = sessionName;
        }

        public void Disable()
        {
            _gameObject.SetActive(false);
        }

        private void OnClick()
        {
            Clicked?.Invoke(SessionName);
        }
    }
}