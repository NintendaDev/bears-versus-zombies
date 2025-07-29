using Sirenix.OdinInspector;
using UnityEngine;

namespace SampleGame.App
{
    public sealed class PerformanceSetter : MonoBehaviour
    {
        [SerializeField, MinValue(30)] private int _targetFrameRate = 60;
        [SerializeField, MinValue(30)] private bool _setUpOnAwake = true;
        
        private void Awake()
        {
            if (_setUpOnAwake)
                SetUp();
        }

        public void SetUp()
        {
            Application.targetFrameRate = _targetFrameRate;
        }
    }
}