using Fusion;
using UnityEngine;

namespace SampleGame.Gameplay.GameContext
{
    public struct InputData : INetworkInput
    {
        public Vector3 MoveDirection;
        
        public NetworkButtons Buttons;
    }
}