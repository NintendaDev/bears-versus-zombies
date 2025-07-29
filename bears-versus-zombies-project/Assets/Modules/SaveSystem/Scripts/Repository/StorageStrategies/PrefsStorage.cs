using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Modules.SaveSystem.SaveStrategies
{
    public sealed class PrefsStorage : Storage
    {
        [SerializeField, Required] private string _key = "GameSave";

        public override UniTask<(bool, string)> TryReadAsync()
        {
            string data = PlayerPrefs.GetString(_key);
            bool isSuccess = string.IsNullOrEmpty(data) == false;
            
            return UniTask.FromResult((isSuccess, data));
        }

        public override UniTask WriteAsync(string data)
        {
            PlayerPrefs.SetString(_key, data);
            
            return UniTask.CompletedTask;
        }
    }
}