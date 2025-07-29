using System;
using Fusion;
using Modules.Services;
using SampleGame.App;
using SampleGame.Gameplay.GameContext;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SampleGame.Gameplay.UI
{
    public sealed class GameplayUIController : SceneGameCycleObserver
    {
        [SerializeField, Required, ChildGameObjectsOnly] 
        private SimpleScreenView _gameplayScreen;
        
        [SerializeField, Required, ChildGameObjectsOnly] 
        private FinishScreenView _finishScreen;
        
        private GameFacade _gameFacade;
        private GameLocalizationSystem _localizationService;

        public override void Spawned()
        {
            base.Spawned();
            
            _gameFacade = ServiceLocator.Instance.Get<GameFacade>();
            _localizationService = ServiceLocator.Instance.Get<GameLocalizationSystem>();
            
            _gameplayScreen.Initialize();
            _finishScreen.Initialize();
            
            _gameFacade.GameClosed += OnGameClose;
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            base.Despawned(runner, hasState);
            
            _gameFacade.GameClosed -= OnGameClose;
        }

        protected override void OnGameStateChange(GameState gameState, FinishReason finishReason)
        {
            switch (gameState)
            {
                case GameState.Waiting:
                    ShowGameplayScreen();
                    break;

                case GameState.Playing:
                    ShowGameplayScreen();
                    break;

                case GameState.Finished:
                    ProcessGameFinishReason(finishReason);
                    break;
            }
        }

        private void ShowGameplayScreen()
        {
            HideAll();
            _gameplayScreen.Show();
        }

        private void HideAll()
        {
            _gameplayScreen.Hide();
            _finishScreen.Hide();
        }

        private void ProcessGameFinishReason(FinishReason reason)
        {
            switch (reason)
            {
                case FinishReason.Win:
                    ShowFinishScreen(LocalizationTerm.Gameplay_YouWin, LocalizationTerm.Gameplay_WinMessage);
                    break;
                
                case FinishReason.PlayerDisconnect:
                    ShowFinishScreen(LocalizationTerm.Gameplay_GameOver, 
                        LocalizationTerm.Gameplay_PlayerDisconnectMessage);
                    break;

                case FinishReason.PlayerDie:
                    ShowFinishScreen(LocalizationTerm.Gameplay_YouLose,
                        LocalizationTerm.Gameplay_PlayerKillLoseMessage);
                    break;
                    
                case FinishReason.BusDestroyed:
                    ShowFinishScreen(LocalizationTerm.Gameplay_YouLose,
                        LocalizationTerm.Gameplay_BusDestroyLoseMessage);
                    break;

                case FinishReason.NetworkError:
                    ShowFinishScreen(LocalizationTerm.Gameplay_GameOver, 
                        LocalizationTerm.Gameplay_PlayerDisconnectMessage);
                    break;

                case FinishReason.PlayersWaitTimeout:
                    ShowFinishScreen(LocalizationTerm.Gameplay_GameOver, 
                        LocalizationTerm.Gameplay_WaitPlayersTimeoutMessage);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(reason), reason, null);
            }
        }
        
        private void ShowFinishScreen(LocalizationTerm titleTerm, LocalizationTerm messageTerm)
        {
            HideAll();
            
            _finishScreen.Show(title: _localizationService.MakeTranslatedText(titleTerm).ToUpper(),
                message: _localizationService.MakeTranslatedText(messageTerm).ToUpper());
        }

        private void OnGameClose()
        {
            ShowFinishScreen(LocalizationTerm.Gameplay_GameOver, 
                LocalizationTerm.Gameplay_PlayerDisconnectMessage);
        }
    }
}