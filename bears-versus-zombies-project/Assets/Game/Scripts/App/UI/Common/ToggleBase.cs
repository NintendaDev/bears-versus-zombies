using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace SampleGame.App.UI
{
    public abstract class ToggleBase<T> : MonoBehaviour
    {
        [SerializeField, Required, ChildGameObjectsOnly]
        private Toggle _toggle;

        public event Action<T> Checked;

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
            
            Checked?.Invoke((T)(object)this);
            Select();
        }
    }
}