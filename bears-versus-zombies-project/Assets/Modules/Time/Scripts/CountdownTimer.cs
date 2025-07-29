using System;
using R3;
using UnityEngine;

namespace Modules.Timers
{
    public sealed class CountdownTimer : IDisposable
    {
        private readonly CompositeDisposable _disposables = new();
        private readonly ReactiveProperty<float> _countdown = new();

        public ReadOnlyReactiveProperty<float> Countdown => _countdown;
        
        public bool IsRunning => _countdown.Value > 0;

        public event Action Finished;
        
        public void Dispose()
        {
            _disposables.Dispose();
        }
        
        public void Start(float seconds)
        {
            _disposables.Clear();
            _countdown.Value = seconds;
            
            Observable.EveryUpdate()
                .Where(_ => _countdown.Value > 0)
                .Subscribe(_ => OnTimerTick())
                .AddTo(_disposables);
        }

        private void OnTimerTick()
        {
            _countdown.Value = Mathf.Max(0, _countdown.Value - Time.deltaTime);
            
            if (_countdown.Value == 0)
            {
                _disposables.Clear();
                Finished?.Invoke();
            }
        }
    }
}