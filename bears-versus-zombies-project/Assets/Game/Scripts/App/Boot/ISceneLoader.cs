using Cysharp.Threading.Tasks;
using Modules.LoadingTree;

namespace SampleGame.App
{
    public interface ISceneLoader
    {
        public UniTask<LoadingResult> LoadAsync();
    }
}