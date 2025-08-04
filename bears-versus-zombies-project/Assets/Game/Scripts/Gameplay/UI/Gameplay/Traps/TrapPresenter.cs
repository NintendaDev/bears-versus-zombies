using Fusion;
using SampleGame.Gameplay.GameContext;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SampleGame.Gameplay.UI
{
    public sealed class TrapPresenter : SimulationBehaviour, ISpawned, IDespawned
    {
        [SerializeField] private TrapType _type;
        [SerializeField] private TrapsSettings _settings;
        [SerializeField, Required] private TrapView _view;
        
        private TrapSpawner _trapSpawner;
        private IGameInitializeEvent _gameInitializeEvent;
        private PlayersService _playersService;

        void ISpawned.Spawned()
        {
            _gameInitializeEvent = GameContextService.Instance.Get<GameInitializeEventer>();
            _playersService = GameContextService.Instance.Get<PlayersService>();
            
            if (_settings.TryGetButtonKey(_type, out string buttonKey))
                _view.SetButtonKey(buttonKey.ToUpper());
            
            if (_settings.TryGetCost(_type, out int cost))
                _view.SetCost(cost.ToString());
            
            if (_gameInitializeEvent.IsInitialized)
                OnGameInitialized();
            else
                _gameInitializeEvent.Initialized += OnGameInitialized;
        }
        
        void IDespawned.Despawned(NetworkRunner runner, bool hasState)
        {
            _gameInitializeEvent.Initialized -= OnGameInitialized;
            Unsubscribe();
        }

        private void OnGameInitialized()
        {
            if (_trapSpawner != null)
                Unsubscribe();
            
            _trapSpawner = _playersService.GetLocalPlayer().GetComponent<TrapSpawner>();
            Subscribe();
        }

        private void Unsubscribe()
        {
            if (_trapSpawner == null)
                return;
            
            _trapSpawner.Success -= OnSpawnSuccess;
            _trapSpawner.Error -= OnSpawnError;
        }

        private void OnSpawnSuccess(TrapType trapType)
        {
            if (trapType != _type)
                return;
            
            _view.OnSpawnSuccess();
        }

        private void OnSpawnError(TrapType trapType)
        {
            if (trapType != _type)
                return;
            
            _view.OnSpawnError();
        }

        private void Subscribe()
        {
            if (_trapSpawner == null)
                return;
            
            _trapSpawner.Success += OnSpawnSuccess;
            _trapSpawner.Error += OnSpawnError;
        }
    }
}