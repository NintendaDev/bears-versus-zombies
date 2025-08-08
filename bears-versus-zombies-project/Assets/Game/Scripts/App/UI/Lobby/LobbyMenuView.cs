using Modules.UI;
using R3;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SampleGame.App.UI
{
    public sealed class LobbyMenuView : AnimatedView
    {
        [SerializeField, Required] private Button _backButton;
        [SerializeField, Required] private SessionConnectButton _sessionConnectButton;
        [SerializeField, Required] private Button _createSessionButton;
        [SerializeField, Required] private TMP_InputField _createSessionInput;
        
        private readonly ReactiveCommand _backClickedCommand = new();
        private readonly ReactiveCommand<string> _createSessionClickedCommand = new();

        public Observable<Unit> BackClicked => _backClickedCommand.AsObservable();

        public Observable<string> CreateSessionClicked => _createSessionClickedCommand.AsObservable();
        
        public Observable<Unit> SessionConnectClicked => _sessionConnectButton.Clicked;

        public void Initialize()
        {
            _sessionConnectButton.Initialize();
            
            _backButton.OnClickAsObservable()
                .Subscribe( _backClickedCommand.Execute)
                .AddTo(this);
            
            _createSessionButton.OnClickAsObservable()
                .Subscribe((_) => _createSessionClickedCommand.Execute(_createSessionInput.text))
                .AddTo(this);
        }

        public void EnableSessionConnectButton(string sessionName)
        {
            _sessionConnectButton.Enable(sessionName);
        }
        
        public void DisableSessionConnectButton() => _sessionConnectButton.Disable();
    }
}