using Fusion;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace SampleGame.App.UI
{
    public sealed class SessionButtonPresenter : MonoBehaviour
    {
        [SerializeField, Required, ChildGameObjectsOnly]
        private SessionButton _button;

        private GameLocalizationSystem _localizationSystem;
        private SessionInfo _lastSessionInfo;

        [Inject]
        private void Construct(GameLocalizationSystem gameLocalizationSystem)
        {
            _localizationSystem = gameLocalizationSystem;
        }

        private void OnEnable()
        {
            _localizationSystem.LocalizationChanged += OnLocalizationChange;
        }

        private void OnDisable()
        {
            _localizationSystem.LocalizationChanged -= OnLocalizationChange;
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
    }
}