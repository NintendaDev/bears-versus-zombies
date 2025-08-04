using Unity.VisualScripting;
using UnityEngine;

namespace SampleGame.App
{
    public sealed class PerformanceSetter : IInitializable
    {
        private readonly int _targetFrameRate;

        public PerformanceSetter(int targetFrameRate)
        {
            _targetFrameRate = targetFrameRate;
        }

        public void Initialize()
        {
            Application.targetFrameRate = _targetFrameRate;
        }
    }
}