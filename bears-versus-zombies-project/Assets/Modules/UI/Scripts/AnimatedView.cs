using OM.Animora.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Modules.UI
{
    [RequireComponent(typeof(RectTransform))]
    public abstract class AnimatedView : MonoBehaviour
    {
        [SerializeField, Required] private AnimoraPlayer _animator;
        [SerializeField] private bool _hasResetPositionOnAwake = true;

        protected virtual void Awake()
        {
            if (_hasResetPositionOnAwake)
            {
                RectTransform rectTransform = GetComponent<RectTransform>();
                rectTransform.anchoredPosition = Vector2.zero;
            }
        }
        
        [Button, HideInEditorMode]
        public virtual void ShowInstance()
        {
            gameObject.SetActive(true);
        }

        [Button, HideInEditorMode]
        public virtual void Show()
        {
            ShowInstance();
            _animator.PlayAnimation();
        }

        [Button, HideInEditorMode]
        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}