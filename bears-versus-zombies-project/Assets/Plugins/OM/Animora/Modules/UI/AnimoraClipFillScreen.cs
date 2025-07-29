using OM.Animora.Runtime;
using UnityEngine;

namespace OM.Animora.Modules
{
    [System.Serializable]
    [AnimoraCreate("Fill Screen", "UI/Fill Screen")]
    [AnimoraIcon("AnimationClip Icon")]
    [AnimoraKeywords("fill", "screen")]
    [AnimoraDescription("This is a fill screen action")]
    public class AnimoraClipFillScreen : AnimoraClipWithTarget<RectTransform>
    {
        [OM_StartGroup("Fill Screen", "Settings")]
        [SerializeField] private Vector2 offset = Vector2.zero;
        [SerializeField] private EaseData ease = EaseData.GetDefault();

        private Vector2[] _targetSize;
        private Vector2[] _startSize;
        private Vector2[] _startPosition;

        public override void Enter()
        {
            base.Enter();

            var allTargets = targets.GetTargets();
            _startPosition = new Vector2[allTargets.Count];
            _targetSize = new Vector2[allTargets.Count];
            _startSize = new Vector2[allTargets.Count];

            for (var index = 0; index < allTargets.Count; index++)
            {
                var target = allTargets[index];
                target.anchorMin = new Vector2(0.5f, 0.5f);
                target.anchorMax = new Vector2(0.5f, 0.5f);
                target.pivot = new Vector2(0.5f, 0.5f);

                _startPosition[index] = target.anchoredPosition;
                _targetSize[index] = ((RectTransform)target.parent).rect.size - offset;
                _startSize[index] = target.sizeDelta;
            }
        }
        
        

        public override void OnEvaluate(float time, float clipTime, float normalizedTime, bool isPreviewing)
        {
            base.OnEvaluate(time, clipTime, normalizedTime, isPreviewing);
            
            var list = targets.GetTargets();
            for (var i = 0; i < list.Count; i++)
            {
                var target = list[i];
                target.sizeDelta = Vector2.LerpUnclamped(_startSize[i], _targetSize[i], ease.Evaluate(normalizedTime));
                target.anchoredPosition = Vector2.LerpUnclamped(_startPosition[i], Vector2.zero, ease.Evaluate(normalizedTime));
            }
        }

        public override void OnPreviewChanged(AnimoraPlayer animoraPlayer, bool isOn)
        {
            base.OnPreviewChanged(animoraPlayer, isOn);

            var list = targets.GetTargets();
            for (var i = 0; i < list.Count; i++)
            {
                var target = list[i];
            
                AnimoraPreviewManager.RecordOrUndoObject(isOn,this,target.sizeDelta, (e)=> target.sizeDelta = e);
                AnimoraPreviewManager.RecordOrUndoObject(isOn,this,target.anchorMin, (e)=> target.anchorMin = e);
                AnimoraPreviewManager.RecordOrUndoObject(isOn,this,target.anchorMax, (e)=> target.anchorMax = e);
                AnimoraPreviewManager.RecordOrUndoObject(isOn,this,target.pivot, (e)=> target.pivot = e);
                AnimoraPreviewManager.RecordOrUndoObject(isOn,this,target.anchoredPosition, (e)=> target.anchoredPosition = e);
            }

        }

        public override void OnCreate(AnimoraPlayer player)
        {
            base.OnCreate(player);
            ActionsManager.AddActionDirect(AnimoraAction.CreateInstance<AnimoraActionSetAnchorPosition>(), player);
        }
    }
}