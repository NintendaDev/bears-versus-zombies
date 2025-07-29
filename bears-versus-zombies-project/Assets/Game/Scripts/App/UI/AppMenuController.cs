using Cysharp.Threading.Tasks;
using Modules.EventBus;
using Modules.Services;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SampleGame.App.UI
{
    public sealed class AppMenuController : MonoBehaviour
    {
        [SerializeField, Required]
        private MainMenuView _mainMenuView;
        
        [SerializeField, Required]
        private LobbyMenuView _lobbyMenuView;

        private ISignalBus _signalBus;
        private bool _isInitialized;

        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }
        
        public UniTask InitializeAsync()
        {
            if (_isInitialized)
                return UniTask.CompletedTask;
            
            _signalBus = ServiceLocator.Instance.Get<ISignalBus>();
            _isInitialized = true;
            
            Subscribe();
            
            return UniTask.CompletedTask;
        }

        private void Subscribe()
        {
            if (_isInitialized == false)
                return;
            
            _signalBus.Subscribe<ShowMainMenuInstanceSignal>(ShowMainMenuInstance);
            _signalBus.Subscribe<ShowMainMenuSignal>(ShowMainMenu);
            _signalBus.Subscribe<OpenLobbyMenuSignal>(ShowLobby);
        }

        private void Unsubscribe()
        {
            if (_isInitialized == false)
                return;
            
            _signalBus.Unsubscribe<ShowMainMenuInstanceSignal>(ShowMainMenuInstance);
            _signalBus.Unsubscribe<ShowMainMenuSignal>(ShowMainMenu);
            _signalBus.Unsubscribe<OpenLobbyMenuSignal>(ShowLobby);
        }
        
        private void ShowMainMenuInstance()
        {
            HideAll();
            _mainMenuView.ShowInstance();
        }

        private void HideAll()
        {
            _mainMenuView.Hide();
            _lobbyMenuView.Hide();
        }

        private void ShowMainMenu()
        {
            HideAll();
            _mainMenuView.Show();
        }

        private void ShowLobby()
        {
            HideAll();
            _lobbyMenuView.Show();
        }
    }
}