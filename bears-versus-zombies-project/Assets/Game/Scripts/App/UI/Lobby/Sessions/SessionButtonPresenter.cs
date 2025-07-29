using Cysharp.Threading.Tasks;
using Fusion;
using Modules.Services;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SampleGame.App.UI
{
    public sealed class SessionButtonPresenter : MonoBehaviour
    {
        [SerializeField, Required, ChildGameObjectsOnly]
        private SessionButton _button;

        private GameLocalizationSystem _localizationSystem;
        private SessionInfo _lastSessionInfo;
        private bool _isInitialized;

        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        public UniTask InitializeAsync()
        {
            if (_isInitialized)
                return UniTask.CompletedTask;
            
            _localizationSystem = ServiceLocator.Instance.Get<GameLocalizationSystem>();
            _isInitialized = true;
            Subscribe();
            
            return UniTask.CompletedTask;
        }

        private void Subscribe()
        {
            if (_isInitialized == false)
                return;
            
            _localizationSystem.LocalizationChanged += OnLocalizationChange;
        }
        
        public void UpdateSession(SessionInfo sessionInfo)
        {
            _lastSessionInfo = sessionInfo;
            
            _button.Initialize(sessionInfo.Name, 
                $"{sessionInfo.PlayerCount} / {sessionInfo.MaxPlayers}",
                _localizationSystem.MakeTranslatedRegionCode(sessionInfo.Region).ToUpper());
        }

        private void OnLocalizationChange()
        {
            UpdateSession(_lastSessionInfo);
        }
        
        private void Unsubscribe()
        {
            if (_isInitialized == false)
                return;
            
            _localizationSystem.LocalizationChanged -= OnLocalizationChange;
        }
    }
}