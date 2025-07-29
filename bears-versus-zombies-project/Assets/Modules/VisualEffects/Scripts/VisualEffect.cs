using Sirenix.OdinInspector;
using UnityEngine;

namespace Modules.VFX
{
    public abstract class VisualEffect : MonoBehaviour, IVisualEffect
    {
        [SerializeField] private bool _canPlayOnStart;
        
        protected virtual void Start()
        {
            if (_canPlayOnStart)
                Play();
        }
        
        [Button, HideInEditorMode]
        public abstract void Play();
        
        [Button, HideInEditorMode]
        public abstract void Stop();
    }
}