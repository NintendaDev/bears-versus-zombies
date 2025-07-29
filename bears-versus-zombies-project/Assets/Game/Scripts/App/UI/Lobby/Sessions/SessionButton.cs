using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SampleGame.App.UI
{
    public sealed class SessionButton : MonoBehaviour
    {
        [SerializeField, Required, ChildGameObjectsOnly]
        private Button _button;
        
        [SerializeField, Required, ChildGameObjectsOnly]
        private TMP_Text _sessionNameLabel;
        
        [SerializeField, Required, ChildGameObjectsOnly]
        private TMP_Text _playersCounterLabel;
        
        [SerializeField, Required, ChildGameObjectsOnly]
        private TMP_Text _regionLabel;
        
        public string SessionName => _sessionNameLabel.text;
        
        public event Action<string> Clicked;

        private void OnEnable()
        {
            _button.onClick.AddListener(OnClick);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnClick);
        }

        public void Initialize(string sessionName, string playersCount, string region)
        {
            _sessionNameLabel.text = sessionName;
            _playersCounterLabel.text = playersCount;
            _regionLabel.text = region;
        }

        private void OnClick()
        {
            Clicked?.Invoke(SessionName);
        }
    }
}