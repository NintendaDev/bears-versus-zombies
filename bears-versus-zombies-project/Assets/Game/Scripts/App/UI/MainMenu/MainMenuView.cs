using Modules.UI;
using R3;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SampleGame.App.UI
{
    public sealed class MainMenuView : AnimatedView
    {
        [SerializeField, Required] private Button _singlePlayerButton;
        [SerializeField, Required] private Button _lobbyButton;
        [SerializeField, Required] private TMP_InputField _eventNameInput;
        
        private readonly ReactiveCommand _singlePlayerClickedCommand = new();
        private readonly ReactiveCommand _lobbyClickedCommand = new();

        public Observable<Unit> SinglePlayerClicked => _singlePlayerClickedCommand.AsObservable();
        
        public Observable<Unit> LobbyClicked => _lobbyClickedCommand.AsObservable();

        public void Initialize()
        {
            _singlePlayerButton.OnClickAsObservable()
                .Subscribe(_singlePlayerClickedCommand.Execute)
                .AddTo(this);
            
            _lobbyButton.OnClickAsObservable()
                .Subscribe( _lobbyClickedCommand.Execute)
                .AddTo(this);
        }
    }
}