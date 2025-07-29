using R3;
using SampleGame.Gameplay.GameObjects;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SampleGame.Gameplay.UI
{
    public sealed class LifetimePresenter : MonoBehaviour
    {
        [SerializeField, Required] private AutoKillView _view;
        [SerializeField, Required] private LifetimeComponent _killer;
        
        private readonly CompositeDisposable _disposable = new();

        private void OnEnable()
        {
            _disposable.Clear();
            _killer.CurrentTimer.Subscribe(OnTimerTick).AddTo(_disposable);
        }

        private void OnDisable()
        {
            _disposable.Clear();
        }

        private void OnTimerTick(float currentValue)
        {
            _view.SetPercent(currentValue / _killer.KillDelaySeconds);
        }
    }
}