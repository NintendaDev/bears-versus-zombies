using System;
using UnityEngine;

namespace SampleGame.Gameplay.GameContext
{
    [Serializable]
    public struct WaveSettings
    {
        [field: SerializeField] public int Count { get; private set; }
            
        [field: SerializeField] public float Delay { get; private set; }
    }
}