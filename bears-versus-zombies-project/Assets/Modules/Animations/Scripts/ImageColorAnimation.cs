using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.Animations.Scripts
{
    public sealed class ImageColorAnimator : MonoBehaviour
    {
        [SerializeField, Required] private Image _image;
        
        private Tween _tween;
        
        public void OnDestroy()
        {
            DisposeTween();
        }

        public void PlayAnimation(Color source, Color destination,   float duration)
        {
            DisposeTween();
            _image.color = destination;
            _tween = _image.DOColor(source, duration: duration);
        }

        private void DisposeTween()
        {
            if (_tween != null)
                _tween.Kill(complete: true);
        }
    }
}