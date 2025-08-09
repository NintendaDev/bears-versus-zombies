using Cysharp.Threading.Tasks;

namespace SampleGame.App.SceneManagement
{
    public interface IGameplayUnloader
    {
        public UniTask UnloadAsync();
    }
}