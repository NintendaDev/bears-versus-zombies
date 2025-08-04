using Fusion;
using SampleGame.App;
using SampleGame.App.SceneManagement;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SampleGame.Gameplay.UI
{
    public sealed class ExitPresenter : SimulationBehaviour, ISpawned, IDespawned
    {
        [SerializeField, Required] private Button _button;
        
        private GameFacade _gameFacade;
        private bool _isDisableUnsubscribe;
        private IGameplayTerminator _gameplayTerminator;

        [Inject]
        private void Construct(GameFacade gameFacade, IGameplayTerminator gameplayTerminator)
        {
            _gameFacade = gameFacade;
            _gameplayTerminator = gameplayTerminator;
        }

        void ISpawned.Spawned()
        {
            _gameFacade.GameClosed += OnGameClose;
            _button.onClick.AddListener(OnButtonClick);
        }

        void IDespawned.Despawned(NetworkRunner runner, bool hasState)
        {
            if (_isDisableUnsubscribe == false)
                _button.onClick.RemoveListener(OnButtonClick);
        }

        private void OnDestroy()
        {
            if (_isDisableUnsubscribe)
                _button.onClick.RemoveListener(OnButtonClick);
        }

        private void OnGameClose()
        {
            _isDisableUnsubscribe = true;
        }

        private async void OnButtonClick() => await _gameplayTerminator.TerminateAsync();
    }
}