using Sirenix.OdinInspector;
using UnityEngine;

namespace Modules.Wallet
{
    [CreateAssetMenu(fileName = "new WalletSettings", menuName = "Modules/WalletSettings")]
    public sealed class WalletSettings : ScriptableObject
    {
        [field: SerializeField, MinValue(0)] public int StartValue { get; private set; }
    }
}