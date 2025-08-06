using Cysharp.Threading.Tasks;
using Modules.LoadingTree;
using Modules.SaveSystem.SaveLoad;

namespace SampleGame.App
{
    public sealed class SaveLoadingOperation : LoadingOperationBase
    {
        private readonly IGameSaveLoader _gameSaveLoader;

        public SaveLoadingOperation(IGameSaveLoader gameSaveLoader)
        {
            _gameSaveLoader = gameSaveLoader;
        }
        
        protected override async UniTask<LoadingResult> RunInternal(LoadingBundle bundle)
        {
            if (await _gameSaveLoader.TryLoadAsync())
                return LoadingResult.Success();
            
            return LoadingResult.Error("Error load save data");
        }
    }
}