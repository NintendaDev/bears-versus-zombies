using System;
using Cysharp.Threading.Tasks;
using Modules.EventBus;
using Modules.SaveSystem.Signals;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Modules.SaveSystem.SaveLoad
{
    public sealed class SaveLoadController : MonoBehaviour
    {
        [SerializeField, Required] private GameSaveLoader _saveLoader;
        [SerializeField, Required] private SignalBus _signalBus;

        private readonly CompositeDisposable _disposables = new();
        private const float SavePeriodSeconds = 1f;
        private bool _isRequiredSave;
        private UniTask _saveTask;

        private void OnEnable()
        {
            _disposables.Clear();
            
            _signalBus.Subscribe<SaveSignal>(OnSaveSignal);

            Observable
                .Interval(TimeSpan.FromSeconds(SavePeriodSeconds))
                .Subscribe(_ => StartSaveBehaviour())
                .AddTo(_disposables);
        }

        private void OnDisable()
        {
            _signalBus.Unsubscribe<SaveSignal>(OnSaveSignal);
            _disposables.Clear();
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