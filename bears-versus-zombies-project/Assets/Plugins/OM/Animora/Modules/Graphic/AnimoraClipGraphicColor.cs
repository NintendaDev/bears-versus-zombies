using OM.Animora.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace OM.Animora.Modules
{
    [System.Serializable]
    [AnimoraCreate("Color Image", "UI/Color Image")]
    [AnimoraIcon("d_RectTransform Icon")]
    [AnimoraKeywords("Color", "Image", "action")]
    [AnimoraDescription("This is a set Color Image action")]
    public class AnimoraClipGraphicColor : AnimoraClipWithTarget<Graphic>
    {
        [SerializeField] private AnimoraInterpolation<Color> color;
        
        public override void Enter()
        {
            base.Enter();
            color.Setup(targets.GetTargets(), (i) => targets.GetTargetAt(i).color,
                (value1, value2) => value1 * value2, CurrentPlayDirection);
        }
        
        public override void OnEvaluate(float time, float clipTime, float normalizedTime, bool isPreviewing)
        {
            base.OnEvaluate(time, clipTime, normalizedTime, isPreviewing);
            for (var i = 0; i < targets.GetTargets().Count; i++)
            {
                var target = targets.GetTargetAt(i);
                if (target == null) continue;
                target.color = color.Interpolate(normalizedTime,i,Color.LerpUnclamped);
                if (isPreviewing)
                {
                    SetDirty();
                }
            }
        }
        
        public override void OnPreviewChanged(AnimoraPlayer animoraPlayer, bool isOn)
        {
            base.OnPreviewChanged(animoraPlayer, isOn);
            for (var i = 0; i < targets.GetTargets().Count; i++)
            {
                var target = targets.GetTargetAt(i);
                if (target == null) continue;
                AnimoraPreviewManager.RecordOrUndoObject(isOn, this, target.color,
                    (value) => target.color = value);
                SetDirty();
            }
        }

        public override void OnCreate(AnimoraPlayer player)
        {
            base.OnCreate(player);
            ActionsManager.AddActionDirect(AnimoraAction.CreateInstance<AnimoraActionSetGraphicColor>(), player);
        }

        private void SetDirty()
        {
#if UNITY_EDITOR
            for (var i = 0; i < targets.GetTargets().Count; i++)
            {
                var target = targets.GetTargetAt(i);
                if (target == null) continue;
                UnityEditor.EditorUtility.SetDirty(target);
            }
#endif
        }
    }
}