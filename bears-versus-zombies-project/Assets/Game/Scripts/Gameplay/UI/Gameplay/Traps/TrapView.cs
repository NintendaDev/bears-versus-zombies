using Modules.Animations.Scripts;
using Modules.VFX;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace SampleGame.Gameplay.UI
{
    public sealed class TrapView : MonoBehaviour
    {
        [SerializeField, Required] private ImageColorAnimator _colorAnimator;
        [SerializeField, Required] private TMP_Text _cost;
        [SerializeField, Required] private TMP_Text _buttonKey;
        
        [Title("Visual")]
        [SerializeField, MinValue(0)] private float _animationDuration = 1f;
        [SerializeField] private Color _defaultColor = new(15f, 39f, 71f, 0.8f);
        [SerializeField] private Color _errorColor = new(152f, 0f, 0f, 0.8f);
        [SerializeField] private Color _successColor = new(33f, 99f, 6f, 0.8f);
        [SerializeField] private VisualEffect _successEffects;
        [SerializeField] private VisualEffect _errorEffects;

        public void SetCost(string cost)
        {
            _cost.text = cost;
        }
        
        public void SetButtonKey(string buttonKey)
        {
            _buttonKey.text = buttonKey;
        }

        public void OnSpawnSuccess()
        {
            _colorAnimator.PlayAnimation(_defaultColor, _successColor, _animationDuration);
            _successEffects?.Play();
        }

        public void OnSpawnError()
        {
            _colorAnimator.PlayAnimation(_defaultColor, _errorColor, _animationDuration);
            _errorEffects?.Play();
        }
    }
}