using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Modules.SaveSystem.Repositories;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Modules.SaveSystem.SaveLoad
{
    public sealed class GameSaveLoader : MonoBehaviour, IGameSaveLoader
    {
        [SerializeField, Required] private GameRepository _repository;
        [SerializeField, Required] private Transform _serializersRoot;
        
        private IGameSerializer[] _serializers;

        public UniTask InitializeAsync()
        {
            _serializers = _serializersRoot.GetComponentsInChildren<IGameSerializer>();
            
            return UniTask.CompletedTask;
        }

        public async UniTask SaveAsync()
        {
            var gameState = new Dictionary<string, string>();
            
            foreach (IGameSerializer serializer in _serializers)
                serializer.Serialize(gameState);

            await _repository.SetStateAsync(gameState);
        }

        public async UniTask<bool> TryLoadAsync()
        {
            Dictionary<string, string> gameState = await _repository.GetStateAsync();

            if (gameState == null || gameState.Count == 0)
                return true;
            
            foreach (IGameSerializer serializer in _serializers)
                serializer.Deserialize(gameState);
            
            return true;
        }
    }
}