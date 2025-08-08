using Fusion;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace SampleGame.App.UI
{
    public sealed class SessionRowPresenter : MonoBehaviour
    {
        [SerializeField, Required, ChildGameObjectsOnly]
        private SessionRowView _rowView;

        private LocalizationManager _localizationManager;
        private SessionInfo _lastSessionInfo;
        private ReactiveCommand<string> _clickedCommand = new();
        
        public string SessionName => _lastSessionInfo?.Name ?? string.Empty;

        public Observable<string> Clicked => _clickedCommand.AsObservable();

        [Inject]
        private void Construct(LocalizationManager localizationManager)
        {
            _localizationManager = localizationManager;

            _rowView.Clicked
                .Subscribe((_) => _clickedCommand.Execute(_lastSessionInfo.Name))
                .AddTo(this);
        }

        private void OnEnable()
        {
            _localizationManager.LocalizationChanged += OnLocalizationChange;
        }

        private void OnDisable()
        {
            _localizationManager.LocalizationChanged -= OnLocalizationChange;
        }

        public void UpdateSession(SessionInfo sessionInfo)
        {
            _lastSessionInfo = sessionInfo;
            
            _rowView.Initialize(sessionInfo.Name, 
                $"{sessionInfo.PlayerCount} / {sessionInfo.MaxPlayers}",
                _localizationManager.MakeTranslatedRegionCode(sessionInfo.Region).ToUpper());
        }

        private void OnLocalizationChange()
        {
            UpdateSession(_lastSessionInfo);
        }
    }
}