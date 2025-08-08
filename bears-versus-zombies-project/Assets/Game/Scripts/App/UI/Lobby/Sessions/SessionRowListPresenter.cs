using System.Collections.Generic;
using Fusion;
using Modules.EventBus;
using ObservableCollections;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace SampleGame.App.UI
{
    public sealed class SessionRowListPresenter : MonoBehaviour
    {
        [SerializeField, Required, ChildGameObjectsOnly]
        private Transform _container;
        
        private readonly CompositeDisposable _disposables = new();
        private readonly Dictionary<string, SessionRowPresenter> _rowsPresenters = new();
        private GameFacade _gameFacade;
        private ISignalBus _signalBus;
        private IInstantiator _instantiator;
        private MainMenuAssetProvider _assetProvider;

        [Inject]
        private void Construct(GameFacade gameFacade, ISignalBus signalBus, IInstantiator instantiator,
            MainMenuAssetProvider assetProvider)
        {
            _gameFacade = gameFacade;
            _signalBus = signalBus;
            _instantiator = instantiator;
            _assetProvider = assetProvider;
            
            foreach (Transform parent in _container)
                Destroy(parent.gameObject);
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }

        public void Show()
        {
            DestroyAllButtons();

            foreach (KeyValuePair<string, SessionInfo> sessionData in _gameFacade.ActualSessions)
                TryCreateButton(sessionData.Value);
            
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

        public void Hide()
        {
            DestroyAllButtons();
        }

        private void DestroyAllButtons()
        {
            foreach (SessionRowPresenter presenter in _rowsPresenters.Values)
            {
                _signalBus.Invoke(new SessionRowDropSignal(presenter.SessionName));
                Destroy(presenter.gameObject);
            }
            
            _rowsPresenters.Clear();
            _disposables.Clear();
        }

        private bool TryCreateButton(SessionInfo sessionInfo)
        {
            SessionRowView rowView = CreateSessionRow(sessionInfo, _container);
            SessionRowPresenter presenter = rowView.GetComponent<SessionRowPresenter>();
            _rowsPresenters[sessionInfo.Name] = presenter;

            presenter.Clicked
                .Subscribe((sessionName) => OnRowClick(sessionName))
                .AddTo(_disposables);
            
            _rowsPresenters[sessionInfo.Name] = presenter;

            return true;
        }

        private void OnRowClick(string sessionName)
        {
            _signalBus.Invoke(new SessionRowClickSignal(sessionName));
        }

        private void OnSessionAdd(CollectionAddEvent<KeyValuePair<string, SessionInfo>> collectionAddEvent)
        {
            TryCreateButton(collectionAddEvent.Value.Value);
        }

        private void OnSessionRemove(CollectionRemoveEvent<KeyValuePair<string, SessionInfo>> collectionRemoveEvent)
        {
            DestroyButton(collectionRemoveEvent.Value.Value);
        }

        private void DestroyButton(SessionInfo sessionInfo)
        {
            if (_rowsPresenters.ContainsKey(sessionInfo.Name) == false)
                return;
            
            SessionRowPresenter presenter = _rowsPresenters[sessionInfo.Name];
            
            _signalBus.Invoke(new SessionRowDropSignal(presenter.SessionName));
            Destroy(presenter.gameObject);
            
            _rowsPresenters.Remove(sessionInfo.Name);
        }

        private void OnSessionChange(CollectionChangedEvent<KeyValuePair<string, SessionInfo>> collectionChangedEvent)
        {
            SessionInfo sessionInfo = collectionChangedEvent.NewItem.Value;

            if (sessionInfo == null)
                return;

            if (_rowsPresenters.TryGetValue(sessionInfo.Name, out SessionRowPresenter rowPresenter) == false)
                return;
            
            rowPresenter.UpdateSession(sessionInfo);
        }
        
        private SessionRowView CreateSessionRow(SessionInfo sessionInfo, Transform parent)
        {
            SessionRowView rowView = _instantiator.InstantiatePrefab(_assetProvider.GetSessionRowPrefab(), parent).
                GetComponent<SessionRowView>();
            
            SessionRowPresenter presenter = rowView.GetComponent<SessionRowPresenter>();
            presenter.UpdateSession(sessionInfo);

            return rowView;
        }
    }
}