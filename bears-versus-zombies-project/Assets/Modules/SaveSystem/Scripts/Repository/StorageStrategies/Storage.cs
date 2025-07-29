using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Modules.SaveSystem.SaveStrategies
{
    public abstract class Storage : MonoBehaviour, IStorage
    {
        public abstract UniTask<(bool, string)> TryReadAsync();

        public abstract UniTask WriteAsync(string data);
    }
}