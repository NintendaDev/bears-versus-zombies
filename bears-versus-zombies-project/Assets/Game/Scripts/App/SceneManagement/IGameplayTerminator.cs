using Cysharp.Threading.Tasks;

namespace SampleGame.App.SceneManagement
{
    public interface IGameplayTerminator
    {
        public UniTask TerminateAsync();
    }
}