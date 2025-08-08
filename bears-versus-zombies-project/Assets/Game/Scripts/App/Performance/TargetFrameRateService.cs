using Unity.VisualScripting;
using UnityEngine;

namespace SampleGame.App
{
    public sealed class TargetFrameRateService : IInitializable
    {
        private readonly int _targetFrameRate;

        public TargetFrameRateService(int targetFrameRate)
        {
            _targetFrameRate = targetFrameRate;
        }

        public void Initialize()
        {
            Application.targetFrameRate = _targetFrameRate;
        }
    }
}