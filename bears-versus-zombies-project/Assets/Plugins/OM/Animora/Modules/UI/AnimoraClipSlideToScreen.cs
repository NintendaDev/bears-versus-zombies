using OM.Animora.Runtime;
using UnityEngine;

namespace OM.Animora.Modules
{
    [System.Serializable]
    [AnimoraCreate("Slide To Screen", "UI/Slide To Screen")]
    [AnimoraIcon("AnimationClip Icon")]
    [AnimoraKeywords("slide", "to", "screen", "action")]
    [AnimoraDescription("This is a slide to screen action")]
    public class AnimoraClipSlideToScreen : AnimoraClipWithTarget<RectTransform>
    {
        public enum ScreenSlideType
        {
            FromLeft,
            FromRight,
            FromTop,
            FromBottom,
        }
        
        [OM_StartGroup("Screen Slide Settings", "Settings")]
        [SerializeField] private ScreenSlideType slideType;
        [SerializeField] private EaseData ease = new EaseData(EasingFunction.OutBack);
        
        
        public override void Enter()
        {
            base.Enter();

            var list = targets.GetTargets();
            for (var index = 0; index < list.Count; index++)
            {
                var target = list[index];
                target.anchorMin = new Vector2(0.5f, 0.5f);
                target.anchorMax = new Vector2(0.5f, 0.5f);
                target.pivot = new Vector2(0.5f, 0.5f);
                target.sizeDelta = ((RectTransform)target.parent).rect.size;
                target.anchoredPosition = GetStartPosition(index);
            }
        }


        private Vector2 GetStartPosition(int index)
        {
            var rectTransform = targets.GetTargetAt(index);
            switch (slideType)
            {
                case ScreenSlideType.FromLeft:
                    return new Vector2(-rectTransform.sizeDelta.x, 0);
                case ScreenSlideType.FromRight:
                    return new Vector2(rectTransform.sizeDelta.x, 0);
                case ScreenSlideType.FromTop:
                    return new Vector2(0, rectTransform.sizeDelta.y);
                case ScreenSlideType.FromBottom:
                    return new Vector2(0, -rectTransform.sizeDelta.y);
                default:
                    return Vector2.zero;
            }
        }
        
        
        public override void OnEvaluate(float time, float clipTime, float normalizedTime, bool isPreviewing)
        {
            base.OnEvaluate(time, clipTime, normalizedTime, isPreviewing);

            var list = targets.GetTargets();
            for (var i = 0; i < list.Count; i++)
            {
                var rectTransform = targets.GetTargetAt(i);
                rectTransform.anchoredPosition =
                    Vector2.LerpUnclamped(GetStartPosition(i), Vector2.zero, ease.Evaluate(normalizedTime));
            }
        }
        
        public override void OnPreviewChanged(AnimoraPlayer animoraPlayer, bool isOn)
        {
            base.OnPreviewChanged(animoraPlayer, isOn);
            
            var list = targets.GetTargets();
            for (var i = 0; i < list.Count; i++)
            {
                var rectTransform = targets.GetTargetAt(i);
            
                AnimoraPreviewManager.RecordOrUndoObject(isOn,this,rectTransform.sizeDelta, (e)=> rectTransform.sizeDelta = e);
                AnimoraPreviewManager.RecordOrUndoObject(isOn,this,rectTransform.anchorMin, (e)=> rectTransform.anchorMin = e);
                AnimoraPreviewManager.RecordOrUndoObject(isOn,this,rectTransform.anchorMax, (e)=> rectTransform.anchorMax = e);
                AnimoraPreviewManager.RecordOrUndoObject(isOn,this,rectTransform.pivot, (e)=> rectTransform.pivot = e);
                AnimoraPreviewManager.RecordOrUndoObject(isOn,this,rectTransform.anchoredPosition, (e)=> rectTransform.anchoredPosition = e);
            }
            
        }
    }
}