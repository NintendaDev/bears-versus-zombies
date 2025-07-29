using Fusion;

namespace SampleGame.Gameplay.GameObjects
{
    public sealed class ConnectionTokenComponent : NetworkBehaviour
    {
        [Networked]
        public int Token { get; private set; }

        public bool TrySetToken(int token)
        {
            if (Runner.IsServer == false)
                return false;

            Token = token;

            return true;
        }
    }
}