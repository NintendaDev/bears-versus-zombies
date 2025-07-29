using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace SampleGame.Gameplay.UI
{
    public sealed class AutoKillView : MonoBehaviour
    {
        [SerializeField, Required] private Image _background;
        [SerializeField, Required] private Image _indicator;
        
        private bool _isVisible;

        public void SetPercent(float percent)
        { 
            if (percent > 0 && _isVisible == false)
                Show();
            
            if (percent <= 0 && _isVisible)
                Hide();
            
            _indicator.fillAmount = percent;
        }
        
        private void Show()
        {
            _background.enabled = true;
            _indicator.enabled = true;
            _isVisible = true;
        }

        private void Hide()
        {
            _background.enabled = false;
            _indicator.enabled = false;
            _isVisible = false;
        }
    }
}