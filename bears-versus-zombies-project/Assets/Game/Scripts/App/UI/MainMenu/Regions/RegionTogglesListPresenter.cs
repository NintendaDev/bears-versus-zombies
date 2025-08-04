using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Fusion;
using Modules.AsyncTaskTokens;
using ObservableCollections;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using ZLinq;

namespace SampleGame.App.UI
{
    public sealed class RegionTogglesListPresenter : MonoBehaviour
    {
        [SerializeField, Required, ChildGameObjectsOnly]
        private Transform _container;

        [SerializeField, Required, ChildGameObjectsOnly]
        private ToggleGroup _toggleGroup;
        
        [SerializeField, MinValue(10)] 
        private float _regionsRefreshDelaySeconds = 180;
        
        private RegionToggleFactory _factory;

        [ShowInInspector, Sirenix.OdinInspector.ReadOnly]
        private readonly Dictionary<RegionToggle, RegionTogglePresenter> _toggles = new();
        
        private readonly CompositeDisposable _refreshRegionsDisposable = new();
        private readonly ITokenSourceService _tokenSourceService = new CancellationTokenSourceService();
        private readonly CompositeDisposable _disposables = new();
        private GameFacade _gameFacade;
        private CancellationTokenSource _refreshRegionsCancellationTokenSource;
        private bool _isInitialized;

        [Inject]
        private void Construct(GameFacade gameFacade, RegionToggleFactory factory)
        {
            _gameFacade = gameFacade;
            _factory = factory;
        }

        private void OnEnable()
        {
            if (_isInitialized == false)
                return;
            
            _disposables.Clear();
            
            _gameFacade.ActualRegions
                .ObserveChanged().Subscribe(x => OnRegionChange(x))
                .AddTo(_disposables);
        }

        private void OnDisable()
        {
            _disposables.Clear();
        }

        public UniTask InitializeAsync()
        {
            CreateAllRegionsButtons();
            OnEnable();
            
            _isInitialized = true;
            return UniTask.CompletedTask;
        }

        private void OnDestroy()
        {
            _refreshRegionsDisposable.Dispose();
            _tokenSourceService.TryDisposeToken(_refreshRegionsCancellationTokenSource);
        }

        public void SelectCurrentRegion()
        {
            foreach (RegionToggle button in _toggles.Keys.AsValueEnumerable()
                         .Where(x => x.RegionInfo.RegionCode == _gameFacade.CurrentRegion))
            {
                button.Select();
            }
        }

        public void StartAutoRefresh()
        {
            _refreshRegionsDisposable.Clear();
            
            _refreshRegionsCancellationTokenSource = _tokenSourceService
                .DisposeAndCreate(_refreshRegionsCancellationTokenSource);
            
            Observable.Interval(TimeSpan.FromSeconds(_regionsRefreshDelaySeconds))
                .Subscribe(_ =>
                {
                    _gameFacade.RefreshActualRegionsAsync(_refreshRegionsCancellationTokenSource.Token).Forget();
                })
                .AddTo(_refreshRegionsDisposable);
        }

        public void StopAutoRefresh()
        {
            _refreshRegionsDisposable.Clear();
            _tokenSourceService.TryDisposeToken(_refreshRegionsCancellationTokenSource);
        }

        private void OnRegionChange(CollectionChangedEvent<KeyValuePair<string, RegionInfo>> collectionChangedEvent)
        {
            RegionInfo regionInfo = collectionChangedEvent.NewItem.Value;

            foreach (RegionToggle button in _toggles.Keys 
                         .AsValueEnumerable()
                         .Where(button => button.RegionInfo.RegionCode == regionInfo.RegionCode))
            {
                _toggles[button].UpdateRegion(regionInfo);
            }
        }

        private void CreateAllRegionsButtons()
        {
            foreach (RegionToggle button in _toggles.Keys)
                Destroy(button.gameObject);
            
            foreach (Transform child in _container)
                Destroy(child.gameObject);
            
            _toggles.Clear();

            foreach (KeyValuePair<string, RegionInfo> regionData in _gameFacade.ActualRegions)
                CreateRegionButton(regionData.Value);
        }

        private void CreateRegionButton(RegionInfo regionInfo) => 
            _factory.Create(regionInfo, _toggleGroup, _container);
    }
}