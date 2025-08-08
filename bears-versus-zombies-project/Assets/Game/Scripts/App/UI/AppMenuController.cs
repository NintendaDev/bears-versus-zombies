using System;
using Modules.EventBus;

namespace SampleGame.App.UI
{
    public sealed class AppMenuController : IDisposable
    {
        private readonly MainMenuPresenter _mainMenuPresenter;
        private readonly LobbyPresenter _lobbyPresenter;
        private readonly ISignalBus _signalBus;

        public AppMenuController(MainMenuPresenter mainMenuPresenter, LobbyPresenter lobbyPresenter, 
            ISignalBus signalBus)
        {
            _mainMenuPresenter = mainMenuPresenter;
            _lobbyPresenter = lobbyPresenter;
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
            _mainMenuPresenter.ShowInstance();
        }

        private void HideAll()
        {
            _mainMenuPresenter.Hide();
            _lobbyPresenter.Hide();
        }

        private void ShowMainMenu()
        {
            HideAll();
            _mainMenuPresenter.Show();
        }

        private void ShowLobby()
        {
            HideAll();
            _lobbyPresenter.Show();
        }
    }
}