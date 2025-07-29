using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SampleGame.App
{
    public abstract class SceneInitializer : MonoBehaviour
    {
        public abstract UniTask InitializeAsync();
    }
}