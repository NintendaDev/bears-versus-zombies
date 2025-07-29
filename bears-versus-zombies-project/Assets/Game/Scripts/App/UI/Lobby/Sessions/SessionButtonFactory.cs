using Cysharp.Threading.Tasks;
using Fusion;
using Modules.AssetsManagement.AddressablesOperations;
using Modules.AssetsManagement.StaticData;
using Modules.Services;
using UnityEngine;

namespace SampleGame.App.UI
{
    public sealed class SessionButtonFactory : MonoBehaviour
    {
        private IAddressablesService _addressablesService;
        private MainMenuAssets _assetsConfig;

        private SessionButton _prefab;
        private GameFacade _gameFacade;

        private void OnDestroy()
        {
            _addressablesService.Release(_assetsConfig.SessionButtonReference);
        }

        public async UniTask InitializeAsync()
        {
            IStaticDataService staticData = ServiceLocator.Instance.Get<IStaticDataService>();
            _addressablesService = ServiceLocator.Instance.Get<IAddressablesService>();
            _assetsConfig = staticData.GetConfiguration<MainMenuAssets>();

            await LoadAssetsAsync();
        }
        
        public async UniTask<SessionButton> CreateButtonAsync(SessionInfo sessionInfo, Transform parent)
        {
            SessionButton button = Instantiate(_prefab, parent);
            SessionButtonPresenter presenter = button.GetComponent<SessionButtonPresenter>();
            await presenter.InitializeAsync();
            presenter.UpdateSession(sessionInfo);

            return button;
        }
        
        private async UniTask LoadAssetsAsync()
        {
            GameObject sessionButtonObject = await _addressablesService
                .LoadAsync<GameObject>(_assetsConfig.SessionButtonReference);
            
            _prefab = sessionButtonObject.GetComponent<SessionButton>();
        }
    }
}