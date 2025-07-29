using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Modules.Encrypt;
using Modules.SaveSystem.Repositories.SerializeStrategies;
using Modules.SaveSystem.SaveStrategies;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Modules.SaveSystem.Repositories
{
    public sealed class GameRepository : MonoBehaviour, IGameRepository
    {
        [SerializeField, Required] private string _aesPassword;
        [SerializeField, Required] private Storage _storage;
        [SerializeField, Required] private Serialization _serialization;
        
        private bool HasEncrypt => string.IsNullOrEmpty(_aesPassword);

        public async UniTask<Dictionary<string, string>> GetStateAsync()
        {
            (bool IsSucces, string EncryptedData) storageResult = await _storage.TryReadAsync();
            
            if (storageResult.IsSucces == false)
                return new Dictionary<string, string>();
            
            string jsonData = (HasEncrypt) ?  storageResult.EncryptedData.Decrypt(_aesPassword) 
                : storageResult.EncryptedData;
            
            var gameState = _serialization.Deserialize(jsonData);
            
            if (gameState == null)
                return new Dictionary<string, string>();
            
            return gameState;
        }

        public async UniTask SetStateAsync(Dictionary<string, string> gameState)
        {
            string serializedData = _serialization.Serialize(gameState);
            string encryptedData = (HasEncrypt) ?  serializedData.Encrypt(_aesPassword) : serializedData;
            
            await _storage.WriteAsync(encryptedData);
        }
    }
}