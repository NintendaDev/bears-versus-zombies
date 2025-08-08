using Fusion;
using UnityEngine;

namespace SampleGame.Gameplay.Context
{
    public struct InputData : INetworkInput
    {
        public Vector3 MoveDirection;
        
        public NetworkButtons Buttons;
    }
}