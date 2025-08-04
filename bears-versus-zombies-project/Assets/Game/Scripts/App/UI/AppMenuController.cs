using System;
using Modules.EventBus;

namespace SampleGame.App.UI
{
    public sealed class AppMenuController : IDisposable
    {
        private readonly MainMenuView _mainMenuView;
        private readonly LobbyMenuView _lobbyMenuView;
        private readonly ISignalBus _signalBus;

        public AppMenuController(MainMenuView mainMenuView, LobbyMenuView lobbyMenuView, ISignalBus signalBus)
        {
            _mainMenuView = mainMenuView;
            _lobbyMenuView = lobbyMenuView;
            _signalBus = signalBus;

            _signalBus.Subscribe<ShowMainMenuInstanceSignal>(ShowMainMenuInstance);
            _signalBus.Subscribe<ShowMainMenuSignal>(ShowMainMenu);
            _signalBus.Subscribe<OpenLobbyMenuSignal>(ShowLobby);
        }

        public void Dispose()
        {
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