using System.Threading;
using Cysharp.Threading.Tasks;
using Fusion;
using Fusion.Photon.Realtime;
using Modules.AsyncTaskTokens;
using Modules.SceneManagement;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SampleGame.App
{
    public partial class GameFacade : MonoBehaviour, INetworkRunnerCallbacks
    {
        [SerializeField, Required, AssetsOnly] private NetworkRunner _networkRunnerPrefab;
        [SerializeField, Required] private ConnectionTokenService _connectionTokenService;
        [SerializeField, Required] private string _levelSceneAddressablesPath = "Assets/Game/Scenes/GameLevel.unity";
        [SerializeField, Required] private AssetReference _levelSceneReference;
        
        private const string SingleSessionName = "SingleSession";
        private const int SinglePlayersCount = 1;
        private const int MultiPlayersCount = 2;
        private readonly ITokenSourceService _tokenSourceService = new TokenSourceService();
        private readonly ISingleSceneLoader _sceneLoader = new SingleSceneLoader();
        private string _lastSessionName;
        private CancellationTokenSource _connectionCancellationTokenSource;

        private void OnDestroy()
        {
            _tokenSourceService.Dispose();
        }

        public async UniTask InitializeAsync()
        {
            CreateNetworkComponents();
            await RefreshActualRegionsAsync();
            SetRegion(DefaultRegion);
        }

        public async UniTask<bool> TryLoadSingleGame()
        {
            _connectionCancellationTokenSource =
                _tokenSourceService.DisposeAndCreate(_connectionCancellationTokenSource);

            _lastSessionName = SingleSessionName;

            return await TryStartGame(GameMode.Single,
                SingleSessionName,
                playerCount: SinglePlayersCount,
                cancellationToken: default);
        }

        public async UniTask<bool> TryCreateMultiplayerGame(string sessionName)
        {
            _connectionCancellationTokenSource =
                _tokenSourceService.DisposeAndCreate(_connectionCancellationTokenSource);

            _lastSessionName = sessionName;
            await ShutdownRunnerAsync(ShutdownReason.Ok);

            return await TryStartGame(GameMode.Host,
                sessionName,
                playerCount: MultiPlayersCount,
                cancellationToken: _connectionCancellationTokenSource.Token);
        }

        public async UniTask<bool> TryConnectToMultiplayerGame(string sessionName)
        {
            _connectionCancellationTokenSource =
                _tokenSourceService.DisposeAndCreate(_connectionCancellationTokenSource);

            _lastSessionName = sessionName;
            await ShutdownRunnerAsync(ShutdownReason.Ok);

            return await TryStartGame(GameMode.Client,
                sessionName,
                playerCount: MultiPlayersCount,
                cancellationToken: _connectionCancellationTokenSource.Token);
        }

        private async UniTask<bool> TryStartGame(GameMode gameMode, string sessionName, int playerCount,
            CancellationToken cancellationToken)
        {
            CreateNetworkComponents();
            byte[] token = await _connectionTokenService.GetTokenAsync();
            FusionAppSettings settings = CreateAppSettings();
            NetworkSceneManager.DisableHostMigrationMode();

            StartGameResult startResult = await Runner.StartGame(new StartGameArgs
            {
                GameMode = gameMode,
                SessionName = sessionName,
                Scene = GetGameplaySceneRef(),
                SceneManager = NetworkSceneManager,
                ObjectProvider = ObjectProvider,
                PlayerCount = playerCount,
                ConnectionToken = token,
                StartGameCancellationToken = cancellationToken,
                CustomPhotonAppSettings = settings,
                CustomLobbyName = _defaultLobbyName,
            }).AsUniTask();

            if (startResult.Ok && gameMode == GameMode.AutoHostOrClient)
                await PushHostMigrationSnapshotAsync();

            return startResult.Ok;
        }
        
        private SceneRef GetGameplaySceneRef() => SceneRef.FromPath(_levelSceneAddressablesPath);
    }
}