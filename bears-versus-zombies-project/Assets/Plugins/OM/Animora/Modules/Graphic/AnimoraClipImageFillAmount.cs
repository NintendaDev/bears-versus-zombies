using OM.Animora.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace OM.Animora.Modules
{
    [System.Serializable]
    [AnimoraCreate("Fill Amount", "UI/Fill Amount")]
    [AnimoraIcon("d_Image Icon")]
    [AnimoraKeywords("set", "Fill", "Amount")]
    [AnimoraDescription("This is a set Fill Amount action")]
    public class AnimoraClipImageFillAmount : AnimoraClipWithTarget<Image>
    {
        [SerializeField] private AnimoraInterpolation<float> fillAmount;
        
        public override void Enter()
        {
            base.Enter();
            fillAmount.Setup(targets.GetTargets(), (i) => targets.GetTargetAt(i).fillAmount,
                (value1, value2) => value1 + value2, CurrentPlayDirection);
        }
        
        public override void OnEvaluate(float time, float clipTime, float normalizedTime, bool isPreviewing)
        {
            base.OnEvaluate(time, clipTime, normalizedTime, isPreviewing);
            for (var i = 0; i < targets.GetTargets().Count; i++)
            {
                var target = targets.GetTargetAt(i);
                if (target == null) continue;
                target.fillAmount = fillAmount.Interpolate(normalizedTime,i,Mathf.LerpUnclamped);
            }
        }
        
        public override void OnPreviewChanged(AnimoraPlayer animoraPlayer, bool isOn)
        {
            base.OnPreviewChanged(animoraPlayer, isOn);
            for (var i = 0; i < targets.GetTargets().Count; i++)
            {
                var target = targets.GetTargetAt(i);
                if (target == null) continue;
                AnimoraPreviewManager.RecordOrUndoObject(isOn, this, target.fillAmount,
                    (value) => target.fillAmount = value);
            }
        }
        
        public override void OnCreate(AnimoraPlayer player)
        {
            base.OnCreate(player);
            ActionsManager.AddActionDirect(AnimoraAction.CreateInstance<AnimoraActionSetFillAmount>(), player);
        }

    }
}