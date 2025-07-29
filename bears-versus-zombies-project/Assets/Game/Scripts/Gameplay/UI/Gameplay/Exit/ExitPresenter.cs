using Fusion;
using Modules.LoadingCurtain;
using Modules.Services;
using SampleGame.App;
using SampleGame.App.SceneManagement;
using SampleGame.Gameplay.GameContext;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace SampleGame.Gameplay.UI
{
    public sealed class ExitPresenter : SceneSimulationBehaviour, ISpawned, IDespawned
    {
        [SerializeField, Required] private Button _button;
        
        private GameplayToMenuLoader _gameToMenuLoader;
        private ILoadingCurtain _loadingCurtain;
        private GameFacade _gameFacade;
        private bool _isDisableUnsubscribe;

        void ISpawned.Spawned()
        {
            _gameToMenuLoader = ServiceLocator.Instance.Get<GameplayToMenuLoader>();
            _gameFacade = ServiceLocator.Instance.Get<GameFacade>();
            
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

        private async void OnButtonClick() => await _gameToMenuLoader.StartLoadingAsync();
    }
}