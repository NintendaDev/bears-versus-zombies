using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Fusion;
using Modules.EventBus;
using Modules.Services;
using ObservableCollections;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SampleGame.App.UI
{
    public sealed class SessionButtonListPresenter : MonoBehaviour
    {
        [SerializeField, Required, ChildGameObjectsOnly]
        private Transform _container;
        
        private readonly CompositeDisposable _disposables = new();
        private readonly Dictionary<string, SessionButton> _currentButtons = new();
        private readonly Dictionary<string, SessionButtonPresenter> _buttonsPresenters = new();
        private SessionButtonFactory _factory;
        private GameFacade _gameFacade;
        private ISignalBus _signalBus;

        private void OnDestroy()
        {
            _disposables.Dispose();
            
            foreach (SessionButton button in _currentButtons.Values)
                button.Clicked -= OnButtonClick;
        }

        public UniTask InitializeAsync()
        {
            _factory = ServiceLocator.Instance.Get<SessionButtonFactory>();
            _gameFacade = ServiceLocator.Instance.Get<GameFacade>();
            _signalBus = ServiceLocator.Instance.Get<ISignalBus>();
            
            foreach (Transform parent in _container)
                Destroy(parent.gameObject);
            
            return UniTask.CompletedTask;
        }

        public void OnShow()
        {
            _disposables.Clear();
            DestroyAllButtons();

            foreach (KeyValuePair<string, SessionInfo> sessionData in _gameFacade.ActualSessions)
            {
                if (CanDrawSession(sessionData.Value))
                    TryCreateButtonAsync(sessionData.Value);
            }
            
            _gameFacade.ActualSessions
                .ObserveAdd()
                .Subscribe(OnSessionAdd)
                .AddTo(_disposables);
            
            _gameFacade.ActualSessions
                .ObserveRemove()
                .Subscribe(OnSessionRemove)
                .AddTo(_disposables);
            
            _gameFacade.ActualSessions
                .ObserveChanged()
                .Subscribe(OnSessionChange)
                .AddTo(_disposables);
        }

        public void OnHide()
        {
            _disposables.Clear();
            DestroyAllButtons();
        }

        private void DestroyAllButtons()
        {
            foreach (SessionButton button in _currentButtons.Values)
            {
                button.Clicked -= OnButtonClick;
                _signalBus.Invoke(new SessionButtonDropSignal(button.SessionName));
                Destroy(button.gameObject);
            }
            
            _currentButtons.Clear();
        }

        private bool CanDrawSession(SessionInfo sessionInfo)
        {
            return sessionInfo.IsValid 
                   && sessionInfo.IsOpen 
                   && sessionInfo.IsVisible;
        }

        private async UniTask<bool> TryCreateButtonAsync(SessionInfo sessionInfo)
        {
            if (CanDrawSession(sessionInfo) == false)
                return false;

            SessionButton button = await _factory.CreateButtonAsync(sessionInfo, _container);
            SessionButtonPresenter presenter = button.GetComponent<SessionButtonPresenter>();
            _buttonsPresenters[sessionInfo.Name] = presenter;
            
            button.Clicked += OnButtonClick;
            _currentButtons.Add(sessionInfo.Name, button);

            return true;
        }

        private void OnButtonClick(string sessionName)
        {
            _signalBus.Invoke(new SessionButtonClickSignal(sessionName));
        }

        private void OnSessionAdd(CollectionAddEvent<KeyValuePair<string, SessionInfo>> collectionAddEvent)
        {
            TryCreateButtonAsync(collectionAddEvent.Value.Value).Forget();
        }

        private void OnSessionRemove(CollectionRemoveEvent<KeyValuePair<string, SessionInfo>> collectionRemoveEvent)
        {
            DestroyButton(collectionRemoveEvent.Value.Value);
        }

        private void DestroyButton(SessionInfo sessionInfo)
        {
            if (_currentButtons.ContainsKey(sessionInfo.Name) == false)
                return;
            
            SessionButton button = _currentButtons[sessionInfo.Name];
            button.Clicked -= OnButtonClick;
            
            _signalBus.Invoke(new SessionButtonDropSignal(button.SessionName));
            Destroy(button.gameObject);
            
            _buttonsPresenters.Remove(sessionInfo.Name);
            _currentButtons.Remove(sessionInfo.Name);
        }

        private void OnSessionChange(CollectionChangedEvent<KeyValuePair<string, SessionInfo>> collectionChangedEvent)
        {
            SessionInfo sessionInfo = collectionChangedEvent.NewItem.Value;

            if (sessionInfo == null)
                return;
            
            if (_buttonsPresenters.TryGetValue(sessionInfo.Name, out SessionButtonPresenter buttonPresenter) == false)
                return;
            
            buttonPresenter.UpdateSession(sessionInfo);
        }
    }
}