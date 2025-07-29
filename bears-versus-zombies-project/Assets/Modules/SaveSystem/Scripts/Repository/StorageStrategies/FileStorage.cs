using System.IO;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Modules.SaveSystem.SaveStrategies
{
    public sealed class FileStorage : Storage
    {
        [SerializeField, Required]
        private string _filePath;

        public override UniTask<(bool, string)> TryReadAsync()
        {
            if (File.Exists(_filePath) == false)
                return UniTask.FromResult((true, string.Empty));

            string data = File.ReadAllText(_filePath);

            if (string.IsNullOrEmpty(data))
                return UniTask.FromResult((false, data));

            return UniTask.FromResult((true, data));
        }

        public override UniTask WriteAsync(string data)
        {
            File.WriteAllText(_filePath, data);
            
            return UniTask.CompletedTask;
        }
    }
}