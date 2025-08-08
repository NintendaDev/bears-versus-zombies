using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Fusion;
using Fusion.Photon.Realtime;
using Modules.AssetsManagement.StaticData;
using Modules.AsyncTaskTokens;
using Modules.SceneManagement;

namespace SampleGame.App
{
    public partial class GameFacade : IDisposable, INetworkRunnerCallbacks
    {
        private readonly ITokenSourceService _cancelTokenSourceService = new CancellationTokenSourceService();
        private readonly ISingleSceneLoader _sceneLoader = new SingleSceneLoader();
        private readonly IStaticDataService _staticDataService;
        private readonly IConnectionTokenService _connectionTokenService;
        private readonly INetworkRegionsService _networkRegionsService;
        private GameFacadeConfig _config;
        private string _lastSessionName;
        private CancellationTokenSource _connectionCancellationTokenSource;

        public GameFacade(IStaticDataService staticDataService, IConnectionTokenService connectionTokenService,
            INetworkRegionsService networkRegionsService)
        {
            _staticDataService = staticDataService;
            _connectionTokenService = connectionTokenService;
            _networkRegionsService = networkRegionsService;
        }

        public bool IsRunning => Runner != null && Runner.IsRunning;

        public GameMode GameMode => Runner.GameMode;

        public bool IsClient => Runner.IsClient;
        
        public void Dispose()
        {
            _cancelTokenSourceService.Dispose();
        }

        public async UniTask InitializeAsync()
        {
            _config = _staticDataService.GetConfiguration<GameFacadeConfig>();
            CreateNetworkComponents();
            await _networkRegionsService.RefreshActualRegionsAsync();
        }

        public async UniTask<bool> TryLoadSingleGame()
        {
            _connectionCancellationTokenSource =
                _cancelTokenSourceService.DisposeAndCreate(_connectionCancellationTokenSource);

            _lastSessionName = GameFacadeConfig.SingleSessionName;

            return await TryStartGame(GameMode.Single,
                GameFacadeConfig.SingleSessionName,
                playerCount: GameFacadeConfig.SinglePlayersCount,
                cancellationToken: default);
        }

        public async UniTask<bool> TryCreateMultiplayerGame(string sessionName)
        {
            _connectionCancellationTokenSource =
                _cancelTokenSourceService.DisposeAndCreate(_connectionCancellationTokenSource);

            _lastSessionName = sessionName;
            await ShutdownRunnerAsync(ShutdownReason.Ok);

            return await TryStartGame(GameMode.Host,
                sessionName,
                playerCount: GameFacadeConfig.MultiPlayersCount,
                cancellationToken: _connectionCancellationTokenSource.Token);
        }

        public async UniTask<bool> TryConnectToMultiplayerGame(string sessionName)
        {
            _connectionCancellationTokenSource =
                _cancelTokenSourceService.DisposeAndCreate(_connectionCancellationTokenSource);

            _lastSessionName = sessionName;
            await ShutdownRunnerAsync(ShutdownReason.Ok);

            return await TryStartGame(GameMode.Client,
                sessionName,
                playerCount: GameFacadeConfig.MultiPlayersCount,
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
        
        private SceneRef GetGameplaySceneRef() => SceneRef.FromPath(_config.LevelSceneAddressablesPath);
    }
}