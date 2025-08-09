using Cysharp.Threading.Tasks;
using Fusion;

namespace SampleGame.App
{
    public partial class GameFacade
    {
        public async UniTask QuitGameAsync()
        {
            await ShutdownRunnerAsync(ShutdownReason.HostMigration);
        }

        public async UniTask ShutdownGameAsync()
        {
            await ShutdownRunnerAsync(ShutdownReason.Ok);
        }
        
        private async UniTask ReloadGameplaySceneAsync()
        {
            await Runner.LoadScene(GetGameplaySceneRef(), setActiveOnLoad: true);
        }

        private async UniTask ShutdownRunnerAsync(ShutdownReason reason, bool isDestroyGameObject = true)
        {
            if (Runner != null)
            {
                if (Runner.IsRunning)
                {
                    await Runner.Shutdown(shutdownReason: reason, destroyGameObject: isDestroyGameObject);
                    Runner.RemoveCallbacks(this);
                }
            }
        }
    }
}