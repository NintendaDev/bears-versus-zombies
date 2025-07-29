using Fusion;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Modules.Hitboxes
{
    public sealed class HitboxesStateSwitcher : MonoBehaviour
    {
        [SerializeField, Required] private HitboxRoot _hitboxRoot;
        [SerializeField] private Hitbox[] _hitboxes;

        public void Enable()
        {
            if (_hitboxes.Length == 0)
                return;
            
            _hitboxes.ForEach(x => _hitboxRoot.SetHitboxActive(x, true));
        }

        public void Disable()
        {
            if (_hitboxes.Length == 0)
                return;
            
            _hitboxes.ForEach(x => _hitboxRoot.SetHitboxActive(x, false));
        }
        
        [Button, HideInPlayMode]
        private void SearchChildrenHitboxes()
        {
            _hitboxes = GetComponentsInChildren<Hitbox>();
        }
    }
}