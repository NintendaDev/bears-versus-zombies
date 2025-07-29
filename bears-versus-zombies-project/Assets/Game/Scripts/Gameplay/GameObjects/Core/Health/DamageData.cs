using Fusion;

namespace SampleGame.Gameplay.GameObjects
{
    public struct DamageData : INetworkStruct
    {
        public int Value;

        public int Tick;
        
        public NetworkBool IsSelf;
        
        public static DamageData Default(int tick) => new() { Value = 0, IsSelf = false, Tick = tick };
        
        public static DamageData Self(int tick) => new() { Value = 0, IsSelf = true, Tick = tick  };
        
        public static DamageData External(int value, int tick) => new() { Value = value, IsSelf = false, 
            Tick = tick  };
    }
}