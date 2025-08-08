using R3;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.UI
{
    public abstract class ToggleBase<T> : MonoBehaviour
    {
        [SerializeField, Required, ChildGameObjectsOnly]
        private Toggle _toggle;

        private readonly ReactiveCommand<T> _checkedCommand = new();

        public Observable<T> Checked => _checkedCommand.AsObservable();

        protected virtual void OnEnable()
        {
            _toggle.onValueChanged.AddListener(OnValueChange);
        }

        protected virtual void OnDisable()
        {
            _toggle.onValueChanged.RemoveListener(OnValueChange);
        }
        
        public void Select()
        {
            _toggle.isOn = true;
        }
        
        public void Link(ToggleGroup group)
        {
            _toggle.group = group;
        }
        
        private void OnValueChange(bool isSelect)
        {
            if (isSelect == false)
                return;
            
            _checkedCommand.Execute((T)(object)this);
            Select();
        }
    }
}