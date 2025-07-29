using Fusion;
using Modules.Services;
using R3;
using SampleGame.Gameplay.GameContext;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SampleGame.Gameplay.UI
{
    public sealed class WaitPlayersPresenter : SceneGameCycleObserver
    {
        [SerializeField, Required] private TextView _view;
        
        private PlayersWaiter _waiter;
        private CompositeDisposable _disposable;

        public override void Spawned()
        {
            base.Spawned();
            
            _waiter = ServiceLocator.Instance.Get<PlayersWaiter>();
            _disposable = new CompositeDisposable();
            
            _view.Initialize();
            _view.Hide();
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            base.Despawned(runner, hasState);
            
            _disposable.Dispose();
        }

        protected override void OnGameStateChange(GameState gameState, FinishReason finishReason)
        {
            if (Runner.GameMode == GameMode.Single)
                return;
            
            if (gameState == GameState.Waiting)
            {
                _waiter.Countdown.Subscribe(OnCountdown).AddTo(_disposable);
                _view.Show();
                
                return;
            }
            
            Disable();
        }

        private void OnCountdown(float currentValue)
        {
            _view.SetText(Mathf.CeilToInt(currentValue).ToString());
        }

        private void Disable()
        {
            _disposable.Clear();
            _view.Hide();
        }
    }
}