using Fusion;
using R3;
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

        private readonly CompositeDisposable _disposables = new();
        private GameFacade _gameFacade;
        private bool _isDisableUnsubscribe;
        private IGameplayUnloader _gameplayUnloader;

        [Inject]
        private void Construct(GameFacade gameFacade, IGameplayUnloader gameplayUnloader)
        {
            _gameFacade = gameFacade;
            _gameplayUnloader = gameplayUnloader;
        }

        void ISpawned.Spawned()
        {
            _gameFacade.GameClosed
                .Subscribe((_) => OnGameClose())
                .AddTo(_disposables);
            
            _button.onClick.AddListener(OnButtonClick);
        }

        void IDespawned.Despawned(NetworkRunner runner, bool hasState)
        {
            _disposables.Clear();
            
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

        private async void OnButtonClick() => await _gameplayUnloader.UnloadAsync();
    }
}