using System;
using Cysharp.Threading.Tasks;
using Fusion;
using Modules.AssetsManagement.AddressablesOperations;
using Modules.AssetsManagement.StaticData;
using UnityEngine;
using Zenject;

namespace SampleGame.App.UI
{
    public sealed class SessionButtonFactory : IDisposable
    {
        private readonly IInstantiator _instantiator;
        private readonly IAddressablesService _addressablesService;
        private readonly MainMenuAssets _assetsConfig;

        private SessionButton _prefab;
        private GameFacade _gameFacade;

        public SessionButtonFactory(IInstantiator instantiator, IStaticDataService staticDataService, 
            IAddressablesService addressablesService)
        {
            _instantiator = instantiator;
            _assetsConfig = staticDataService.GetConfiguration<MainMenuAssets>();
            _addressablesService = addressablesService;
        }

        public void Dispose()
        {
            _addressablesService.Release(_assetsConfig.SessionButtonReference);
        }

        public async UniTask InitializeAsync()
        {
            GameObject sessionButtonObject = await _addressablesService
                .LoadAsync<GameObject>(_assetsConfig.SessionButtonReference);
            
            _prefab = sessionButtonObject.GetComponent<SessionButton>();
        }

        public SessionButton Create(SessionInfo sessionInfo, Transform parent)
        {
            SessionButton button = _instantiator.InstantiatePrefab(_prefab, parent).GetComponent<SessionButton>();
            SessionButtonPresenter presenter = button.GetComponent<SessionButtonPresenter>();
            presenter.UpdateSession(sessionInfo);

            return button;
        }
    }
}