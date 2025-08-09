using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.UI
{
    public sealed class ProgressBar : MonoBehaviour
    {
        [SerializeField, Required, ChildGameObjectsOnly] 
        private Slider _slider;

        [SerializeField, MinValue(0)] 
        private float _animationSpeed = 0.75f;

        private Tween _tween;
        private GameObject _gameObject;

        public void Reset()
        {
            _slider.value = 0;
        }

        public void Show()
        {
            if (_gameObject == null)
                _gameObject = gameObject;

            if (_gameObject.activeSelf)
                return;
            
            _gameObject.SetActive(true);
        }

        public void Hide()
        {
            if (_gameObject == null)
                _gameObject = gameObject;
            
            if (_gameObject.activeSelf == false)
                return;
            
            _gameObject.SetActive(false);
        }
        
        public void SetProgress(float progress)
        {
            if (_tween != null)
                _tween.Kill(complete: false);
            
            _tween = DOTween.To(value => _slider.value = value, 
                _slider.value, progress, _animationSpeed)
                .SetSpeedBased();
        }
    }
}