using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SampleGame.App.UI
{
    public sealed class MainMenuView : AnimatedView
    {
        [SerializeField, Required] private Button _singlePlayerButton;
        [SerializeField, Required] private Button _lobbyButton;
        [SerializeField, Required] private TMP_InputField _eventNameInput;
        
        private IMainMenuPresenter _mainMenuPresenter;

        public event UnityAction SinglePlayerClicked
        {
            add => _singlePlayerButton.onClick.AddListener(value);
            remove => _singlePlayerButton.onClick.RemoveListener(value);
        }
        
        public event UnityAction LobbyClicked
        {
            add => _lobbyButton.onClick.AddListener(value);
            remove => _lobbyButton.onClick.RemoveListener(value);
        }

        public void Initialize(IMainMenuPresenter mainMenuPresenter)
        {
            _mainMenuPresenter = mainMenuPresenter;
        }

        public override void Show()
        {
            base.Show();
            
            _mainMenuPresenter.OnShow();
        }

        public override void Hide()
        {
            base.Hide();
            
            _mainMenuPresenter.OnHide();
        }
    }
}