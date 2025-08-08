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

        [ShowInInspector, Sirenix.OdinInspector.ReadOnly]
        private readonly Dictionary<RegionTogglePresenter, RegionToggle> _toggles = new();
        
        private readonly CompositeDisposable _refreshRegionsDisposable = new();
        private readonly ITokenSourceService _tokenSourceService = new CancellationTokenSourceService();
        private readonly CompositeDisposable _disposables = new();
        private INetworkRegionsService _networkRegionsService;
        private CancellationTokenSource _refreshRegionsCancellationTokenSource;
        private bool _isInitialized;
        private MainMenuAssetProvider _assetProvider;
        private IInstantiator _instantiator;

        [Inject]
        private void Construct(INetworkRegionsService networkRegionsService, MainMenuAssetProvider assetProvider,
            IInstantiator instantiator)
        {
            _networkRegionsService = networkRegionsService;
            _assetProvider = assetProvider;
            _instantiator = instantiator;
        }

        private void OnEnable()
        {
            if (_isInitialized == false)
                return;
            
            _disposables.Clear();
            
            _networkRegionsService.ActualRegions
                .ObserveChanged()
                .Subscribe(x => OnRegionChange(x))
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
            ProcessToggleByRegion(_networkRegionsService.CurrentRegion, 
                toggleAction: (button) => button.Select());
        }

        public void StartAutoRefresh()
        {
            _refreshRegionsDisposable.Clear();
            
            _refreshRegionsCancellationTokenSource = _tokenSourceService
                .DisposeAndCreate(_refreshRegionsCancellationTokenSource);
            
            Observable.Interval(TimeSpan.FromSeconds(_regionsRefreshDelaySeconds))
                .Subscribe(_ =>
                {
                    _networkRegionsService.RefreshActualRegionsAsync(_refreshRegionsCancellationTokenSource.Token).Forget();
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

            ProcessToggleByRegion(regionInfo.RegionCode,
                presenterAction: (presenter) => presenter.UpdateRegion(regionInfo));
        }

        private void CreateAllRegionsButtons()
        {
            foreach (RegionTogglePresenter presenter in _toggles.Keys)
                Destroy(presenter.gameObject);
            
            foreach (Transform child in _container)
                Destroy(child.gameObject);
            
            _toggles.Clear();

            foreach (KeyValuePair<string, RegionInfo> regionData in _networkRegionsService.ActualRegions)
                CreateRegionToggle(regionData.Value);
        }
        
        private void CreateRegionToggle(RegionInfo regionInfo)
        {
            RegionToggle toggle = _instantiator.InstantiatePrefab(_assetProvider.GetRegionTogglePrefab(), _container)
                .GetComponent<RegionToggle>();
            
            RegionTogglePresenter presenter = toggle.GetComponent<RegionTogglePresenter>();
            
            presenter.UpdateRegion(regionInfo);
            toggle.Link(_toggleGroup);
            
            if (regionInfo.RegionCode == _networkRegionsService.CurrentRegion)
                toggle.Select();
        }
        
        private void ProcessToggleByRegion(string regionCode, Action<RegionToggle> toggleAction = null,
            Action<RegionTogglePresenter> presenterAction = null)
        {
            foreach (RegionTogglePresenter presenter in _toggles.Keys)
            {
                if (presenter.LastRegionInfo.RegionCode != regionCode)
                    continue;
                
                RegionToggle button = _toggles[presenter];
                toggleAction?.Invoke(button);
                presenterAction?.Invoke(presenter);
            }
        }
    }
}