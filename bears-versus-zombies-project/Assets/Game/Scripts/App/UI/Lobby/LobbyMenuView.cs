using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SampleGame.App.UI
{
    public sealed class LobbyMenuView : AnimatedView
    {
        [SerializeField, Required] private Button _backButton;
        [SerializeField, Required] private SessionConnectButton _sessionConnectButton;
        [SerializeField, Required] private Button _createSessionButton;
        [SerializeField, Required] private TMP_InputField _createSessionInput;
        
        private ILobbyPresenter _presenter;
        
        public event UnityAction BackClicked
        {
            add => _backButton.onClick.AddListener(value);
            remove => _backButton.onClick.RemoveListener(value);
        }

        public event Action<string> CreateSessionClicked;
        
        public event Action<string> SessionConnectClicked
        {
            add => _sessionConnectButton.Clicked += value;
            remove => _sessionConnectButton.Clicked -= value;
        }

        private void OnEnable()
        {
            _createSessionButton.onClick.AddListener(OnCreateSessionClick);
        }

        private void OnDisable()
        {
            _createSessionButton.onClick.RemoveListener(OnCreateSessionClick);
        }

        public void Initialize(ILobbyPresenter presenter)
        {
            _presenter = presenter;
        }

        public override void Show()
        {
            base.Show();
            _presenter.OnShow();
        }

        public override void Hide()
        {
            base.Hide();
            _presenter.OnHide();
        }

        public void EnableSessionConnectButton(string sessionName)
        {
            _sessionConnectButton.Enable(sessionName);
        }
        
        public void DisableSessionConnectButton() => _sessionConnectButton.Disable();

        private void OnCreateSessionClick()
        {
            if (string.IsNullOrEmpty(_createSessionInput.text))
                return;
            
            CreateSessionClicked?.Invoke(_createSessionInput.text);
        }
    }
}