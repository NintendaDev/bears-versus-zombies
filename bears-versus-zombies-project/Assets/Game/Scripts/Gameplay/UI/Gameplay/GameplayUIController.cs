using System;
using Fusion;
using R3;
using SampleGame.App;
using SampleGame.Gameplay.Context;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace SampleGame.Gameplay.UI
{
    public sealed class GameplayUIController : SceneGameCycleObserver
    {
        [SerializeField, Required, ChildGameObjectsOnly] 
        private SimpleScreenView _gameplayScreen;
        
        [SerializeField, Required, ChildGameObjectsOnly] 
        private FinishScreenView _finishScreen;

        private readonly CompositeDisposable _disposables = new();
        private GameFacade _gameFacade;
        private LocalizationManager _localizationService;

        [Inject]
        private void Construct(GameFacade gameFacade, LocalizationManager localizationService)
        {
            _gameFacade = gameFacade;
            _localizationService = localizationService;
            _gameplayScreen.Initialize();
            _finishScreen.Initialize();
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            base.Despawned(runner, hasState);
            
            _gameFacade.GameClosed
                .Subscribe((_) => OnGameClose())
                .AddTo(_disposables);
        }

        protected override void OnSpawnedInternal()
        {
            base.OnSpawnedInternal();
            _disposables.Clear();
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