using OM.Animora.Runtime;
using UnityEngine;

namespace OM.Animora.Demos
{
    public class Demo6Switcher : MonoBehaviour
    {
        [SerializeField] private bool startValue = true;
        [SerializeField] private AnimoraPlayer animationOn;
        [SerializeField] private AnimoraPlayer animationOff;
    
        private bool _isOn = true;

        private void Awake()
        {
            _isOn = startValue;
            Animate();
        }

        public void Toggle()
        {
            _isOn = !_isOn;

            Animate();
        }

        public void Animate()
        {
            if (_isOn)
            {
                animationOff?.StopAnimation();
                animationOn?.PlayAnimation();
            }
            else
            {
                animationOn?.StopAnimation();
                animationOff?.PlayAnimation();
            }
        }
    }
}
