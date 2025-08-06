using System;
using Cysharp.Threading.Tasks;
using Modules.EventBus;
using Modules.SaveSystem.Signals;
using R3;
using UnityEngine;

namespace Modules.SaveSystem.SaveLoad
{
    public sealed class SaveLoadController : IDisposable
    {
        private readonly IGameSaveLoader _saveLoader;
        private readonly ISignalBus _signalBus;
        private readonly CompositeDisposable _disposables = new();
        private bool _isRequiredSave;
        private UniTask _saveTask;

        public SaveLoadController(IGameSaveLoader saveLoader, ISignalBus signalBus,
            float savePeriodSeconds)
        {
            _saveLoader = saveLoader;
            _signalBus = signalBus;
            
            _signalBus.Subscribe<SaveSignal>(OnSaveSignal);

            Observable
                .Interval(TimeSpan.FromSeconds(savePeriodSeconds))
                .Subscribe(_ => StartSaveBehaviour())
                .AddTo(_disposables);
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<SaveSignal>(OnSaveSignal);
            _disposables.Dispose();
        }

        private void OnSaveSignal()
        {
            _isRequiredSave = true;
        }

        private void StartSaveBehaviour()
        {
            if (_saveTask.Status != UniTaskStatus.Succeeded)
                return;
            
            _saveTask = StartSaveBehaviourAsync();
            _saveTask.Forget();
        }

        private async UniTask StartSaveBehaviourAsync()
        {
            if (_isRequiredSave == false)
                return;
            
            await _saveLoader.SaveAsync();
            Debug.Log($"Game saved...");
            _isRequiredSave = false;
        }
    }
}